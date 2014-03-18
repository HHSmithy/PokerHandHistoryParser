using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Compression;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.PartyPoker;

namespace HandHistories.Parser.Parsers.RegexParser.Base
{
    public abstract class HandHistoryRegexParserImplBase : IHandHistoryParser
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public abstract SiteName SiteName { get; }

        public abstract string TableNameRegex { get; }
        public abstract string TableSizeRegex { get; }

        public abstract string GameTypeRegex { get; }
        public abstract string GameNumberRegex { get; }

        public abstract string GameLimitRegex { get; }
        public abstract string GameLimitRegexWithSlash { get; }

        public abstract string GameDateRegex { get; }
        public abstract string DateYearRegex { get; }
        public abstract string DateMonthRegex { get; }
        public abstract string DateDayRegex { get; }
        public abstract string DateTimeRegex { get; }
                
        public abstract string ButtonLocationRegex { get; }

        public abstract string SeatInfoRegex { get; }
        public abstract string SeatInfoPlayerNameRegex { get; }
        public abstract string SeatInfoStartingStackRegex { get; }
        public abstract string SeatInfoSeatNumberRegex { get; }

        public abstract string BoardRegexFlop { get; }
        public abstract string BoardRegexTurn { get; }
        public abstract string BoardRegexRiver { get; }

        public abstract string GetHoleCardsRegex(string playerName);

        public virtual string PlayerNameRegex { get { return @"[\w _\.\-\$]+"; } }
        public virtual string CardRegex { get { return @"[2-9TJQKA][cdsh]"; } }

        protected HandHistoryRegexParserImplBase()
        {

        }

        public abstract IEnumerable<string> SplitUpMultipleHands(string rawHandHistories);

        public HandHistorySummary ParseFullHandSummary(string handText, bool rethrowExceptions = false)
        {
            try
            {
                if (IsValidHand(handText) == false)
                {
                    throw new InvalidHandException(handText ?? "NULL");
                }

                HandHistorySummary handHistorySummary = new HandHistorySummary()
                                                            {
                                                                DateOfHandUtc = ParseDateUtc(handText),
                                                                GameDescription = ParseGameDescriptor(handText),
                                                                HandId = ParseHandId(handText),
                                                                TableName = ParseTableName(handText),
                                                                NumPlayersSeated = ParseNumPlayers(handText),
                                                                DealerButtonPosition = ParseDealerPosition(handText)
                                                            };

                handHistorySummary.FullHandHistoryText = handText.Replace("\r", "").Replace("\n", "\r\n");

                return handHistorySummary;
            }
            catch (Exception ex)
            {
                if (rethrowExceptions)
                {
                    throw;
                }

                logger.Warn("Couldn't parse hand {0} with error {1}", handText, ex.Message);
                return null;
            }            
        }

        public HandHistory ParseFullHandHistory(string handText, bool rethrowExceptions = false)
        {
            try
            {                
                if (IsValidHand(handText) == false)
                {
                    throw new InvalidHandException(handText ?? "NULL");
                }

                HandHistorySummary handHistorySummary = ParseFullHandSummary(handText, rethrowExceptions);

                HandHistory handHistory = new HandHistory()
                                              {
                                                  DateOfHandUtc = handHistorySummary.DateOfHandUtc,
                                                  GameDescription = handHistorySummary.GameDescription,
                                                  HandId = handHistorySummary.HandId,
                                                  TableName = handHistorySummary.TableName,
                                                  NumPlayersSeated = handHistorySummary.NumPlayersSeated,
                                                  DealerButtonPosition = handHistorySummary.DealerButtonPosition
                                              };
              
                handHistory.FullHandHistoryText = handText.Replace("\r", "").Replace("\n", "\r\n");

                handHistory.ComumnityCards = ParseCommunityCards(handText);

                List<HandAction> handActions;
                handHistory.Players = ParsePlayers(handText, out handActions);
                handHistory.HandActions = handActions;

                handHistory.Players = SetSittingOutPlayers(handHistory.Players, handHistory.HandActions);                

                if (handHistory.Players.Count <= 1)
                {
                    throw new PlayersException(handText, "Only found " + handHistory.Players.Count + " players with actions.");
                }       

                return handHistory;
            }
            catch (Exception ex)
            {
                if (rethrowExceptions)
                {
                    throw;
                }

                logger.Warn("Couldn't parse hand {0} with error {1}", handText, ex.Message);
                return null;
            }            
        }

        private PlayerList SetSittingOutPlayers(PlayerList players, List<HandAction> handActions)
        {
            for (int i = players.Count - 1; i >= 0; i--)
            {
                if (handActions.Any(a => a.PlayerName.Equals(players[i].PlayerName)) == false)
                {
                    players[i].IsSittingOut = true;
                }
            }

            return players;
        }

        public abstract bool IsValidHand(string handText);

        public abstract bool IsValidOrCancelledHand(string handText, out bool isCancelled);

        public long ParseHandId(string handText)
        {
            try
            {
                string gameNumber = Regex.Match(handText, GameNumberRegex).Value;
                return long.Parse(gameNumber);
            }
            catch (Exception exception)
            {
                throw new HandActionException(handText, "ParseGameNumber: " + exception.Message);
            }
        }

        public string ParseTableName(string handText)
        {
            try
            {
                var tableName = Regex.Match(handText, TableNameRegex).Value;
                return tableName;
            }
            catch (Exception exception)
            {
                throw new TableNameException(handText, "ParseTableName: " + exception.Message);
            }     
        }

        public GameDescriptor ParseGameDescriptor(string handText)
        {            
            return new GameDescriptor()
                       {
                           GameType = ParseGameType(handText),
                           Limit = ParseLimit(handText),
                           SeatType = ParseSeatType(handText),
                           Site = SiteName,
                           TableType = ParseTableType(handText),
                           PokerFormat = PokerFormat.CashGame
                       };
        }

        public DateTime ParseDateUtc(string handText)
        {
            try
            {
                var dateString = Regex.Match(handText, GameDateRegex).Value;
                DateTime dateTime;

                string month = Regex.Match(dateString, DateMonthRegex).Value;
                string day = Regex.Match(dateString, DateDayRegex).Value;
                string time = Regex.Match(dateString, DateTimeRegex).Value;
                string year = Regex.Match(dateString, DateYearRegex).Value;
                string dateFormatString = string.Format("{0} {1}, {2} {3}", month, day, year, time);

                bool result = DateTime.TryParse(dateFormatString, out dateTime);
                if (result)
                {
                    return ConvertHandDateToUtc(dateTime);
                }

                throw new Exception("Could not parse date.");
            }
            catch (Exception exception)
            {
                throw new ParseHandDateException(handText, "ParseDateUtc: " + exception.Message);
            }     
        }

        protected DateTime ConvertHandDateToUtc(DateTime handDate)
        {
            DateTime converted = TimeZoneInfo.ConvertTimeToUtc(handDate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            return DateTime.SpecifyKind(converted, DateTimeKind.Utc);
        }

        public SeatType ParseSeatType(string handText)
        {
            try
            {
                int tableSize = Int32.Parse(Regex.Match(handText, TableSizeRegex).Value);
                return SeatType.FromMaxPlayers(tableSize);
            }
            catch (Exception exception)
            {
                throw new SeatTypeException(handText, "ParseSeatType: " + exception.Message);
            }  
        }

        public GameType ParseGameType(string handText)
        {
            try
            {
                var gameType = Regex.Match(handText, GameTypeRegex).Value;

                if (gameType.Equals("NL Texas Hold'em")) return GameType.NoLimitHoldem;
                else if (gameType.Equals("Texas Hold'em")) return GameType.FixedLimitHoldem;
                else if (gameType.Equals("FL Texas Hold'em")) return GameType.FixedLimitHoldem;
                else if (gameType.Equals("PL Omaha")) return GameType.PotLimitOmaha;
                else if (gameType.Equals("PL Omaha Hi-Lo")) return GameType.PotLimitOmahaHiLo;
                else if (gameType.Equals("PL Texas Hold'em")) return GameType.PotLimitHoldem;
                
                throw new UnrecognizedGameTypeException(handText, "Game type not recognized: " + gameType);
            }
            catch (Exception exception)
            {
                throw;
            }     
        }

        public TableType ParseTableType(string handText)
        {
            string tableName = ParseTableName(handText);

            if (tableName.StartsWith("Super Speed"))
                return TableType.FromTableTypeDescriptions(TableTypeDescription.SuperSpeed);
            if (tableName.StartsWith("Speed"))
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Speed);
            if (tableName.StartsWith("Jackpot"))
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Jackpot);
            if (tableName.StartsWith("20BB Min Speed"))
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Speed, TableTypeDescription.Shallow);
            if (tableName.StartsWith("20BB Min"))
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Shallow);
            if (tableName.StartsWith("Deep"))
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Deep);
            if (tableName.StartsWith("Heads Up Anonymous"))
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Anonymous);
            if (tableName.StartsWith("Table "))
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
            if (tableName.StartsWith("Heads Up "))
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);

            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        public abstract Limit ParseLimit(string handText);
        
        public int ParseNumPlayers(string handText)
        {
            return ParsePlayers(handText).Count;
        }
       
        protected virtual Currency ParseCurrency(string handText)
        {
            // todo this is a hack and slow, refactor!
            if (handText.IndexOf(@"$") != -1)
            {
                return Currency.USD;
            }
            if (handText.IndexOf("€") != -1)
            {
                return Currency.EURO;
            }
            if (handText.IndexOf(@"£") != -1)
            {
                return Currency.GBP;
            }

            throw new LimitException(handText, "ParseCurrency: No currency found");
        }        

        public int ParseDealerPosition(string handText)
        {
            try
            {
                var buttonPosition = Regex.Match(handText, ButtonLocationRegex).Value;
                return Int32.Parse(buttonPosition);
            }
            catch (Exception exception)
            {
                throw new PlayersException(handText, "ParseButtonPosition: " + exception.Message);
            }    
        }

        public List<HandAction> ParseHandActions(string handText)
        {
            PlayerList players = ParsePlayers(handText);

            return new PartyActionRegexParserImpl().ParseActions(handText, players);
        }

        protected List<HandAction> ParseHandActions(string handText, PlayerList playerList)
        {
            return new PartyActionRegexParserImpl().ParseActions(handText, playerList);
        }

        public PlayerList ParsePlayers(string handText)
        {
            List<HandAction> handActions;
            return ParsePlayers(handText, out handActions);
        }

        protected PlayerList ParsePlayers(string handText, out List<HandAction> handActions)
        {
            try
            {
                var seat = Regex.Match(handText, SeatInfoRegex);

                if (!seat.Success)
                {
                    throw new PlayersException( handText, "ParsePlayerNames: Couldn't find any seating info!");
                }

                PlayerList players = new PlayerList();

                do
                {
                    decimal startingStack = decimal.Parse(Regex.Match(seat.Value, SeatInfoStartingStackRegex).Value, System.Globalization.CultureInfo.InvariantCulture);
                    int position = Int32.Parse(Regex.Match(seat.Value, SeatInfoSeatNumberRegex).Value);
                    string screenName = Regex.Match(seat.Value, SeatInfoPlayerNameRegex).Value;

                    players.Add(new Player(screenName, startingStack, position));

                } while ((seat = seat.NextMatch()).Success);

                foreach (var player in players)
                {
                    player.HoleCards = ParseHoleCards(handText, player.PlayerName);
                }

                handActions = ParseHandActions(handText, players);

                SetSittingOutPlayers(players, handActions);

                return players;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public BoardCards ParseCommunityCards(string handText)
        {
            try
            {
                string flop = Regex.Match(handText, BoardRegexFlop).Value;
                string turn = Regex.Match(handText, BoardRegexTurn).Value;
                string river = Regex.Match(handText, BoardRegexRiver).Value;

                string combined = flop + turn + river;

                return ParseAllCardsFromText(combined);
            }
            catch (Exception ex)
            {
                throw new CardException(handText, "ParseBoardCards: " + ex.Message);
            }
        }

        protected BoardCards ParseAllCardsFromText(string text)
        {
            List<Card> commCards = new List<Card>();

            var cards = Regex.Match(text, CardRegex);

            if (!cards.Success) return BoardCards.ForPreflop();

            do
            {
                commCards.Add(Card.Parse(cards.Value));
                cards = cards.NextMatch();
            } while (cards.Success);

            return BoardCards.FromCards(commCards.ToArray());
        }

        private HoleCards ParseHoleCards(string handText, string playerName)
        {
            try
            {
                string regexString = GetHoleCardsRegex(playerName);
                var regex = Regex.Match(handText, regexString);
                if (regex.Success)
                {
                    return HoleCards.FromCards(playerName, CardGroup.Parse(regex.Value));
                }
                else
                {
                    return HoleCards.NoHolecards(playerName);
                }
            }
            catch (Exception ex)
            {
                throw new CardException(handText, "ParseHoleCards: Exception " + ex.Message);
            }
        }       
    }
}
