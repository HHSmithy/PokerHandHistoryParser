using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.LineCategoryParser.Base;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.FastParsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Parsers.LineCategoryParser.PartyPoker
{
    sealed partial class PartyPokerLineCatParserImpl : HandHistoryParserLineCatImpl
    {
        public override SiteName SiteName => SiteName.PartyPoker;
        public override bool RequiresUncalledBetFix => true;
        public override bool RequiresUncalledBetWinAdjustment => true;
        public override bool RequiresTotalPotCalculation => true;
        public override bool RequiresAllInUpdates => true;

        public override void Categorize(Categories cat, string[] lines)
        {
            int i = 0;
            //Header
            for (; i < 5; i++)
            {
                cat.Add(LineCategory.Header, lines[i]);
            }
            //Seats
            for (; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!(line.StartsWithFast("Seat ") && line.EndsWith(')')))
                {
                    break;
                }
                cat.Add(LineCategory.Seat, line);
            }
            //Skip tourney info
            //Trny: 125049467 Level: 12
            //Blinds - Antes(1.200 / 2.400 - 400)
            for (; i < lines.Length; i++)
            {
                var line = lines[i];
                //Player4 is sitting out
                //Player3 posts small blind [$0.01 USD].
                //smultrontutten posts ante [400]
                if (line.EndsWith('.') 
                    || line.EndsWith(']') 
                    || line.EndsWith('t'))
                {
                    break;
                }
            }
            //Actions
            for (; i < lines.Length; i++)
            {
                var line = lines[i];
                if (isDealtToLine(line))//Dealt to PP_Hero [  Jc Kd ]
                {
                    cat.Add(LineCategory.Other, line);
                }
                else if (isConnectionLostLine(line))
                {
                    cat.Add(LineCategory.Other, line);
                }
                else if (isChatLine(line))
                {
                    cat.Add(LineCategory.Ignore, line);
                }
                else if(line.EndsWithFast(" is sitting out"))
                {
                    cat.Add(LineCategory.Seat, line);
                }
                else if (isIgnoredLine(line))
                {
                    cat.Add(LineCategory.Ignore, line);
                }
                else
                {
                    cat.Add(LineCategory.Action, line);
                }
            }
        }

        static bool isChatLine(string line)
        {
            return line.Contains(": ");
        }

        static bool isDealtToLine(string line)
        {
            return line.StartsWithFast("Dealt to ") && line.EndsWith(']');
        }

        static bool isConnectionLostLine(string line)
        {
            return line == "Connection Lost due to some reason";
        }

        static bool isIgnoredLine(string line)
        {
            if (line.EndsWith('.'))
            {
                return line.EndsWithFast(" the table.") 
                    || line.EndsWithFast(" will be using their time bank for this hand.")
                    || line.StartsWithFast("Your time bank will be activated in ");
            }
            return false;
        }
        
        protected override bool IsValidHand(Categories lines)
        {
            bool isCancelled; // in this case eat it
            return IsValidOrCancelledHand(lines, out isCancelled);
        }

        protected override bool IsValidOrCancelledHand(Categories lines, out bool isCancelled)
        {
            //Expected one of the last lines to look like:
            //"Player wins $102 USD from the main pot with a flush, Ace high."
            isCancelled = false;

            for (int i = lines.Action.Count - 1; i >= lines.Action.Count - 10; i--)
            {
                var line = lines.Action[i];
                // if the line starts with ** we can definitely leave the loop
                if (line.StartsWithFast("** "))
                    break;

                if (line.Contains(" wins ")) return true;
                if (isConnectionLostLine(line)) return false;
            }

            return false;
        }

        protected override BoardCards ParseCommunityCards(List<string> summary)
        {
            // Expected board:
            // "** Dealing Flop ** [ Tc, 7c, Qc ]"
            // "** Dealing Turn ** [ Jc ]"
            // "** Dealing River ** [ 4h ]"
            string cards = "";
            foreach (var line in Lines.Action)
            {
                if (line.StartsWith('*') && line.EndsWith(']'))
                {
                    int cardsStartIndex = line.IndexOf('[') + 2;
                    int cardsEndIndex = line.IndexOf(']', cardsStartIndex);
                    cards += line.SubstringBetween(cardsStartIndex, cardsEndIndex);
                }
            }

            return BoardCards.FromCards(cards);
        }

        protected override DateTime ParseDateUtc(List<string> header)
        {
            // Expect the second line to look like this: 
            // "$600 USD PL Omaha - Thursday, September 25, 01:10:46 EDT 2014"
            string line = header[1];

            int splitIndex = line.IndexOfFast(" - ") + 3;

            int monthStartIndex = line.IndexOf(',', splitIndex) + 2;
            int monthEndIndex = line.IndexOf(' ', monthStartIndex);
            string month = line.SubstringBetween(monthStartIndex, monthEndIndex);

            int dayStartIndex = monthEndIndex + 1;
            int dayEndIndex = line.IndexOf(',', dayStartIndex);
            string dayStr = line.SubstringBetween(dayStartIndex, dayEndIndex);
            int day = int.Parse(dayStr);

            int timeStartIndex = dayEndIndex + 2;
            int timeEndIndex = line.IndexOf(' ', timeStartIndex);
            string timeStr = line.SubstringBetween(timeStartIndex, timeEndIndex);

            int yearIndex = line.LastIndexOf(' ');
            string Year = line.Substring(yearIndex);
            int year = int.Parse(Year);

            string timezone = line.Substring(timeEndIndex + 1, 3);
            TimeSpan time = TimeSpan.Parse(timeStr, CultureInfo.InvariantCulture);

            DateTime result = new DateTime(year, GetMonthNumber(month), day, time.Hours, time.Minutes, time.Seconds);
            return ConvertHandDateToUtc(result, Utils.Time.TimeZoneUtil.GetTimeZoneFromAbbreviation(timezone));
        }

        static DateTime ConvertHandDateToUtc(DateTime handDate, TimeZoneInfo timezone)
        {
            DateTime converted = TimeZoneInfo.ConvertTimeToUtc(handDate, timezone);

            return DateTime.SpecifyKind(converted, DateTimeKind.Utc);
        }

        static int GetMonthNumber(string month)
        {
            switch (month)
            {
                case "January":   return 1;
                case "February":  return 2;
                case "March":     return 3;
                case "April":     return 4;
                case "May":       return 5;
                case "June":      return 6;
                case "July":      return 7;
                case "August":    return 8;
                case "September": return 9;
                case "October":   return 10;
                case "November":  return 11;
                case "December":  return 12;
                default:
                    throw new ArgumentException("Month: " + month);
            }
        }

        protected override int ParseDealerPosition(List<string> header)
        {
            // Expect the 6th line to look like this:
            // "Seat 4 is the button"
            return FastInt.Parse(header[3], 5);
        }

        protected override GameType ParseGameType(List<string> header)
        {
            if (ParsePokerFormat(header) == PokerFormat.CashGame)
            {
                return ParseCashGameGametype(header);
            }
            else
            {
                return ParseGametypeTournament(header);
            }
        }

        private static GameType ParseCashGameGametype(List<string> header)
        {
            // Expect the fourth line to look like this: 
            // "$600 USD PL Omaha - Thursday, September 25, 01:10:46 EDT 2014"
            string line = header[1];

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

            throw new UnrecognizedGameTypeException(line, "Unrecognized game-type: " + line.Substring(startIndex, endIndex - startIndex));
        }

        protected override List<HandAction> ParseHandActions(List<string> actions, out List<WinningsAction> winners)
        {
            // actions take place from the last seat info until the *** SUMMARY *** line            
            List<HandAction> handActions = new List<HandAction>(actions.Count);
            winners = new List<WinningsAction>();

            int i = 0;
            //Parse blinds
            for (; i < actions.Count; i++)
            {
                var line = actions[i];
                if (line == "** Dealing down cards **")
                {
                    i++;
                    break;
                }

                handActions.Add(ParseBlindAction(line));
            }

            Street currentStreet = Street.Preflop;
            //Parse actions
            for (; i < actions.Count; i++)
            {
                var line = actions[i];
                try
                {
                    ParseRegularActionLine(line, ref currentStreet, handActions, winners);
                }
                catch (Exception ex)
                {
                    // in some cases chatlines don't have playername involved, so we ignore every parsing error where no playername is involved 
                    // AND the line does not start with a star
                    // !!! THIS SHOULD BE DISABLED FOR DEBUGGING PURPOSES !!!
                    var players = ParsePlayers(Lines.Seat);
                    if (!players.Select(p => p.PlayerName).Any(line.Contains) && line[0] != '*')
                        continue;

                    // skip emoticon lines like
                    // zehnbube - no skill
                    // zehnbube - donkey fish
                    if (players.Select(p => p.PlayerName + " -").Any(line.Contains))
                        continue;

                    // if it's the last line, there can be some weird chat lines that we can ignore
                    if (i == actions.Count - 1)
                        continue;

                    throw new HandActionException(line, "Couldn't parse line '" + line + " with ex: " + ex.Message);
                }
            }

            return handActions;
        }

        static HandAction ParseBlindAction(string line)
        {
            string playerName;
            HandActionType action;

            int amountStartIndex = line.LastIndexOf('[');
            char blindIdentifier = line[amountStartIndex - 9];
            switch (blindIdentifier)
            {
                //"Player posts big blind [$10 USD]."
                case 'i':
                    playerName = line.Remove(amountStartIndex - 17);//" posts big blind ".Length
                    action = HandActionType.BIG_BLIND;
                    break;

                //"Player posts small blind [$5 USD]."
                case 'l':
                    playerName = line.Remove(amountStartIndex - 19);//" posts small blind ".Length
                    action = HandActionType.SMALL_BLIND;
                    break;

                //Peacli posts big blind + dead [$3].
                //or may be
                //skullcrusher99 posts big blind + dead [6 $].
                case 'd':
                    playerName = line.Remove(amountStartIndex - 24);//" posts big blind + dead ".Length
                    action = HandActionType.POSTS;
                    break;

                //saboniiplz posts ante [400]
                case 's':
                    playerName = line.Remove(amountStartIndex - 12);//" posts big blind + dead ".Length
                    action = HandActionType.ANTE;
                    break;
                default:
                    throw new ArgumentException("Unknown posting Action: " + line);
            }

            var amount = ParseDecimal(line, amountStartIndex + 1);
            return new HandAction(playerName, action, amount, Street.Preflop);
        }

        /// <summary>
        /// Parses a handaction or changes the current street
        /// </summary>
        /// <param name="line"></param>
        /// <param name="currentStreet"></param>
        /// <param name="handActions"></param>
        /// <returns>True if we have reached the end of the action block.</returns>
        public static void ParseRegularActionLine(string line, ref Street currentStreet, List<HandAction> actions, List<WinningsAction> winners)
        {
            char lastChar = line[line.Length - 1];
            switch (lastChar)
            {
                //Expected formats:
                //player posts small blind [$5 USD].
                //player posts big blind [$10 USD].
                case '.':
                    ParseDotAction(line, currentStreet, actions, winners);
                    return;
                case ']':
                    char firstChar = line[0];
                    if (firstChar == '*')
                    {
                        currentStreet = ParseStreet(line);
                    }
                    else
                    {
                        actions.Add(ParseActionWithSize(line, currentStreet));
                    }
                    return;
                case 's':
                    //saboniiplz wins 28,304 chips
                    if (line[line.Length - 2] == 'p')
                    {
                        winners.Add(ParseWinsAction(line));
                    }
                    else
                    {
                        actions.Add(ParseActionWithoutSize(line, currentStreet));
                    }
                    return;
                //Expected Formats:
                //"Player wins $5.18 USD"
                case 'D':
                    winners.Add(ParseWinsAction(line));
                    return;
            }
        }

        static HandAction ParseActionWithoutSize(string line, Street street)
        {
            char identifier = line[line.Length - 2];
            switch (identifier)
            {
                //Expected formats:
                case 'd'://"<playername> folds"
                    return new HandAction(line.Remove(line.Length - 6), HandActionType.FOLD, 0m, street);
                case 'k'://"<playername> checks"
                    return new HandAction(line.Remove(line.Length - 7), HandActionType.CHECK, 0m, street);
                default:
                    throw new ArgumentException("Unknown Action: \"" + line + "\"");
            }
        }

        static HandAction ParseActionWithSize(string line, Street currentStreet)
        {
            int amountStartIndex = line.LastIndexOf('[');
            decimal amount = ParseDecimal(line, amountStartIndex + 1);

            char actionID = line[amountStartIndex - 3];
            string playerName;
            switch (actionID)
            {
                case 't'://Player bets [$23.75 USD]
                    playerName = line.Remove(amountStartIndex - 6);//" bets ".Length
                    return new HandAction(playerName, HandActionType.BET, amount, currentStreet);
                case 'l'://Player calls [$25 USD]
                    playerName = line.Remove(amountStartIndex - 7);//" calls ".Length
                    return new HandAction(playerName, HandActionType.CALL, amount, currentStreet);
                case 'e'://Player raises [$30 USD]
                    playerName = line.Remove(amountStartIndex - 8);//" raises ".Length
                    return new HandAction(playerName, HandActionType.RAISE, amount, currentStreet);
                case 'n'://"dr. spaz is all-In  [$4.90 USD]"
                    playerName = line.Remove(amountStartIndex - 12);//" is all-In  ".Length
                    return new HandAction(playerName, HandActionType.ALL_IN, amount, currentStreet);
                default:
                    throw new ArgumentException("Unknown actionID: " + line);
            }
        }

        static WinningsAction ParseWinsAction(string line, int potID = 0)
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
                return new WinningsAction(playerName, WinningsActionType.WINS_SIDE_POT, amount, potID);
            }
            return new WinningsAction(playerName, WinningsActionType.WINS, amount, potID);
        }

        static void ParseDotAction(string line, Street street, List<HandAction> actions, List<WinningsAction> winners)
        {
            string playerName;
            int playerNameIndex = 0;

            char lastChar = line[line.Length - 2];
            if (lastChar == ']')
            {
                throw new ArgumentException("Blinds must be parsed with ParseBlindAction(string)");
            }
            else if (line.Contains(" shows"))
            {
                if (isWinType(line, " shows [", ref playerNameIndex))
                {
                    playerName = line.Remove(playerNameIndex);
                    actions.Add(new HandAction(playerName, HandActionType.SHOW, 0m, Street.Showdown));
                    return;
                }
                else if (line.Contains(" for low."))
                {
                    playerNameIndex = line.IndexOfFast(" shows");
                    playerName = line.Remove(playerNameIndex);
                    actions.Add(new HandAction(playerName, HandActionType.SHOWS_FOR_LOW, 0m, Street.Showdown));
                    return;
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
                    winners.Add(new WinningsAction(playerName, WinningsActionType.WINS, amount, 0));
                    return;
                }

                if (isWinType(line, " wins Lo (", ref playerNameIndex))
                {
                    amountStartIndex = playerNameIndex + " wins Lo (".Length + 1;
                    decimal amount = ParseDecimal(line, amountStartIndex);

                    playerName = line.Remove(playerNameIndex);
                    winners.Add(new WinningsAction(playerName, WinningsActionType.WINS_THE_LOW, amount, 0));
                    return;
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
                        winners.Add(ParseWinsAction(line, id));
                        return;
                    }
                    else
                    {
                        winners.Add(ParseWinsAction(line));
                        return;
                    }
                }
            }
            else if (line.Contains(" does not "))
            {
                playerNameIndex = line.IndexOfFast(" does not ");
                playerName = line.Remove(playerNameIndex);
                actions.Add(new HandAction(playerName, HandActionType.MUCKS, 0m, Street.Showdown));
                return;
            }
            else if (line.Contains(" doesn't show"))
            {
                playerNameIndex = line.IndexOfFast(" doesn't show");
                playerName = line.Remove(playerNameIndex);
                actions.Add(new HandAction(playerName, HandActionType.SHOW, 0m, Street.Showdown));
                return;
            }
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

        protected override long[] ParseHandId(List<string> header)
        {
            // Expect the first line to look like this: 
            // "***** Hand History for Game 13550493674 *****"
            const int firstDigitIndex = 28;//"***** Hand History for Game ".Length

            string line = header[0];
            string handId = line.SubstringBetween(firstDigitIndex, line.Length - 6);
            return HandID.Parse(handId);
        }

        protected override string ParseHeroName(List<string> other)
        {
            //Expected hero line:
            //"Dealt to PlayerName [  3s 4d Qd 8h ]"
            var line = other.FirstOrDefault(isDealtToLine);
            if (line != null)
            {
                int endIndex = line.LastIndexOf('[');
                return line.SubstringBetween(9, endIndex - 1);
            }
            return null;
        }

        protected override Limit ParseLimit(List<string> header)
        {
            var format = ParsePokerFormat(header);
            if (format == PokerFormat.CashGame)
            {
                return ParseCashgameLimit(header);
            }
            else
            {
                return ParseTournamentLimit(header);
            }
        }

        private Limit ParseCashgameLimit(List<string> header)
        {
            // The different formats for the stakes:
            // "$5/$10 USD FL Texas Hold'em"
            // "$600 USD PL Omaha"

            string line = header[1];
            string limitSubstring = line.SubstringBetween(1, line.IndexOf(' '));

            Currency currency = ParseLimitCurrency(line);

            // If there is a game limit with a slash then the limit is in form $2/$4
            // then convert the game limit into a game type without a slash which would be 400 for 2/4
            if (limitSubstring.Contains("/"))
            {
                return ParseNormalLimit(limitSubstring, currency);
            }

            string tableName = ParseTableName(header);
            GameType game = ParseGameType(header);

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

        static Currency ParseLimitCurrency(string line)
        {
            char currencySymbol = line[0];

            switch (currencySymbol)
            {
                case '$':
                    return Currency.USD;
                case '€':
                    return Currency.EURO;
                case '£':
                    return Currency.GBP;
                case 'N':
                case 'P':
                case 'F':
                    return Currency.CHIPS;
                default:
                    throw new LimitException(line, "Unrecognized currency symbol " + currencySymbol);
            }
        }

        protected override PlayerList ParsePlayers(List<string> seats)
        {
            PlayerList playerList = new PlayerList();

            foreach (var line in seats)
            {
                char lastChar = line[line.Length - 1];

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
                else if (line.EndsWithFast(" is sitting out"))
                {
                    string playerName = line.Remove(line.Length - 15); //" is sitting out".Length
                    playerList[playerName].IsSittingOut = true;
                }
            }

            //Parse Dealt to hero
            var heroCardsLine = Lines.Other.FirstOrDefault(isDealtToLine);
            if (heroCardsLine != null)
            {
                int openSquareIndex = heroCardsLine.LastIndexOf('[');

                string cards = heroCardsLine.SubstringBetween(openSquareIndex + 3, heroCardsLine.Length - 2);
                HoleCards holeCards = HoleCards.FromCards(cards.Replace(" ", ""));

                string playerName = heroCardsLine.SubstringBetween(9, openSquareIndex - 1);

                Player player = playerList.First(p => p.PlayerName.Equals(playerName));
                player.HoleCards = holeCards;
            }

            // Looking for the showdown info which looks like this
            // Player1 checks
            // Player2 shows [ 8h, 5h, Ad, 3d ]high card Ace.
            // Player1 shows [ 9h, Qd, Qs, 6d ]three of a kind, Queens.
            // Player1 wins $72 USD from the main pot with three of a kind, Queens.
            var actionlines = Lines.Action;
            for (int lineNumber = actionlines.Count - 1; lineNumber > 0; lineNumber--)
            {
                //jimmyhoo: shows [7h 6h] (a full house, Sevens full of Jacks)
                //EASSA: mucks hand 
                //jimmyhoo collected $562 from pot
                string line = actionlines[lineNumber];
                //Skip when player mucks and collects
                //EASSA: mucks hand 
                char lastChar = line[line.Length - 1];

                if (lastChar != '.') break;
                if (!line.Contains(" show")) continue;

                int lastSquareBracket = line.LastIndexOf(']');
                if (lastSquareBracket == -1)
                {
                    continue;
                }

                int firstSquareBracket = line.LastIndexOf('[', lastSquareBracket);

                // can show single cards:
                // Zaza5573: shows [Qc]
                if (lastSquareBracket - firstSquareBracket <= 3)
                {
                    continue;
                }

                int nameEndIndex = line.IndexOfFast(" doesn't show [ ");
                if (nameEndIndex == -1)
                {
                    nameEndIndex = line.IndexOfFast(" shows [ ");
                }

                string playerName = line.Remove(nameEndIndex);
                string cards = line.SubstringBetween(firstSquareBracket + 1, lastSquareBracket);

                playerList[playerName].HoleCards = HoleCards.FromCards(cards);
            }

            return playerList;
        }

        protected override PokerFormat ParsePokerFormat(List<string> header)
        {
            string line = header[1];

            if (line.Contains("Buy-in Trny:"))
            {
                if (header[2].Contains("Table #"))
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

        protected override SeatType ParseSeatType(List<string> header)
        {
            // line 5 looks like :
            // "Total number of players : 2/6 "
            var line = header[4];
            int maxPlayerIndex = line.LastIndexOf('/') + 1;

            // 2-max, 6-max or 9-max
            int maxPlayers = FastInt.Parse(line[maxPlayerIndex]);

            // can't have 1max so must be 10max
            if (maxPlayers == 1)
            {
                maxPlayers = 10;
            }

            return SeatType.FromMaxPlayers(maxPlayers);
        }

        protected override string ParseTableName(List<string> header)
        {
            // Line 3 is in form:
            // "Table Houston (Real Money)"
            string line = header[2];
            return line.SubstringBetween(6, line.LastIndexOfFast(" ("));
        }

        protected override TableType ParseTableType(List<string> header)
        {
            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
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

        public IEnumerable<string[]> SplitUpMultipleHandsToLines(string rawHandHistories)
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

        static decimal ParseDecimal(string line, int startIndex)
        {
            int endIndex = line.IndexOf(' ', startIndex);
            if (endIndex == -1)
                endIndex = line.IndexOf(']', startIndex);

            string text = line.Substring(startIndex, endIndex - startIndex);
            return text.ParseAmount();
        }

        static Street ParseStreet(string line)
        {
            const int identifierIndex = 11;
            char streetIDChar = line[identifierIndex];
            switch (streetIDChar)
            {
                case 'F'://** Dealing Flop ** [ 8s, 5c, 6h ]
                    return Street.Flop;
                case 'T'://** Dealing Turn ** [ Ac ]
                    return Street.Turn;
                case 'R'://** Dealing River ** [ Tc ]
                    return Street.River;
                default:
                    throw new ArgumentException("Unknown streetID: " + streetIDChar);
            }
        }

        public override void FinalizeHand(HandHistory hand)
        {
            FixSitoutPlayers(hand);
            if (hand.Players.Any(p => isAnonymousPlayer(p.PlayerName)))
            {
                var anonTableType = TableTypeDescription.Anonymous;
                hand.GameDescription.TableType = new TableType(hand.GameDescription.TableType.Concat(new List<TableTypeDescription>() { anonTableType }));
            }
        }

        static bool isAnonymousPlayer(string playerName)
        {
            if (playerName.Length == 6 || playerName.Length == 7)
            {
                return playerName.StartsWithFast("Player") && char.IsDigit(playerName[6]);
            }
            return false;
        }

        static void FixSitoutPlayers(HandHistory Hand)
        {
            //The sitting out attribute is not present if the player is waiting for Big Blind
            foreach (var player in Hand.Players)
            {
                player.IsSittingOut = Hand.HandActions.FirstOrDefault(p => p.PlayerName == player.PlayerName) == null;
            }
        }
    }
}
