using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Strings;
using System.Globalization;
using HandHistories.Parser.Utils.FastParsing;
using HandHistories.Parser.Utils.Extensions;

namespace HandHistories.Parser.Parsers.FastParser.PartyPoker
{
    public sealed class PartyPokerFastParserImpl : HandHistoryParserFastImpl
    {
        const int gameIDStartIndex = 17;
        static CultureInfo provider = CultureInfo.InvariantCulture;

        private SiteName _siteName;

        public override SiteName SiteName
        {
            get { return _siteName; }
        }

        public override bool RequiresTotalPotCalculation
        {
            get { return true; }
        }

        // So the same parser can be used for It and Fr variations
        public PartyPokerFastParserImpl(SiteName siteName = SiteName.PartyPoker)
        {
            _siteName = siteName;
        }

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            const string splitStr = "***** Hand ";

            return rawHandHistories.LazyStringSplit(splitStr)
                .Where(hand => hand.StartsWith("History", StringComparison.Ordinal))
                .Select(p => splitStr + p);
        }

        protected override string[] SplitHandsLines(string handText)
        {
            return base.SplitHandsLines(handText)
                .TakeWhile(p => !p.StartsWith("Game #", StringComparison.Ordinal) && !p.EndsWith(" starts.", StringComparison.Ordinal))
                .ToArray();
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

            int splitIndex = line.IndexOf(" - ") + 3;

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

            
            TimeSpan time = TimeSpan.Parse(timeStr, provider);

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
            int endIndex = line.LastIndexOf(" (");
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
                decimal limitBB = decimal.Parse(limitSubstring, System.Globalization.CultureInfo.InvariantCulture);
                return Limit.FromSmallBlindBigBlind(limitBB / 2.0m, limitBB, currency);
            }

            // Handle 20BB tables, due to Party putting the limit up as 40% of the actual
            // limit. So for instance 20BB party $100NL the limit is displayed as $40NL.
            // No idea why this is so.    
            if (tableName.StartsWith("20BB"))
            {
                return Parse20BBLimit(limitSubstring, currency);
            }

            return ParseBuyInLimit(limitSubstring, currency);
        }

        static Limit Parse20BBLimit(string limitSubstring, Currency currency)
        {
            decimal bigblind = decimal.Parse(limitSubstring, provider) * 0.4m;

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
                default:
                    throw new LimitException(line, "Unrecognized currency symbol " + currencySymbol);
            }
            return currency;
        }

        static Limit ParseBuyInLimit(string limitSubstring, Currency currency)
        {
            decimal buyIn = decimal.Parse(limitSubstring, provider);
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

            decimal small = decimal.Parse(SB, provider);
            decimal big = decimal.Parse(BB, provider);
            return Limit.FromSmallBlindBigBlind(small, big, currency);
        }

        public override bool IsValidHand(string[] handLines)
        {
            bool isCancelled; // in this case eat it
            return IsValidOrCancelledHand(handLines, out isCancelled);
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            //Expected last line:
            //"Player wins $102 USD from the main pot with a flush, Ace high."

            isCancelled = false;

            for (int i = handLines.Length - 1; i > 0; i--)
            {
                string line = handLines[i];
                if (line.Contains(" wins "))
                {
                    return true;
                }
            }

            return false;
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            // actions take place from the last seat info until the *** SUMMARY *** line            

            int actionIndex = GetFirstActionIndex(handLines);

            List<HandAction> handActions = new List<HandAction>(handLines.Length - actionIndex);

            actionIndex = ParseBlindActions(handLines, ref handActions, actionIndex);

            Street currentStreet = Street.Preflop;

            for (int lineNumber = actionIndex; lineNumber < handLines.Length; lineNumber++)
            {
                string handLine = handLines[lineNumber];

                try
                {
                    bool isFinished = ParseLine(handLine, gameType, ref currentStreet, ref handActions);

                    if (isFinished)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    throw new HandActionException(handLine, "Couldn't parse line '" + handLine + " with ex: " + ex.Message);
                }
            }

            return handActions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="currentStreet"></param>
        /// <param name="handActions"></param>
        /// <returns>True if we have reached the end of the action block.</returns>
        private bool ParseLine(string line, GameType gameType, ref Street currentStreet, ref List<HandAction> handActions)
        {
            //Chat lines start with the PlayerName and
            //PlayerNames can contain characters that disturb parsing
            //So we must check for chatlines first
            char lastChar = line[line.Length - 1];
            HandAction action = null;

            switch (lastChar)
            {
                //Expected formats:
                //player posts small blind [$5 USD].
                //player posts big blind [$10 USD].
                case '.':
                    action = ParseDotAction(line, currentStreet);
                    break;

                case ']':
                    char firstChar = line[0];
                    if (firstChar == '*')
                    {
                        currentStreet = ParseStreet(line);
                        return false;
                    }
                    else if(line.StartsWith("Dealt to"))
                    {
                        return false;
                    }
                    else
                    {
                        action = ParseActionWithSize(line, currentStreet);
                    }
                    break;

                case 's':
                    action = ParseActionWithoutSize(line, currentStreet);
                    break;
                
                //Expected Formats:
                //"Player wins $5.18 USD"
                case 'D':
                    action = ParseWinsAction(line);
                    break;
            }

            if (action != null)
	        {
		        handActions.Add(action);
            }
            return false;
        }

        static HandAction ParseWinsAction(string line, int potID = 0)
        {
            //Expected Formats:
            //"Player wins $5.18 USD"
            int nameEndIndex = line.IndexOf(" wins ");
            string playerName = line.Remove(nameEndIndex);

            int amountStartIndex = nameEndIndex + 7;
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
            decimal amount = ParseDecimal(line, amountStartIndex + 2);

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
                    return new AllInAction(playerName, amount, currentStreet, true);
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
            string playerName;
            char lastChar = line[line.Length - 2];
            int playerNameIndex = 0;

            if (line.EndsWith("this hand."))
            {
                return null;
            }
            else if (line.EndsWith(" has left the table."))
            {
                return null;
            }
            else if (line.EndsWith(" has joined the table."))
            {
                return null;
            }
            //Your time bank will be activated in 6 secs. If you do not want it to be used, please act now.
            else if (line.EndsWith(" please act now."))
            {
                return null;
            }
            else if (line.Contains(" shows"))
            {
                if (isWinType(line, " shows [", ref playerNameIndex))
                {
                    playerName = line.Remove(playerNameIndex);
                    return new HandAction(playerName, HandActionType.SHOW, 0m, street);
                }
                else if (line.Contains(" for low."))
                {
                    playerNameIndex = line.IndexOf(" shows");
                    playerName = line.Remove(playerNameIndex);
                    return new HandAction(playerName, HandActionType.SHOWS_FOR_LOW, 0m, street);
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
                    decimal amount = ParseDecimal(line, amountStartIndex);
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
                playerNameIndex = line.IndexOf(" does not ");
                playerName = line.Remove(playerNameIndex);
                return new HandAction(playerName, HandActionType.MUCKS, 0m, street);
            }
            else if (line.Contains(" doesn't show "))
            {
                playerNameIndex = line.IndexOf(" doesn't show ");
                playerName = line.Remove(playerNameIndex);
                return new HandAction(playerName, HandActionType.SHOW, 0m, street);
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
            string text = line.Substring(startIndex, endIndex - startIndex);
            return decimal.Parse(text, provider);
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
            const int playerListEnd = playerListStart + 10; //there may at most be 10 players
            for (int lineNumber = playerListStart; lineNumber < playerListEnd; lineNumber++)
            {
                string line = handLines[lineNumber];

                char startChar = line[0];
                char endChar = line[line.Length - 1];

                //Seat 4: thaiJhonny ( $1,404 USD )
                if (endChar != ')')
                {
                    lastLineRead = lineNumber;
                    break;
                }

                // seat info expected in format: 
                //Seat 4: thaiJhonny ( $1,404 USD )
                const int seatNumberStartIndex = 5;

                int seatNumber = FastInt.Parse(line, seatNumberStartIndex);

                int playerNameStartIndex = line.IndexOf(':', seatNumberStartIndex) + 2;

                // we need to find the ( before the number. players can have ( in their name so we need to go backwards and skip the last one
                int openParenIndex = line.LastIndexOf('(');

                string playerName = line.Substring(playerNameStartIndex, openParenIndex - playerNameStartIndex - 1);
                decimal stack = ParseDecimal(line, openParenIndex + 3);

                playerList.Add(new Player(playerName, stack, seatNumber));
            }

            if (lastLineRead == -1)
            {
                throw new PlayersException(string.Empty, "Didn't break out of the seat reading block.");
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

        static int GetNameEndIndex(string line)
        {
            int nameEndIndex = line.IndexOf(" doesn't show [ ");
            if (nameEndIndex == -1)
            {
                nameEndIndex = line.IndexOf(" shows [ ");
            }

            return nameEndIndex;
        }

        static int GetShowDownStartIndex(string[] handLines, int lastLineRead, int summaryIndex)
        {
            for (int i = lastLineRead; i < summaryIndex; i++)
            {
                if (handLines[i][0] == '*' && handLines[i][4] == 'S' && handLines[i][5] == 'H')//handLines[i].StartsWith("*** SH"))
                {
                    return i;
                }
            }
            return -1;
        }

        static int GetSummaryStartIndex(string[] handLines, int lastLineRead)
        {
            for (int lineNumber = handLines.Length - 3; lineNumber > lastLineRead; lineNumber--)
            {
                if (handLines[lineNumber][0] != 'S' && 
                    handLines[lineNumber][0] != 'T' &&
                    handLines[lineNumber][0] != 'B')
                {
                    return lineNumber;
                }
            }
            //Summary must exist or it is not a valid Pokerstars Hand
            throw new IndexOutOfRangeException("Could not find *** Summary ***");
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            // Expected board:
            // "** Dealing Flop ** [ Tc, 7c, Qc ]"
            // "** Dealing Turn ** [ Jc ]"
            // "** Dealing River ** [ 4h ]"

            string cards = "";
            BoardCards boardCards = BoardCards.ForPreflop();
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
                if (handlines[i][0] == 'D' && handlines[i].StartsWith("Dealt to "))
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
            const int smallBlindWidth = 19;//" posts small blind ".Length
            const int bigBlindWidth = 17;//" posts big blind ".Length
            const int PostingWidth = 24;//" posts big blind + dead ".Length

            for (int i = firstActionIndex; i < handLines.Length; i++)
            {
                string line = handLines[i];

                char lastChar = line[line.Length - 2];
                if (lastChar == ']')
                {
                    int amountStartIndex = line.LastIndexOf('[');
                    string playerName;
                    HandActionType action;
                    decimal amount;

                    char blindIdentifier = line[amountStartIndex - 9];

                    switch (blindIdentifier)
                    {
                        //"Player posts big blind [$10 USD]."
                        case 'i':
                            playerName = line.Remove(amountStartIndex - bigBlindWidth);
                            action = HandActionType.BIG_BLIND;
                            amount = ParseDecimal(line, amountStartIndex + 2);
                            break;

                        //"Player posts small blind [$5 USD]."
                        case 'l':
                            playerName = line.Remove(amountStartIndex - smallBlindWidth);
                            action = HandActionType.SMALL_BLIND;
                            amount = ParseDecimal(line, amountStartIndex + 2);
                            break;

                        //Peacli posts big blind + dead [$3].
                        case 'd':
                            playerName = line.Remove(amountStartIndex - PostingWidth);
                            action = HandActionType.POSTS;
                            string deadString = line.Substring(amountStartIndex + 2, line.Length - amountStartIndex - 2 - 2);
                            amount = Decimal.Parse(deadString, provider);
                            break;


                        default:
                            throw new ArgumentException("Unknown posting Action: " + line);
                    }

                    handActions.Add(new HandAction(playerName, action, amount, Street.Preflop));
                }

                if (line == "** Dealing down cards **")
                {
                    return i + 1;
                }
            }

            throw new HandActionException(string.Join(Environment.NewLine, handLines), "No cards was dealt");
        }
    }
}
