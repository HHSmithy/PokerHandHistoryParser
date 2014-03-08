using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;

namespace HandHistories.Parser.Parsers.FastParser.Base
{
    abstract public class HandHistoryParserFastImpl : IHandHistoryParser
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly Regex LineSplitRegex = new Regex("\n|\r", RegexOptions.Compiled);
        private static readonly Regex HandSplitRegex = new Regex("\r\n\r\n", RegexOptions.Compiled);

        public abstract SiteName SiteName { get; }

        public virtual bool RequresAdjustedRaiseSizes
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
     
        public virtual IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                            .Where(s => string.IsNullOrWhiteSpace(s) == false)
                            .Select(s => s.Trim('\r', 'n'));
        }

        protected virtual string [] SplitHandsLines(string handText)
        {
             return LineSplitRegex.Split(handText.Trim('\r', '\n')).Select(s => s.Trim(' ', '\r', '\n')).Where(l => string.IsNullOrWhiteSpace(l) == false).ToArray();           
        }

        public HandHistorySummary ParseFullHandSummary(string handText, bool rethrowExceptions = false)
        {
            try
            {
                string[] lines = SplitHandsLines(handText);

                if (IsValidHand(lines) == false)
                {
                    throw new InvalidHandException(handText ?? "NULL");
                }                

                return ParseFullHandSummary(lines);
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

        protected HandHistorySummary ParseFullHandSummary(string[] handLines)
        {
            HandHistorySummary handHistorySummary = new HandHistorySummary();
                            
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
            catch (Exception ex)
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
            try
            {
                string[] lines = SplitHandsLines(handText);

                if (IsValidHand(lines) == false)
                {
                    throw new InvalidHandException(handText ?? "NULL");                    
                }

                string[] handLines = SplitHandsLines(handText);
               
                HandHistory handHistory = new HandHistory
                        {
                            DateOfHandUtc = ParseDateUtc(handLines),
                            GameDescription = ParseGameDescriptor(handLines),
                            HandId = ParseHandId(handLines),
                            TableName = ParseTableName(handLines),
                            DealerButtonPosition = ParseDealerPosition(handLines),
                            FullHandHistoryText = string.Join("\r\n", handLines),
                            ComumnityCards = ParseCommunityCards(handLines),                            
                        };

                handHistory.Players = ParsePlayers(handLines);
                handHistory.NumPlayersSeated = handHistory.Players.Count;

                if (handHistory.Players.Count(p => p.IsSittingOut == false) <= 1)
                {
                    throw new PlayersException(handText, "Only found " + handHistory.Players.Count + " players with actions.");
                }

                handHistory.HandActions = ParseHandActions(handLines, handHistory.GameDescription.GameType);

                if (RequiresActionSorting)
                {
                    handHistory.HandActions = OrderHandActions(handHistory.HandActions);
                }
                if (RequresAdjustedRaiseSizes)
                {
                   handHistory.HandActions = AdjustRaiseSizes(handHistory.HandActions);                   
                }
                if (RequiresAllInDetection)
                {
                    handHistory.HandActions = IdentifyAllInActions(handLines, handHistory.HandActions);
                }

                HandAction anteAction = handHistory.HandActions.FirstOrDefault(a => a.HandActionType == HandActionType.ANTE);
                if (anteAction != null)
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

                logger.Warn("Couldn't parse hand {0} with error {1} and trace {2}", handText, ex.Message, ex.StackTrace);
                return null;
            }        
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
            return new GameDescriptor(PokerFormat.CashGame,
                                      SiteName,
                                      ParseGameType(handLines),
                                      ParseLimit(handLines),
                                      ParseTableType(handLines),
                                      ParseSeatType(handLines));
        }

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

        public  bool IsValidHand(string handText)
        {
            return IsValidHand(SplitHandsLines(handText));
        }

        public abstract bool IsValidHand(string[] handLines);

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
                if (RequresAdjustedRaiseSizes)
                {
                    handActions = AdjustRaiseSizes(handActions);
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
                if (action is AllInAction)
                {
                    identifiedActions.Add(action);       
                    continue;
                }

                //Update the remaining stack with our action's amount
                playerStackRemaining[action.PlayerName] += action.Amount;

                if (playerStackRemaining[action.PlayerName] == 0)
                {
                    //This was a bet/raise/call for our remaining chips - we are all in
                    AllInAction allInAction = new AllInAction(action.PlayerName, action.Amount, action.Street, true);
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

        /// <summary>
        /// Re-raise amounts are handled differently on different sites. Consider the
        /// situation where:
        ///          Player1 Bets $10
        ///          Player2 Raises to $30 total (call of $10, raise $20)
        ///          Player1 Raises to $100 total (call of $20, raise $70)
        /// 
        /// Party will display this as: Bet 10, Raise 30, Raise 70
        /// Stars will display this as: Bet 10, Raise to 30, Raise to 100. 
        /// 
        /// In the case for Stars we will need to deduct previous action amounts from the raise to figure out how
        /// much that raise actuall is i.e Player 1 only wagered $90 more.
        /// </summary>
        /// <param name="handActions"></param>
        protected List<HandAction> AdjustRaiseSizes(List<HandAction> handActions)
        {
            var actionsByStreets = handActions.GroupBy(h => h.Street);

            foreach (var actionsByStreet in actionsByStreets)
            {
                Street street = actionsByStreet.Key;
                List<HandAction> actions = actionsByStreet.ToList();

                // loop backward through the actions and subtracting the action prior to each raise
                // from that raise amount
                for (int i = actions.Count - 1; i >= 0; i--)
                {
                    HandAction currentAction = actions[i];
                    AllInAction allInAction = currentAction as AllInAction;

                    if (currentAction.HandActionType != HandActionType.RAISE &&
                        (allInAction == null ||
                        allInAction.IsRaiseAllIn == false))
                    {
                        continue;
                    }

                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (actions[j].PlayerName.Equals(currentAction.PlayerName))
                        {
                            // Ante's don't get counted in the raise action lines
                            if (actions[j].HandActionType == HandActionType.ANTE)
                            {
                                continue;                                
                            }

                            // If the player previously called any future raise will be the entire amount
                            if (actions[j].HandActionType == HandActionType.CALL)
                            {
                                currentAction.DecreaseAmount(actions[j].Amount);
                                continue;                                
                            }

                            currentAction.DecreaseAmount(actions[j].Amount);
                            break;
                        }
                    }
                    
                }
            }

            return handActions;
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
    }
}
