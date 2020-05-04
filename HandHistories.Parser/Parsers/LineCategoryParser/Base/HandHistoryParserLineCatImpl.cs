using HandHistories.Parser.Parsers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Utils.AllInAction;
using HandHistories.Parser.Utils.RaiseAdjuster;
using HandHistories.Parser.Utils.Uncalled;
using HandHistories.Parser.Utils.Pot;

namespace HandHistories.Parser.Parsers.LineCategoryParser.Base
{
    abstract class HandHistoryParserLineCatImpl : IHandHistoryParser
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// We use a [ThreadStaticAttribute] here to avoid allocations of new Categories. Categories are only alive while within a parsing method
        /// </summary>
        [ThreadStatic]
        internal static Categories Lines;

        private Categories GetCategories(string hand)
        {
            if (Lines == null)
            {
                Lines = new Categories();
            }
            Lines.Clear();

            Categorize(Lines, SplitHandsLines(hand));
            return Lines;
        }

        public virtual bool RequiresAdjustedRaiseSizes => false;

        public virtual bool RequiresActionSorting => false;

        public virtual bool RequiresAllInDetection => false;

        public virtual bool RequiresAllInUpdates => false;

        public virtual bool RequiresTotalPotCalculation => false;

        public virtual bool RequiresUncalledBetFix => false;

        /// <summary>
        /// Adjusts the WinActions to not include any uncalled bets
        /// </summary>
        public virtual bool RequiresUncalledBetWinAdjustment => false;

        public virtual bool RequiresTotalPotAdjustment => false;
        
        public abstract void Categorize(Categories cat, string[] lines);

        public abstract SiteName SiteName { get; }

        protected virtual string[] SplitHandsLines(string handText)
        {
            string[] text = handText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < text.Length; i++)
            {
                text[i] = text[i].Trim();
            }
            return text;
        }

        public bool IsValidHand(string handText)
        {
            return IsValidHand(GetCategories(handText));
        }

        protected abstract bool IsValidHand(Categories lines);

        public bool IsValidOrCancelledHand(string handText, out bool isCancelled)
        {
            return IsValidOrCancelledHand(GetCategories(handText), out isCancelled);
        }

        protected abstract bool IsValidOrCancelledHand(Categories lines, out bool isCancelled);

        public BoardCards ParseCommunityCards(string handText)
        {
            return ParseCommunityCards(GetCategories(handText).Summary);
        }

        protected abstract BoardCards ParseCommunityCards(List<string> summary);

        public DateTime ParseDateUtc(string handText)
        {
            return ParseDateUtc(GetCategories(handText).Header);
        }

        protected abstract DateTime ParseDateUtc(List<string> header);

        public int ParseDealerPosition(string handText)
        {
            return ParseDealerPosition(GetCategories(handText).Header);
        }

        protected abstract int ParseDealerPosition(List<string> header);

        public HandHistory ParseFullHandHistory(string handText, bool rethrowExceptions = false)
        {
            GetCategories(handText);

            try
            {
                bool isCancelled;
                if (IsValidOrCancelledHand(Lines, out isCancelled) == false)
                {
                    throw new InvalidHandException(handText);
                }

                //Set members outside of the constructor for easier performance analysis
                HandHistory handHistory = new HandHistory();

                handHistory.FullHandHistoryText = handText;
                handHistory.DateOfHandUtc = ParseDateUtc(Lines.Header);
                handHistory.GameDescription = ParseGameDescriptor(Lines.Header);
                handHistory.HandId = ParseHandId(Lines.Header);
                handHistory.TableName = ParseTableName(Lines.Header);
                handHistory.DealerButtonPosition = ParseDealerPosition(Lines.Header);
                handHistory.CommunityCards = ParseCommunityCards(Lines.Summary);
                handHistory.Cancelled = isCancelled;
                handHistory.Players = ParsePlayers(Lines.Seat);
                handHistory.NumPlayersSeated = handHistory.Players.Count;

                string heroName = ParseHeroName(Lines.Other);
                handHistory.Hero = handHistory.Players.FirstOrDefault(p => p.PlayerName == heroName);

                if (handHistory.Cancelled)
                {
                    return handHistory;
                }

                if (handHistory.Players.Count(p => p.IsSittingOut == false) <= 1)
                {
                    throw new PlayersException(handText, "Only found " + handHistory.Players.Count + " players with actions.");
                }

                //    if (SupportRunItTwice)
                //    {
                //        handHistory.RunItTwiceData = ParseRunItTwice(handLines);
                //    }

                List<WinningsAction> winners;
                handHistory.HandActions = ParseHandActions(Lines.Action, out winners);
                handHistory.Winners = winners;

                if (RequiresAdjustedRaiseSizes)
                {
                    handHistory.HandActions = RaiseAdjuster.AdjustRaiseSizes(handHistory.HandActions);
                }
                if (RequiresAllInDetection)
                {
                    var playerList = ParsePlayers(Lines.Seat);
                    handHistory.HandActions = AllInActionHelper.IdentifyAllInActions(playerList, handHistory.HandActions);
                }

                if (RequiresAllInUpdates)
                {
                    handHistory.HandActions = AllInActionHelper.UpdateAllInActions(handHistory.HandActions);
                }

                if (RequiresUncalledBetFix)
                {
                    handHistory.HandActions = UncalledBet.Fix(handHistory.HandActions);
                }

                if (RequiresUncalledBetWinAdjustment)
                {
                    winners = UncalledBet.FixUncalledBetWins(handHistory.HandActions, winners);
                }

                //Pot calculation mus be done after uncalledBetFix
                if (RequiresTotalPotCalculation)
                {
                    handHistory.TotalPot = PotCalculator.CalculateTotalPot(handHistory);
                    handHistory.Rake = PotCalculator.CalculateRake(handHistory);
                }

                HandAction anteAction = handHistory.HandActions.FirstOrDefault(a => a.HandActionType == HandActionType.ANTE);
                if (anteAction != null && handHistory.GameDescription.PokerFormat.Equals(PokerFormat.CashGame))
                {
                    handHistory.GameDescription.Limit.IsAnteTable = true;
                    handHistory.GameDescription.Limit.Ante = Math.Abs(anteAction.Amount);
                }

                ParseExtraHandInformation(Lines, handHistory);

                if (handHistory.Players.All(p => p.SeatNumber != handHistory.DealerButtonPosition))
                {
                    throw new InvalidHandException(handText, "Dealer not found");
                }

                FinalizeHand(handHistory);

                SetActionNumbers(handHistory);

                Lines.Clear();
                return handHistory;
            }
            catch (Exception ex)
            {
                if (rethrowExceptions)
                {
                    throw;
                }

                logger.Warn("Couldn't parse hand {0} with error {1} and trace {2}", handText, ex.Message, ex.StackTrace);
                return null;
            }
        }

        public HandHistorySummary ParseFullHandSummary(string handText, bool rethrowExceptions = false)
        {
            Lines.Clear();

            var textlines = SplitHandsLines(handText);
            Categorize(Lines, textlines);

            try
            {
                bool isCancelled;
                if (IsValidOrCancelledHand(Lines, out isCancelled) == false)
                {
                    throw new InvalidHandException(string.Join("\r\n", textlines));
                }

                HandHistorySummary handHistorySummary = new HandHistorySummary();

                handHistorySummary.Cancelled = isCancelled;
                handHistorySummary.DateOfHandUtc = ParseDateUtc(Lines.Header);
                handHistorySummary.GameDescription = ParseGameDescriptor(Lines.Header);
                handHistorySummary.HandId = ParseHandId(Lines.Header);
                handHistorySummary.TableName = ParseTableName(Lines.Header);
                handHistorySummary.DealerButtonPosition = ParseDealerPosition(Lines.Header);
                handHistorySummary.NumPlayersSeated = ParsePlayers(Lines.Seat).Count;
                handHistorySummary.FullHandHistoryText = handText;

                try
                {
                    ParseExtraHandInformation(Lines, handHistorySummary);
                }
                catch
                {
                    throw new ExtraHandParsingAction(handText);
                }

                return handHistorySummary;
            }
            catch (Exception ex)
            {
                if (rethrowExceptions)
                {
                    throw;
                }

                logger.Warn("Couldn't parse hand {0} with error {1} and trace {2}", string.Join("\r\n", textlines), ex.Message, ex.StackTrace);
                return null;
            }
        }

        public GameDescriptor ParseGameDescriptor(string handText)
        {
            return ParseGameDescriptor(GetCategories(handText).Header);
        }

        protected GameDescriptor ParseGameDescriptor(List<string> header)
        {
            var format = ParsePokerFormat(header);
            return new GameDescriptor(format,
                SiteName,
                ParseGameType(header),
                ParseLimit(header),
                ParseTableType(header),
                ParseSeatType(header));
        }
        
        public GameType ParseGameType(string handText)
        {
            return ParseGameType(GetCategories(handText).Header);
        }

        protected abstract GameType ParseGameType(List<string> header);

        public List<HandAction> ParseHandActions(string handText, out List<WinningsAction> winners)
        {
            try
            {
                var handactions = ParseHandActions(GetCategories(handText).Action, out winners);

                if (RequiresAdjustedRaiseSizes)
                {
                    handactions = RaiseAdjuster.AdjustRaiseSizes(handactions);
                }
                if (RequiresAllInDetection)
                {
                    var playerList = ParsePlayers(Lines.Seat);
                    handactions = AllInActionHelper.IdentifyAllInActions(playerList, handactions);
                }
                if (RequiresAllInUpdates)
                {
                    handactions = AllInActionHelper.UpdateAllInActions(handactions);
                }
                if (RequiresUncalledBetWinAdjustment)
                {
                    winners = UncalledBet.FixUncalledBetWins(handactions, winners);
                }

                return handactions;
            }
            catch (Exception ex)
            {
                throw new HandActionException(handText, "ParseHandActions: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
           
        }

        protected abstract List<HandAction> ParseHandActions(List<string> actions, out List<WinningsAction> winners);

        public long[] ParseHandId(string handText)
        {
            return ParseHandId(GetCategories(handText).Header);
        }

        protected abstract long[] ParseHandId(List<string> header);

        public string ParseHeroName(string handText)
        {
            return ParseHeroName(GetCategories(handText).Other);
        }

        protected abstract string ParseHeroName(List<string> action);

        public Limit ParseLimit(string handText)
        {
            return ParseLimit(GetCategories(handText).Header);
        }

        protected abstract Limit ParseLimit(List<string> header);

        public int ParseNumPlayers(string handText)
        {
            return ParsePlayers(handText).Count;
        }

        public PlayerList ParsePlayers(string handText)
        {
            try
            {
                return ParsePlayers(GetCategories(handText).Seat);
            }
            catch (Exception ex)
            {
                throw new PlayersException(handText, "ParsePlayers: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract PlayerList ParsePlayers(List<string> seats);

        public PokerFormat ParsePokerFormat(string handText)
        {
            return ParsePokerFormat(GetCategories(handText).Header);
        }

        protected abstract PokerFormat ParsePokerFormat(List<string> header);

        public SeatType ParseSeatType(string handText)
        {
            return ParseSeatType(GetCategories(handText).Header);
        }

        protected abstract SeatType ParseSeatType(List<string> header);

        public string ParseTableName(string handText)
        {
            return ParseTableName(GetCategories(handText).Header);
        }

        protected abstract string ParseTableName(List<string> header);

        public TableType ParseTableType(string handText)
        {
            return ParseTableType(GetCategories(handText).Header);
        }

        protected abstract TableType ParseTableType(List<string> header);

        public virtual IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            throw new NotImplementedException();
        }

        protected virtual void ParseExtraHandInformation(Categories lines, HandHistorySummary handHistorySummary)
        {
        }

        public virtual void FinalizeHand(HandHistory hand)
        {
        }

        private static void SetActionNumbers(HandHistory handHistory)
        {
            for (int i = 0; i < handHistory.HandActions.Count; i++)
            {
                var action = handHistory.HandActions[i];
                action.ActionNumber = i;
            }
        }
    }
}
