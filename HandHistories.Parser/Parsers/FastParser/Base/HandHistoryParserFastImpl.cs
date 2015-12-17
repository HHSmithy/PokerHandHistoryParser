using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Utils.Pot;
using HandHistories.Parser.Utils.Uncalled;
using HandHistories.Parser.Utils.RaiseAdjuster;

namespace HandHistories.Parser.Parsers.FastParser.Base
{
    abstract public class HandHistoryParserFastImpl : IHandHistoryParser
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly Regex HandSplitRegex = new Regex("\r\n\r\n", RegexOptions.Compiled);

        public abstract SiteName SiteName { get; }

        public virtual bool RequiresAdjustedRaiseSizes
        {
            get { return false; }
        }
               
        public virtual bool RequiresActionSorting
        {
            get { return false; }
        }

        public virtual bool RequiresAllInDetection
        {
            get { return false; }
        }

        public virtual bool RequiresTotalPotCalculation
        {
            get { return false; }
        }

        public virtual bool RequiresUncalledBetFix
        {
            get { return false; }
        }

        public virtual bool SupportRunItTwice
        {
            get { return false; }
        }
     
        public virtual IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                            .Where(s => string.IsNullOrWhiteSpace(s) == false)
                            .Select(s => s.Trim('\r', 'n'));
        }

        public virtual IEnumerable<string[]> SplitUpMultipleHandsToLines(string rawHandHistories)
        {
            foreach (var hand in SplitUpMultipleHands(rawHandHistories))
            {
                yield return SplitHandsLines(hand);
            }
        }

        protected virtual string [] SplitHandsLines(string handText)
        {
            string[] text = handText.Split(new [] { '\n' , '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < text.Length; i++)
			{
                text[i] = text[i].Trim();
			}
            return text;
        }

        public HandHistorySummary ParseFullHandSummary(string handText, bool rethrowExceptions = false)
        {
            try
            {
                string[] lines = SplitHandsLines(handText);

                bool isCancelled;
                if (IsValidOrCancelledHand(lines, out isCancelled) == false)
                {
                    throw new InvalidHandException(handText ?? "NULL");
                }                

                return ParseFullHandSummary(lines, isCancelled);
            }
            catch (Exception ex)
            {
                if (rethrowExceptions)
                {
                    throw;
                }

                logger.Warn("Couldn't parse hand {0} with error {1} and trace {2}", handText, ex.Message,ex.StackTrace);
                return null;
            }     
        }

        protected HandHistorySummary ParseFullHandSummary(string[] handLines, bool isCancelled = false)
        {
            HandHistorySummary handHistorySummary = new HandHistorySummary();

            handHistorySummary.Cancelled = isCancelled;
            handHistorySummary.DateOfHandUtc = ParseDateUtc(handLines);
            handHistorySummary.GameDescription = ParseGameDescriptor(handLines);
            handHistorySummary.HandId = ParseHandId(handLines);
            handHistorySummary.TableName = ParseTableName(handLines);
            handHistorySummary.NumPlayersSeated = ParsePlayers(handLines).Count;
            handHistorySummary.DealerButtonPosition = ParseDealerPosition(handLines);
            handHistorySummary.FullHandHistoryText = string.Join("\r\n", handLines);

            try
            {                
                ParseExtraHandInformation(handLines, handHistorySummary);
            }
            catch
            {
                throw new ExtraHandParsingAction(handLines[0]);
            }            
            
            return handHistorySummary;
        }

        protected virtual void ParseExtraHandInformation(string[] handLines, HandHistorySummary handHistorySummary)
        {
            // do nothing
        }

        public HandHistory ParseFullHandHistory(string handText, bool rethrowExceptions = false)
        {
            string[] handLines;

            try
            {
               handLines = SplitHandsLines(handText);
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

            return ParseFullHandHistory(handLines, rethrowExceptions);
        }

        public HandHistory ParseFullHandHistory(string[] handLines, bool rethrowExceptions = false)
        {
            try
            {
                bool isCancelled;
                if (IsValidOrCancelledHand(handLines, out isCancelled) == false)
                {
                    throw new InvalidHandException(string.Join("\r\n", handLines));                    
                }

                //Set members outside of the constructor for easier performance analysis
                HandHistory handHistory = new HandHistory();

                handHistory.FullHandHistoryText = string.Join("\r\n", handLines);
                handHistory.DateOfHandUtc = ParseDateUtc(handLines);
                handHistory.GameDescription = ParseGameDescriptor(handLines);
                handHistory.HandId = ParseHandId(handLines);
                handHistory.TableName = ParseTableName(handLines);
                handHistory.DealerButtonPosition = ParseDealerPosition(handLines);
                handHistory.ComumnityCards = ParseCommunityCards(handLines);
                handHistory.Cancelled = isCancelled;
                handHistory.Players = ParsePlayers(handLines);
                handHistory.NumPlayersSeated = handHistory.Players.Count;

                string heroName = ParseHeroName(handLines);
                handHistory.Hero = handHistory.Players.FirstOrDefault(p => p.PlayerName == heroName);

                if (handHistory.Cancelled)
                {
                    return handHistory;
                }

                if (handHistory.Players.Count(p => p.IsSittingOut == false) <= 1)
                {
                    throw new PlayersException(string.Join("\r\n", handLines), "Only found " + handHistory.Players.Count + " players with actions.");
                }

                if (SupportRunItTwice)
                {
                    handHistory.RunItTwiceData = ParseRunItTwice(handLines);
                }

                handHistory.HandActions = ParseHandActions(handLines, handHistory.GameDescription.GameType);

                if (RequiresActionSorting)
                {
                    handHistory.HandActions = OrderHandActions(handHistory.HandActions);
                }
                if (RequiresAdjustedRaiseSizes)
                {
                   handHistory.HandActions = RaiseAdjuster.AdjustRaiseSizes(handHistory.HandActions);                   
                }
                if (RequiresAllInDetection)
                {
                    handHistory.HandActions = IdentifyAllInActions(handLines, handHistory.HandActions);
                }
                if (RequiresTotalPotCalculation)
                {
                    handHistory.TotalPot = PotCalculator.CalculateTotalPot(handHistory);
                    handHistory.Rake = PotCalculator.CalculateRake(handHistory);
                }

                if (RequiresUncalledBetFix)
                {
                    handHistory.HandActions = UncalledBet.Fix(handHistory.HandActions);
                }

                HandAction anteAction = handHistory.HandActions.FirstOrDefault(a => a.HandActionType == HandActionType.ANTE);
                if (anteAction != null && handHistory.GameDescription.PokerFormat.Equals(PokerFormat.CashGame))
                {
                    handHistory.GameDescription.Limit.IsAnteTable = true;
                    handHistory.GameDescription.Limit.Ante = Math.Abs(anteAction.Amount);
                }

                try
                {
                    ParseExtraHandInformation(handLines, handHistory);
                }
                catch (Exception)
                {
                    throw new ExtraHandParsingAction(handLines[0]);
                }
             
                return handHistory;
            }
            catch (Exception ex)
            {
                if (rethrowExceptions)
                {
                    throw;
                }

                logger.Warn("Couldn't parse hand {0} with error {1} and trace {2}", string.Join("\r\n", handLines), ex.Message, ex.StackTrace);
                return null;
            }        
        }

        protected abstract string ParseHeroName(string[] handlines);

        public string ParseHeroName(string handText)
        {
            return ParseHeroName(SplitHandsLines(handText));
        }

        public int ParseDealerPosition(string handText)
        {
            try
            {
                return ParseDealerPosition(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new PlayersException(handText, "ParseDealerPosition: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }  
        }

        protected abstract int ParseDealerPosition(string[] handLines);

        public DateTime ParseDateUtc(string handText)
        {
            try
            {
                return ParseDateUtc(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new ParseHandDateException(handText, "ParseDateUtc: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }            
        }

        protected abstract DateTime ParseDateUtc(string[] handLines);

        public long ParseHandId(string handText)
        {
            try
            {
                return ParseHandId(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new HandIdException(handText, "ParseHandId: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            } 
        }

        protected abstract long ParseHandId(string[] handLines);

        public long ParseTournamentId(string handText)
        {
            try
            {
                return ParseTournamentId(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new TournamentIdException(handText, "ParseTournamentId: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract long ParseTournamentId(string[] handLines);

        public string ParseTableName(string handText)
        {
            try
            {
                return ParseTableName(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new TableNameException(handText, "ParseTableName: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            } 
        }

        protected abstract string ParseTableName(string[] handLines);

        public GameDescriptor ParseGameDescriptor(string handText)
        {
            string[] lines = SplitHandsLines(handText);

            return ParseGameDescriptor(lines);
        }

        protected GameDescriptor ParseGameDescriptor(string[] handLines)
        {
            var format = ParsePokerFormat(handLines);


            switch (format)
            {
                case PokerFormat.CashGame:
                    return new GameDescriptor(format,
                                         SiteName,
                                         ParseGameType(handLines),
                                         ParseLimit(handLines),
                                         ParseTableType(handLines),
                                         ParseSeatType(handLines));

                case PokerFormat.SitAndGo:
                    return new GameDescriptor(format,
                                       SiteName,
                                       ParseGameType(handLines),
                                       ParseLimit(handLines),
                                       ParseBuyin(handLines),
                                       ParseTableType(handLines),
                                       ParseSeatType(handLines)); 

                case PokerFormat.MultiTableTournament:
                    return new GameDescriptor(format,
                                        SiteName,
                                        ParseGameType(handLines),
                                        ParseLimit(handLines),
                                        ParseBuyin(handLines),
                                        ParseTableType(handLines),
                                        ParseSeatType(handLines));

                default:
                    throw new PokerFormatException(handLines[0], "Unrecognized PokerFormat for our GameDescriptor:" + format);
            }
        }

        public PokerFormat ParsePokerFormat(string handText)
        {
            try
            {
                return ParsePokerFormat(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new PokerFormatException(handText, "ParsePokerFormat: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            } 
        }

        protected abstract PokerFormat ParsePokerFormat(string[] handLines);

        public SeatType ParseSeatType(string handText)
        {
            try
            {
                return ParseSeatType(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new SeatTypeException(handText, "ParseSeatType: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            } 
        }

        protected abstract SeatType ParseSeatType(string [] handLines);

        public GameType ParseGameType(string handText)
        {
            try
            {
                return ParseGameType(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new UnrecognizedGameTypeException(handText, "ParseGameType: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            } 
        }

        protected abstract GameType ParseGameType(string[] handLines);

        public TableType ParseTableType(string handText)
        {
            try
            {
                return ParseTableType(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new TableTypeException(handText, "ParseTableType: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            } 
        }

        protected abstract TableType ParseTableType(string[] handLines);

        public Limit ParseLimit(string handText)
        {
            try
            {
                return ParseLimit(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new LimitException(handText, "ParseLimit: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            } 
        }

        protected abstract Limit ParseLimit(string[] handLines);

        public Buyin ParseBuyin(string handText)
        {
            try
            {
                return ParseBuyin(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new BuyinException(handText, "ParseBuyin: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        protected abstract Buyin ParseBuyin(string[] handLines);

        public int ParseNumPlayers(string handText)
        {
            try
            {
                return ParsePlayers(SplitHandsLines(handText)).Count;
            }
            catch (Exception ex)
            {
                throw new PlayersException(handText, "ParseNumPlayers: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            } 
        }

        public bool IsValidHand(string handText)
        {
            return IsValidHand(SplitHandsLines(handText));
        }

        public abstract bool IsValidHand(string[] handLines);

        public bool IsValidOrCancelledHand(string handText, out bool isCancelled)
        {
            return IsValidOrCancelledHand(SplitHandsLines(handText), out isCancelled);
        }

        public abstract bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled);

        public List<HandAction> ParseHandActions(string handText)
        {
            try
            {
                string [] handLines = SplitHandsLines(handText);
                GameType gameType = ParseGameType(handLines);
                List<HandAction> handActions = ParseHandActions(handLines, gameType);

                if (RequiresActionSorting)
                {
                    handActions = OrderHandActions(handActions);
                }
                if (RequiresAdjustedRaiseSizes)
                {
                    handActions = RaiseAdjuster.AdjustRaiseSizes(handActions);
                }
                if (RequiresAllInDetection)
                {
                    handActions = IdentifyAllInActions(handLines, handActions);
                }

                return handActions;
            }
            catch (Exception ex)
            {
                throw new HandActionException(handText, "ParseHandActions: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            }
        }

        // We pass the game-type in as it can change the actions and parsing logic
        protected abstract List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown);

        /// <summary>
        /// Some sites (like IPoker) don't specifically identify All-In calls/raises. In these cases we need to parse the actions 
        /// and reclassify certain actions as all-in
        /// </summary>
        protected List<HandAction> IdentifyAllInActions(string[] handLines, List<HandAction> handActions)
        {
            PlayerList playerList = ParsePlayers(handLines);

            Dictionary<string, decimal> playerStackRemaining = new Dictionary<string, decimal>();

            foreach (Player player in playerList)
            {
                playerStackRemaining.Add(player.PlayerName, player.StartingStack);
            }

            List<HandAction> identifiedActions = new List<HandAction>(handActions.Count);

            foreach (HandAction action in handActions)
            {
                //Negative amounts represent putting money into the pot - ignore actions which aren't negative
                if (action.Amount >= 0)
                {
                    identifiedActions.Add(action);
                    continue;
                }

                //Skip actions which have already been identified
                if (action.IsAllIn)
                {
                    identifiedActions.Add(action);       
                    continue;
                }

                //Update the remaining stack with our action's amount
                playerStackRemaining[action.PlayerName] += action.Amount;

                if (playerStackRemaining[action.PlayerName] == 0)
                {
                    HandAction allInAction = new HandAction(action.PlayerName, action.HandActionType, action.Amount, action.Street, true);
                    identifiedActions.Add(allInAction);
                }
                else
                {
                    identifiedActions.Add(action);       
                }
            }

            return identifiedActions;
        }

        /// <summary>
        /// Sometimes hand actions are listed out of order, but with an order number or timestamp (this happens on IPoker).
        /// In these cases the hands must be reorganized before being displayed.
        /// </summary>
        protected List<HandAction> OrderHandActions(List<HandAction> handActions)
        {
            return handActions.OrderBy(action => action.ActionNumber).ToList();
        }
      
        public PlayerList ParsePlayers(string handText)
        {
            try
            {
                return ParsePlayers(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new PlayersException(handText, "ParsePlayers: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            } 
        }

        protected abstract PlayerList ParsePlayers(string[] handLines);

        public BoardCards ParseCommunityCards(string handText)
        {
            try
            {
                return ParseCommunityCards(SplitHandsLines(handText));
            }
            catch (Exception ex)
            {
                throw new CardException(handText, "ParseCommunityCards: Error:" + ex.Message + " Stack:" + ex.StackTrace);
            } 
        }

        protected abstract BoardCards ParseCommunityCards(string[] handLines);

        public virtual RunItTwice ParseRunItTwice(string[] handLines)
        {
            throw new NotImplementedException();
        }
    }
}
