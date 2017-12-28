using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.FastParsing;
using HandHistories.Parser.Utils.Pot;
using HandHistories.Parser.Utils.Uncalled;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HandHistories.Parser.Parsers.FastParser.PartyPoker
{
    public sealed partial class PartyPokerFastParserImpl : HandHistoryParserFastImpl
    {
        private readonly SiteName _siteName;

        private static readonly NumberFormatInfo NumberFormatInfo = new NumberFormatInfo
            {
                NegativeSign = "-",
                CurrencyDecimalSeparator = ".",
                CurrencyGroupSeparator = ",",
            };
        private readonly Currency _currency;

        public override SiteName SiteName
        {
            get { return _siteName; }
        }

        public override bool RequiresUncalledBetFix
        {
            get { return true; }
        }

        public override bool RequiresUncalledBetWinAdjustment
        {
            get { return true; }
        }

        public override bool RequiresTotalPotCalculation
        {
            get { return true; }
        }

        public override bool RequiresAllInUpdates
        {
            get { return true; }
        }

        // So the same parser can be used for It and Fr variations
        public PartyPokerFastParserImpl(SiteName siteName = SiteName.PartyPoker)
        {
            _siteName = siteName;
        }

        protected override string[] SplitHandsLines(string handText)
        {
            return base.SplitHandsLines(handText)
                .TakeWhile(p => !p.StartsWithFast("Game #") && !p.EndsWithFast(" starts."))
                .ToArray();
        }

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return SplitUpMultipleHandsToLines(rawHandHistories).Select(p => string.Join("\r\n", p));
        }

        public override IEnumerable<string[]> SplitUpMultipleHandsToLines(string rawHandHistories)
        {
            var allLines = rawHandHistories.LazyStringSplitFastSkip('\n', jump: 10, jumpAfter: 2);

            List<string> handLines = new List<string>(50);

            bool validHand = false;

            foreach (var item in allLines)
            {
                if (!validHand)
                {
                    if (!item.StartsWithFast("***** Hand History"))
                    {
                        continue;
                    }
                    validHand = true;
                }

                string line = item.TrimEnd('\r', ' ');

                if (string.IsNullOrWhiteSpace(line) || 
                    (line.StartsWithFast("Game #") && line.EndsWithFast(" starts.")))
                {
                    if (handLines.Count > 0)
                    {
                        yield return handLines.ToArray();
                        handLines = new List<string>(50);
                    }
                    continue;
                }
                handLines.Add(line);
            }

            if (handLines.Count > 0)
            {
                yield return handLines.ToArray();
            }
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            // Expect the 6th line to look like this:
            // "Seat 4 is the button"

            const int startIndex = 5;
            return FastInt.Parse(handLines[3], startIndex);
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            // Expect the second line to look like this: 
            // "$600 USD PL Omaha - Thursday, September 25, 01:10:46 EDT 2014"
            string line = handLines[1];

            int splitIndex = line.IndexOfFast(" - ") + 3;

            int monthStartIndex = line.IndexOf(',', splitIndex) + 2;
            int monthEndIndex = line.IndexOf(' ', monthStartIndex);
            string month = line.Substring(monthStartIndex, monthEndIndex - monthStartIndex);
            
            int dayStartIndex = monthEndIndex + 1;
            int dayEndIndex = line.IndexOf(',', dayStartIndex);
            string dayStr = line.Substring(dayStartIndex, dayEndIndex - dayStartIndex);
            int day = int.Parse(dayStr);

            int timeStartIndex = dayEndIndex + 2;
            int timeEndIndex = line.IndexOf(' ', timeStartIndex);
            string timeStr = line.Substring(timeStartIndex, timeEndIndex - timeStartIndex);
            

            int yearIndex = line.LastIndexOf(' ');
            string Year = line.Substring(yearIndex);
            int year = int.Parse(Year);

            
            TimeSpan time = TimeSpan.Parse(timeStr, CultureInfo.InvariantCulture);

            DateTime result = new DateTime(year, GetMonthNumber(month), day, time.Hours, time.Minutes, time.Seconds);
            return ConvertHandDateToUtc(result);
        }

        static DateTime ConvertHandDateToUtc(DateTime handDate)
        {
            DateTime converted = TimeZoneInfo.ConvertTimeToUtc(handDate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            return DateTime.SpecifyKind(converted, DateTimeKind.Utc);
        }

        static int GetMonthNumber(string month)
        {
            switch (month)
            {
                case "January":
                    return 1;
                case "February":
                    return 2;
                case "March":
                    return 3;
                case "April":
                    return 4;
                case "May":
                    return 5;
                case "June":
                    return 6;
                case "July":
                    return 7;
                case "August":
                    return 8;
                case "September":
                    return 9;
                case "October":
                    return 10;
                case "November":
                    return 11;
                case "December":
                    return 12;
                default:
                    throw new ArgumentException("Month: " + month);
            }
        }

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            string line = handLines[1];

            if (line.Contains("Buy-in Trny:"))
            {
                if (handLines[2].Contains("Table #"))
                {
                    return PokerFormat.MultiTableTournament;
                }
                else
                {
                    return PokerFormat.SitAndGo;
                }
            }

            return PokerFormat.CashGame;
        }

        protected override long ParseHandId(string[] handLines)
        {
            // Expect the first line to look like this: 
            // "***** Hand History for Game 13550493674 *****"
            const int firstDigitIndex = 28;  //= "***** Hand History for Game ".Length

            string line = handLines[0];
            int lastDigitIndex = line.IndexOf(' ', firstDigitIndex);

            string handId = handLines[0].Substring(firstDigitIndex, lastDigitIndex - firstDigitIndex);
            return long.Parse(handId);
        }

        protected override string ParseTableName(string[] handLines)
        {
            // Line 3 is in form:
            // "Table Houston (Real Money)"
            string line = handLines[2];

            const int startIndex = 6;
            int endIndex = line.LastIndexOfFast(" (");
            return line.Substring(startIndex, endIndex - startIndex);
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            // line 5 looks like :
            // "Total number of players : 2/6 "
            int maxPlayerIndex = handLines[4].LastIndexOf('/') + 1;

            // 2-max, 6-max or 9-max
            int maxPlayers = FastInt.Parse(handLines[4][maxPlayerIndex]);

            // can't have 1max so must be 10max
            if (maxPlayers == 1)
            {
                maxPlayers = 10;
            }

            return SeatType.FromMaxPlayers(maxPlayers);
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            PokerFormat format = ParsePokerFormat(handLines);

            if (format == PokerFormat.CashGame)
            {
                return ParseCashGameGametype(handLines);
            }
            else
            {
                return ParseGametypeTournament(handLines);
            }
        }

        private static GameType ParseCashGameGametype(string[] handLines)
        {
            // Expect the fourth line to look like this: 
            // "$600 USD PL Omaha - Thursday, September 25, 01:10:46 EDT 2014"
            string line = handLines[1];

            // can be either 1 or 2 spaces after the colon
            int startIndex = line.IndexOf(' ');
            startIndex = line.IndexOf(' ', startIndex + 1) + 1;
            int endIndex = line.IndexOf(" -", startIndex);

            int gameTypeLength = endIndex - startIndex;
            int gameLength = gameTypeLength - 3;
            string limit = line.Substring(startIndex, 2);

            switch (limit)
            {
                case "FL":
                    switch (gameLength)
                    {
                        case 5://"Omaha".Length
                            return GameType.FixedLimitOmaha;
                        case 13://"Texas Hold'em".Length
                            return GameType.FixedLimitHoldem;
                        case 11://"Omaha Hi-Lo".Length
                            return GameType.FixedLimitOmahaHiLo;
                    }
                    break;
                case "NL":
                    switch (gameLength)
                    {
                        case 5://"Omaha".Length
                            return GameType.NoLimitOmaha;
                        case 13://"Texas Hold'em".Length
                            return GameType.NoLimitHoldem;
                        case 11://"Omaha Hi-Lo".Length
                            return GameType.NoLimitOmahaHiLo;
                    }
                    break;
                case "PL":
                    switch (gameLength)
                    {
                        case 5://"Omaha".Length
                            return GameType.PotLimitOmaha;
                        case 13://"Texas Hold'em".Length
                            return GameType.PotLimitHoldem;
                        case 11://"Omaha Hi-Lo".Length
                            return GameType.PotLimitOmahaHiLo;
                    }
                    break;
            }

            throw new UnrecognizedGameTypeException(handLines[0], "Unrecognized game-type: " + line.Substring(startIndex, endIndex - startIndex));
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            var format = ParsePokerFormat(handLines);
            if (format == PokerFormat.CashGame)
            {
                return ParseCashgameLimit(handLines);
            }
            else
            {
                return ParseTournamentLimit(handLines);
            }            
        }

        private Limit ParseCashgameLimit(string[] handLines)
        {
            // The different formats for the stakes:
            // "$5/$10 USD FL Texas Hold'em"
            // "$600 USD PL Omaha"

            string line = handLines[1];
            int endIndex = line.IndexOf(' ');

            string limitSubstring = line.Substring(1, endIndex - 1);

            Currency currency = ParseLimitCurrency(line);

            // If there is a game limit with a slash then the limit is in form $2/$4
            // then convert the game limit into a game type without a slash which would be 400 for 2/4
            if (limitSubstring.Contains("/"))
            {
                return ParseNormalLimit(limitSubstring, currency);
            }

            string tableName = ParseTableName(handLines);
            GameType game = ParseGameType(handLines);

            if (game == GameType.FixedLimitHoldem)
            {
                decimal limitBB = limitSubstring.ParseAmount();
                return Limit.FromSmallBlindBigBlind(limitBB / 2.0m, limitBB, currency);
            }

            // Handle 20BB tables, due to Party putting the limit up as 40% of the actual
            // limit. So for instance 20BB party $100NL the limit is displayed as $40NL.
            // No idea why this is so.    
            if (tableName.StartsWithFast("20BB"))
            {
                return Parse20BBLimit(limitSubstring, currency);
            }

            return ParseBuyInLimit(limitSubstring, currency);
        }

        static Limit Parse20BBLimit(string limitSubstring, Currency currency)
        {
            decimal bigblind = limitSubstring.ParseAmount() * 0.4m;

            return Limit.FromSmallBlindBigBlind(bigblind / 2, bigblind, currency);
        }

        static Currency ParseLimitCurrency(string line)
        {
            Currency currency;
            char currencySymbol = line[0];

            switch (currencySymbol)
            {
                case '$':
                    currency = Currency.USD;
                    break;
                case '€':
                    currency = Currency.EURO;
                    break;
                case '£':
                    currency = Currency.GBP;
                    break;

                case 'N':
                case 'P':
                case 'F':
                    return Currency.CHIPS;

                default:
                    throw new LimitException(line, "Unrecognized currency symbol " + currencySymbol);
            }
            return currency;
        }

        static Limit ParseBuyInLimit(string limitSubstring, Currency currency)
        {
            decimal buyIn = limitSubstring.ParseAmount();
            decimal bigBlind = buyIn / 100.0m;

            if (bigBlind == 0.25m)
            {
                return Limit.FromSmallBlindBigBlind(0.10m, 0.25m, currency);
            }

            return Limit.FromSmallBlindBigBlind(bigBlind / 2.0m, bigBlind, currency);
        }

        static Limit ParseNormalLimit(string limitSubstring, Currency currency)
        {
            //Expected limitSubstring format:
            //0.05/$0.10
            int splitIndex = limitSubstring.IndexOf('/');
            string SB = limitSubstring.Remove(splitIndex);
            string BB = limitSubstring.Substring(splitIndex + 2);

            decimal small = SB.ParseAmount();
            decimal big = BB.ParseAmount();
            return Limit.FromSmallBlindBigBlind(small, big, currency);
        }

        public override bool IsValidHand(string[] handLines)
        {
            bool isCancelled; // in this case eat it
            return IsValidOrCancelledHand(handLines, out isCancelled);
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            //Expected one of the last lines to look like:
            //"Player wins $102 USD from the main pot with a flush, Ace high."
            
            isCancelled = false;

            for (int i = handLines.Length - 1; i >= handLines.Length - 10; i--)
            {
                // if the line starts with ** we can definitely leave the loop
                if (handLines[i][0] == '*' && handLines[i][1] == '*')
                    break;

                if (handLines[i].Contains(" wins "))
                    return true;
            }
            return false;
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType)
        {
            // actions take place from the last seat info until the *** SUMMARY *** line            

            int actionIndex = GetFirstActionIndex(handLines);

            List<HandAction> handActions = new List<HandAction>(handLines.Length - actionIndex);

            actionIndex = ParseBlindActions(handLines, ref handActions, actionIndex);

            Street currentStreet = Street.Preflop;

            for (int lineNumber = actionIndex; lineNumber < handLines.Length; lineNumber++)
            {
                string line = handLines[lineNumber];

                if (IsConnectionLost(line))
                {
                    throw new InvalidHandException(string.Join(Environment.NewLine, handLines), "Lost connection during HH saving, unable to parse file");
                }

                try
                {
                    var action = ParseRegularActionLine(line, ref currentStreet);

                    if (action != null)
                    {
                        handActions.Add(action);
                    }
                }
                catch (Exception ex)
                {
                    // in some cases chatlines don't have playername involved, so we ignore every parsing error where no playername is involved 
                    // AND the line does not start with a star
                    // !!! THIS SHOULD BE DISABLED FOR DEBUGGING PURPOSES !!!
                    var players = ParsePlayers(handLines);
                    if (!players.Select(p => p.PlayerName).Any(line.Contains) && line[0] != '*')
                        continue;

                    // skip emoticon lines like
                    // zehnbube - no skill
                    // zehnbube - donkey fish
                    if (players.Select(p => p.PlayerName + " -").Any(line.Contains))
                        continue;

                    // if it's the last line, there can be some weird chat lines that we can ignore
                    if (lineNumber == handLines.Length - 1)
                        continue;

                    throw new HandActionException(line, "Couldn't parse line '" + line + " with ex: " + ex.Message);
                }
            }

            return handActions;
        }

        static bool IsConnectionLost(string line)
        {
            // if we find the following line in the HH, it is corrupt in most cases
            // Connection Lost due to some reason
            // ^         ^^                    ^^
            if (line[0] == 'C' 
                && line[line.Length - 1] == 'n' && line[line.Length -2] == 'o'
                && line[10] == ' ' && line[11] == 'L')
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses a handaction or changes the current street
        /// </summary>
        /// <param name="line"></param>
        /// <param name="currentStreet"></param>
        /// <param name="handActions"></param>
        /// <returns>True if we have reached the end of the action block.</returns>
        public static HandAction ParseRegularActionLine(string line, ref Street currentStreet)
        {
            //Chat lines start with the PlayerName and
            //PlayerNames can contain characters that disturb parsing
            //So we must check for chatlines first
            if (isChatLine(line))
            {
                return null;
            }

            char lastChar = line[line.Length - 1];
            switch (lastChar)
            {
                //Expected formats:
                //player posts small blind [$5 USD].
                //player posts big blind [$10 USD].
                case '.':
                    return ParseDotAction(line, currentStreet);

                case ']':
                    char firstChar = line[0];
                    if (firstChar == '*')
                    {
                        currentStreet = ParseStreet(line);
                        return null;
                    }
                    else if (line.StartsWithFast("Dealt to"))
                    {
                        return null;
                    }
                    else
                    {
                        return ParseActionWithSize(line, currentStreet);
                    }

                case 's':
                    //saboniiplz wins 28,304 chips
                    if (line[line.Length - 2] == 'p')
                    {
                        return ParseWinsAction(line);
                    }
                    else
                    {
                        return ParseActionWithoutSize(line, currentStreet);
                    }
                    
                //Expected Formats:
                //"Player wins $5.18 USD"
                case 'D':
                    return ParseWinsAction(line);

                default: return null;
            }
        }

        static bool isChatLine(string line)
        {
            return line.Contains(": ");
        }

        static HandAction ParseWinsAction(string line, int potID = 0)
        {
            //Expected Formats:
            //"Player wins $5.18 USD"
            //Tournament format:
            //saboniiplz wins 28,304 chips
            int nameEndIndex = line.IndexOfFast(" wins ");
            string playerName = line.Remove(nameEndIndex);

            int amountStartIndex = nameEndIndex + 6;
            decimal amount = ParseDecimal(line, amountStartIndex);

            if (potID != 0)
            {
                return new WinningsAction(playerName, HandActionType.WINS_SIDE_POT, amount, potID);
            }
            return new WinningsAction(playerName, HandActionType.WINS, amount, potID);
        }

        static HandAction ParseActionWithSize(string line, Street currentStreet)
        {
            const int raiseLength = 8;//" raises ".Length
            const int callsLength = 7;//" calls ".Length
            const int betsLength = 6;//" bets ".Length
            const int allInLength = 12;//" is all-In  ".Length

            int amountStartIndex = line.LastIndexOf('[');
            decimal amount = ParseDecimal(line, amountStartIndex + 1);

            char actionID = line[amountStartIndex - 3];
            string playerName;
            switch (actionID)
            {
                //Player bets [$23.75 USD]
                case 't':
                    playerName = line.Remove(amountStartIndex - betsLength);
                    return new HandAction(playerName, HandActionType.BET, amount, currentStreet);
                //Player calls [$25 USD]
                case 'l':
                    playerName = line.Remove(amountStartIndex - callsLength);
                    return new HandAction(playerName, HandActionType.CALL, amount, currentStreet);
                //Player raises [$30 USD]
                case 'e':
                    playerName = line.Remove(amountStartIndex - raiseLength);
                    return new HandAction(playerName, HandActionType.RAISE, amount, currentStreet);
                //"dr. spaz is all-In  [$4.90 USD]"
                case 'n':
                    playerName = line.Remove(amountStartIndex - allInLength);
                    return new HandAction(playerName, HandActionType.ALL_IN, amount, currentStreet);
                default:
                    throw new ArgumentException("Unknown actionID: " + line);
            }
        }

        static Street ParseStreet(string line)
        {
            const int identifierIndex = 11;
            char streetIDChar = line[identifierIndex];
            switch (streetIDChar)
            {
                case 'F':
                    return Street.Flop;
                case 'T':
                    return Street.Turn;
                case 'R':
                    return Street.River;
                default:
                    throw new ArgumentException("Unknown streetID: " + streetIDChar);
            }
        }

        static HandAction ParseDotAction(string line, Street street)
        {
            //const int smallBlindWidth = 19;//" posts small blind ".Length
            //const int bigBlindWidth = 17;//" posts big blind ".Length
            //const int deadBigBlindWidth = 24;//" posts big blind + dead ".Length

            string playerName;
            int playerNameIndex = 0;

            char lastChar = line[line.Length - 2];
            if (lastChar == ']')
            {
                throw new ArgumentException("Blinds must be parsed with ParseBlindAction(string)");
                //int amountStartIndex = line.LastIndexOf('[');
                //decimal amount = ParseDecimal(line, amountStartIndex + 2);

                //char blindIdentifier = line[amountStartIndex - 8];

                //switch (blindIdentifier)
                //{
                //    //"Player posts big blind [$10 USD]."
                //    case 'g':
                //        playerName = line.Remove(amountStartIndex - bigBlindWidth);
                //        return new HandAction(playerName, HandActionType.BIG_BLIND, amount, street);

                //    //"Player posts small blind [$5 USD]."
                //    case 'l':
                //        playerName = line.Remove(amountStartIndex - smallBlindWidth);
                //        return new HandAction(playerName, HandActionType.SMALL_BLIND, amount, street);

                //    //"Player posts big blind + dead [$0.15].
                //    case ' ':
                //        playerName = line.Remove(amountStartIndex - deadBigBlindWidth);
                //        return new HandAction(playerName, HandActionType.POSTS, amount, street);
                //    default:
                //        throw new ArgumentException("Unkown posting Action: " + line);
                //}
            }
            else if (line.Contains(" shows"))
            {
                if (isWinType(line, " shows [", ref playerNameIndex))
                {
                    playerName = line.Remove(playerNameIndex);
                    return new HandAction(playerName, HandActionType.SHOW, 0m, Street.Showdown);
                }
                else if (line.Contains(" for low."))
                {
                    playerNameIndex = line.IndexOfFast(" shows");
                    playerName = line.Remove(playerNameIndex);
                    return new HandAction(playerName, HandActionType.SHOWS_FOR_LOW, 0m, Street.Showdown);
                }
                else
                {
                    throw new ArgumentException("Unknown Showdown: " + line);
                }
            }
            else if (line.Contains(" wins "))
            {
                playerName = line.Remove(playerNameIndex);
                int amountStartIndex = line.LastIndexOf('[');
                if (isWinType(line, " wins [", ref playerNameIndex))
                {
                    decimal amount = ParseDecimal(line, amountStartIndex + 1);
                    return new WinningsAction(playerName, HandActionType.WINS, amount, 0);
                }

                if (isWinType(line, " wins Lo (", ref playerNameIndex))
                {
                    amountStartIndex = playerNameIndex + " wins Lo (".Length + 1;
                    decimal amount = ParseDecimal(line, amountStartIndex);

                    playerName = line.Remove(playerNameIndex);
                    return new WinningsAction(playerName, HandActionType.WINS_THE_LOW, amount, 0);
                }
                if (amountStartIndex == -1)//Wins Side Pot
                {
                    string sidePotID = " from the side pot ";
                    int idStartIndex = line.IndexOf(sidePotID);
                    if (idStartIndex != -1)
                    {
                        int idEndIndex = line.IndexOf(' ', idStartIndex + sidePotID.Length);
                        string idStr = line.Substring(idStartIndex + sidePotID.Length, idEndIndex - idStartIndex - sidePotID.Length);
                        int id = int.Parse(idStr);
                        return ParseWinsAction(line, id);
                    }
                    else
                    {
                        return ParseWinsAction(line);
                    }
                }
            }
            else if (line.Contains(" does not "))
            {
                playerNameIndex = line.IndexOfFast(" does not ");
                playerName = line.Remove(playerNameIndex);
                return new HandAction(playerName, HandActionType.MUCKS, 0m, Street.Showdown);
            }
            else if (line.Contains(" doesn't show"))
            {
                playerNameIndex = line.IndexOfFast(" doesn't show");
                playerName = line.Remove(playerNameIndex);
                return new HandAction(playerName, HandActionType.SHOW, 0m, Street.Showdown);
            }

            return null;
        }

        static bool isWinType(string line, string type, ref int index)
        {
            int result = line.IndexOf(type, index);
            if (result != -1)
            {
                index = result;
            }
            return result != -1;
        }

        static decimal ParseDecimal(string line, int startIndex)
        {
            int endIndex = line.IndexOf(' ', startIndex);
            if (endIndex == -1)
                endIndex = line.IndexOf(']', startIndex);

            string text = line.Substring(startIndex, endIndex - startIndex);
            return text.ParseAmount();
        }

        static HandAction ParseActionWithoutSize(string line, Street street)
        {
            //Expected formats:
            //"Player checks"
            //"Player folds"
            char identifier = line[line.Length - 2];
            switch (identifier)
            {
                case 'd':
                    return new HandAction(ParseActionPlayerName(line, 6), HandActionType.FOLD, 0m, street);
                case 'k':
                    return new HandAction(ParseActionPlayerName(line, 7), HandActionType.CHECK, 0m, street);
                default:
                    throw new ArgumentException("Unknown Action: \"" + line + "\"");
            }
        }

        static string ParseActionPlayerName(string line, int rightOffset)
        {
            return line.Remove(line.Length - rightOffset);
        }

        static int GetFirstActionIndex(string[] handLines)
        {
            const int FirstSeatIndex = 5;
            for (int lineNumber = FirstSeatIndex; lineNumber < handLines.Length; lineNumber++)
            {
                //Seat 8: Zockermicha ($1613.51 in chips) 
                //BoomDoon: posts small blind $5
                string line = handLines[lineNumber];
                if (line[0] != 'S' || line[line.Length - 1] != ')')
                {
                    return lineNumber;
                }
            }

            throw new HandActionException(string.Empty, "GetFirstActionIndex: Couldn't find it.");
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            PlayerList playerList = new PlayerList();

            int lastLineRead = -1;

            // We start on line index 5 as first 5 lines are table and limit info.
            const int playerListStart = 5;
            for (int lineNumber = playerListStart; lineNumber < handLines.Length; lineNumber++)
            {
                string line = handLines[lineNumber];

                char lastChar = line[line.Length - 1];

                // leave the loop if we spot a summary/hand start of line
                if (line.StartsWithFast("** "))
                {
                    lastLineRead = lineNumber;
                    break;
                }

                // players can also seat at the table after the seat info, post a big blind and be immediately involved in the hand
                const int seatNumberStartIndex = 5;
                
                int playerNameStartIndex = line.IndexOf(':', seatNumberStartIndex) + 2;

                // seat info expected in format: 
                // Seat 4: thaiJhonny ( $1,404 USD )
                if (playerNameStartIndex > 1 && lastChar == ')' && line.StartsWithFast("Seat "))
                {
                    int seatNumber = FastInt.Parse(line, seatNumberStartIndex);
                 
                    // we need to find the ( before the number. players can have ( in their name so we need to go backwards and skip the last one
                    int openParenIndex = line.LastIndexOf('(');

                    string playerName = line.Substring(playerNameStartIndex, openParenIndex - playerNameStartIndex - 1);
                    decimal stack = ParseDecimal(line, openParenIndex + 2);

                    playerList.Add(new Player(playerName, stack, seatNumber));
                }

                // post blind
                // kpark1996 posts big blind [$1 USD].
                else if (lastChar == '.')
                {
                    // they don't have a known seatNumber
                    int seatNumber = -1;
                    decimal stack = 999999.00m; // we make the stacksize very high so bettings can never result in a "negative stack"

                    int nameEndIndex = line.IndexOfFast(" posts ");
                    if (nameEndIndex == -1)
                    {
                        continue;
                    }

                    string playerName = line.Substring(0, nameEndIndex);

                    // only add if the player is unknown
                    if (!playerList.Any(p => p.PlayerName.Equals(playerName)))
                    {
                        playerList.Add(new Player(playerName, stack, seatNumber));
                    }
                }
                
            }

            int heroCardsIndex = GetHeroCardsIndex(handLines, lastLineRead);

            if (heroCardsIndex != -1)
            {
                string heroCardsLine = handLines[heroCardsIndex];
                if (heroCardsLine[heroCardsLine.Length - 1] == ']' &&
                    heroCardsLine.StartsWithFast("Dealt to "))
                {
                    int openSquareIndex = heroCardsLine.LastIndexOf('[');

                    string cards = heroCardsLine.Substring(openSquareIndex + 3, heroCardsLine.Length - openSquareIndex - 3 - 2);
                    HoleCards holeCards = HoleCards.FromCards(cards.Replace(" ", ""));

                    string playerName = heroCardsLine.Substring(9, openSquareIndex - 1 - 9);

                    Player player = playerList.First(p => p.PlayerName.Equals(playerName));
                    player.HoleCards = holeCards;
                }
            }

            // Looking for the showdown info which looks like this
            // Player1 checks
            // Player2 shows [ 8h, 5h, Ad, 3d ]high card Ace.
            // Player1 shows [ 9h, Qd, Qs, 6d ]three of a kind, Queens.
            // Player1 wins $72 USD from the main pot with three of a kind, Queens.

            for (int lineNumber = handLines.Length - 1; lineNumber > 0; lineNumber--)
            {
                //jimmyhoo: shows [7h 6h] (a full house, Sevens full of Jacks)
                //EASSA: mucks hand 
                //jimmyhoo collected $562 from pot
                string line = handLines[lineNumber];
                //Skip when player mucks and collects
                //EASSA: mucks hand 
                char lastChar = line[line.Length - 1];
                if (lastChar != '.')
                {
                    break;
                }

                if (!line.Contains(" show"))
                {
                    continue;
                }

                int lastSquareBracket = line.LastIndexOf(']');

                if (lastSquareBracket == -1)
                {
                    continue;
                }

                int firstSquareBracket = line.LastIndexOf('[', lastSquareBracket);

                // can show single cards:
                // Zaza5573: shows [Qc]
                if (line[firstSquareBracket + 3] == ']')
                {
                    continue;
                }

                int nameEndIndex = GetNameEndIndex(line);// line.IndexOf(' ');

                string playerName = line.Remove(nameEndIndex);

                string cards = line.Substring(firstSquareBracket + 1, lastSquareBracket - (firstSquareBracket + 1));

                playerList[playerName].HoleCards = HoleCards.FromCards(cards);
            }

            return playerList;
        }

        static bool isAnonymousPlayer(string playerName)
        {
            if (playerName.Length == 6 || playerName.Length == 7)
            {
                return playerName.StartsWithFast("Player") && char.IsDigit(playerName[6]);
            }
            return false;
        }

        static int GetHeroCardsIndex(string[] handLines, int startIndex)
        {
            for (int i = startIndex; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line[0] == '*' && line[line.Length - 1] == '*')
                {
                    return i + 1;
                }
            }
            return -1;
        }

        static int GetNameEndIndex(string line)
        {
            int nameEndIndex = line.IndexOfFast(" doesn't show [ ");
            if (nameEndIndex == -1)
            {
                nameEndIndex = line.IndexOfFast(" shows [ ");
            }

            return nameEndIndex;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            // Expected board:
            // "** Dealing Flop ** [ Tc, 7c, Qc ]"
            // "** Dealing Turn ** [ Jc ]"
            // "** Dealing River ** [ 4h ]"

            string cards = "";

            for (int lineNumber = 1; lineNumber < handLines.Length; lineNumber++)
            {
                string line = handLines[lineNumber];
                if (line[0] == '*' && line[line.Length - 1] == ']')
                {
                    int cardsStartIndex = line.IndexOf('[') + 2;
                    int cardsEndIndex = line.IndexOf(']', cardsStartIndex);
                    cards += line.Substring(cardsStartIndex, cardsEndIndex - cardsStartIndex);
                }
            }

            return BoardCards.FromCards(cards);
        }

        protected override string ParseHeroName(string[] handlines)
        {
            //Expected hero line:
            //"Dealt to PlayerName [  3s 4d Qd 8h ]"

            for (int i = 0; i < handlines.Length; i++)
            {
                if (handlines[i][0] == 'D' && handlines[i].StartsWithFast("Dealt to "))
                {
                    string line = handlines[i];
                    int endIndex = line.LastIndexOf('[');
                    return line.Substring(9, endIndex - 9 - 1);
                }
            }
            return null;
        }

        public int ParseBlindActions(string[] handLines, ref List<HandAction> handActions, int firstActionIndex)
        {
            for (int i = firstActionIndex; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line[line.Length - 2] == ']' || line[line.Length - 1] == ']')
                {
                    handActions.Add(ParseBlindAction(line));
                }
                else if (line == "** Dealing down cards **")
                {
                    return i + 1;
                }
            }

            throw new HandActionException(string.Join(Environment.NewLine, handLines), "No cards was dealt");
        }

        public static HandAction ParseBlindAction(string line)
        {
            const int smallBlindWidth = 19;//" posts small blind ".Length
            const int bigBlindWidth = 17;//" posts big blind ".Length
            const int PostingWidth = 24;//" posts big blind + dead ".Length
            const int AnteWidth = 12;//" posts big blind + dead ".Length

            string playerName;
            HandActionType action;
            decimal amount;

            int amountStartIndex = line.LastIndexOf('[');
            char blindIdentifier = line[amountStartIndex - 9];
            switch (blindIdentifier)
            {
                //"Player posts big blind [$10 USD]."
                case 'i':
                    playerName = line.Remove(amountStartIndex - bigBlindWidth);
                    action = HandActionType.BIG_BLIND;
                    amount = ParseDecimal(line, amountStartIndex + 1);
                    break;

                //"Player posts small blind [$5 USD]."
                case 'l':
                    playerName = line.Remove(amountStartIndex - smallBlindWidth);
                    action = HandActionType.SMALL_BLIND;
                    amount = ParseDecimal(line, amountStartIndex + 1);
                    break;

                //Peacli posts big blind + dead [$3].
                //or may be
                //skullcrusher99 posts big blind + dead [6 $].
                case 'd':
                    playerName = line.Remove(amountStartIndex - PostingWidth);
                    action = HandActionType.POSTS;
                    string deadString = line.Substring(amountStartIndex + 1, line.Length - amountStartIndex - 2 - 2);
                    amount = ParseDecimal(line, amountStartIndex + 1);
                    break;

                //saboniiplz posts ante [400]
                case 's':
                    playerName = line.Remove(amountStartIndex - AnteWidth);
                    action = HandActionType.ANTE;
                    amount = ParseDecimal(line, amountStartIndex + 1);
                    break;
                default:
                    throw new ArgumentException("Unknown posting Action: " + line);
            }

            return new HandAction(playerName, action, amount, Street.Preflop);
        }

        protected override void FinalizeHandHistory(HandHistory Hand)
        {
            FixSitoutPlayers(Hand);
            if (Hand.Players.Any(p => isAnonymousPlayer(p.PlayerName)))
	        {
                var anonTableType = TableTypeDescription.Anonymous;
		        Hand.GameDescription.TableType = new TableType(Hand.GameDescription.TableType.Concat(new List<TableTypeDescription>(){anonTableType}));
	        }
        }
    }
}
