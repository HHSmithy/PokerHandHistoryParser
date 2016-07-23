using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Extensions;

namespace HandHistories.Parser.Parsers.FastParser.OnGame
{
    public sealed class OnGameFastParserImpl : HandHistoryParserFastImpl
    {
        private readonly SiteName _siteName;

        public override SiteName SiteName
        {
            get { return _siteName; }
        }

        public override bool RequiresUncalledBetFix
        {
            get { return true; }
        }

        static readonly char[] CurrencyChars = new char[] { '€', '$' };

        private readonly NumberFormatInfo _numberFormatInfo;
        private readonly Currency _currency;
        // So the same parser can be used for It and Fr variations
        public OnGameFastParserImpl(SiteName siteName = SiteName.OnGame)
        {
            _siteName = siteName;
            
            _numberFormatInfo = new NumberFormatInfo
            {
                NegativeSign = "-",
                CurrencyDecimalSeparator = ".",
                CurrencyGroupSeparator = ",",
            };

            switch (siteName)
            {
                case SiteName.OnGameIt:
                case SiteName.OnGameFr:
                    _numberFormatInfo.CurrencySymbol = "€";
                    _currency = Currency.EURO;
                    break;
                default:
                    _numberFormatInfo.CurrencySymbol = "$";
                    _currency = Currency.USD;
                    break;

            }
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            // Line 4 is:
            //  Button: seat 8

            //When Hero is playing there is one extra line
            //User: ONG_Hero
            //Button: seat 9
            //Players in round: 4 (6)
            //Seat 1: Opponent1 ($200)
            //Seat 2: ONG_Hero ($200)

            string line = handLines[3][0] == 'U' ? handLines[4] : handLines[3];

            int indexOfLastSpace = line.LastIndexOf(' ');

            return Int32.Parse(line.Substring(indexOfLastSpace + 1, line.Length - indexOfLastSpace - 1));                
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            // 2nd line is:
            //    Start hand: Mon May 28 08:08:00 CEST 2012

            int firstColon = handLines[1].IndexOf(':');
            string dateString = handLines[1].Substring(firstColon + 2, handLines[1].Length - firstColon - 2);

            // Trim day of week
            string [] dateStringSplit = dateString.Split(' ');

            int month = DateTime.ParseExact(dateStringSplit[1], "MMM", CultureInfo.CurrentCulture).Month;
            int day = Int32.Parse(dateStringSplit[2]);
            int hour = Int32.Parse(dateStringSplit[3].Substring(0, 2));
            int minute = Int32.Parse(dateStringSplit[3].Substring(3, 2));
            int second = Int32.Parse(dateStringSplit[3].Substring(6, 2));
            int year = Int32.Parse(dateStringSplit[5]);                      

            DateTime date = new DateTime(year, month, day, hour, minute, second);

            string timeZone = dateStringSplit[4];

            switch (timeZone)
            {
                case "GMT":
                    return date;
                case "CEST": // Central European Summer Time
                    return date.AddHours(-2);
                case "BST": // British Summer TIme
                case "CET":
                    return date.AddHours(-1);
                case "PST":
                    return date.AddHours(8);
                case "PDT":
                    return date.AddHours(7);//http://www.timeanddate.com/time/zones/pdt
                default:
                    throw new ParseHandDateException(timeZone, "Unrecognized time-zone");
            }            
        }

        protected override void ParseExtraHandInformation(string[] handLines, HandHistorySummary handHistory)
        {
            handHistory.Rake = 0m;
            handHistory.TotalPot = 0m;
            for (int i = handLines.Length - 1; i >= 0; i--)
            {
                string handLine = handLines[i];

                //Rake taken: $0.12
                if (handLine[0] == 'R')
                {
                    handHistory.Rake = decimal.Parse(handLine.Substring(12).TrimStart(CurrencyChars), _numberFormatInfo);
                }

                //Main pot: $1.28 won by cristimanea ($1.20)
                //Side pot 1: $0.70 won by cristimanea ($0.66)
                else if (handLine[1] == 'i' || handLine[0] == 'M')
                {
                    int colonIndex = handLine.IndexOf(':');
                    int wonIndex = handLine.IndexOfFast(" won by");

                    handHistory.TotalPot += decimal.Parse(handLine.Substring(colonIndex + 2, wonIndex - colonIndex - 2).TrimStart(CurrencyChars), _numberFormatInfo);
                }

                // we hit the summary line
                else if (handLine[1] == 'u')
                {
                    return;
                }
            }

            throw new InvalidHandException("Couldn't find sumamry line.");
        }

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            return PokerFormat.CashGame;
        }

        protected override long ParseHandId(string[] handLines)
        {
            // 1st line is:
            //  ***** History for hand R5-244090560-4 *****

            int indexOfR = handLines[0].IndexOf('R');

            string handNumber = handLines[0].Substring(indexOfR + 1, handLines[0].Length - indexOfR - 1 - 6);

            return long.Parse(handNumber.Replace("-", ""));
        }

        protected override long ParseTournamentId(string[] handLines)
        {
            throw new NotImplementedException();
        }

        protected override string ParseTableName(string[] handLines)
        {
            // Line 3 is:
            //  Table: Nijmegen [244308222] (POT_LIMIT OMAHA_HI $0.10/$0.10, Real money)

            int firstParenIndex = handLines[2].IndexOf('(');

            return handLines[2].Substring(7, firstParenIndex - 8);
        }

        static readonly int SeatTypeStartIndex = "Players in round: ".Length;

        protected override SeatType ParseSeatType(string[] handLines)
        {
            // Line 5 onward is:
            //  Players in round: 3
            //  Seat 8: Max Power s ($8.96) 
            //  Seat 9: EvilJihnny99 ($24.73) 
            //  Seat 10: huda22 ($21.50) 

            //When Hero is playing there is one extra line
            //User: ONG_Hero
            //Button: seat 9
            //Players in round: 4 (6)
            //Seat 1: Opponent1 ($200)
            //Seat 2: ONG_Hero ($200)

            int numPlayers = ParseNumberOfPlayers(handLines);

            if (numPlayers <= 2)
            {
                return SeatType.FromMaxPlayers(2);
            }
            if (numPlayers <= 6)
            {
                return SeatType.FromMaxPlayers(6);
            }
            if (numPlayers <= 9)
            {
                return SeatType.FromMaxPlayers(9);
            }
            
            return SeatType.FromMaxPlayers(10);            
        }

        private static bool isHeroHandhistory(string[] handLines)
        {
            return handLines[3][0] == 'U';
        }

        private static int ParseNumberOfPlayers(string[] handLines)
        {
            // Line 5 onward is:
            //  Players in round: 3
            //  Seat 8: Max Power s ($8.96) 
            //  Seat 9: EvilJihnny99 ($24.73) 
            //  Seat 10: huda22 ($21.50) 

            //When Hero is playing there is one extra line
            //User: ONG_Hero
            //Button: seat 9
            //Players in round: 4 (6)
            //Seat 1: Opponent1 ($200)
            //Seat 2: ONG_Hero ($200)
            string line = isHeroHandhistory(handLines) ? handLines[5] : handLines[4];

            int SeatTypeEndIndex = line.EndsWithFast(")") ? line.IndexOf('(', SeatTypeStartIndex) : line.Length;

            int numPlayers = Int32.Parse(line.Substring(SeatTypeStartIndex, SeatTypeEndIndex - SeatTypeStartIndex));
            return numPlayers;
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            // Line 3 looks like:

            // Table: [SPEED] Austin [243945479] (NO_LIMIT TEXAS_HOLDEM $5/$10, Real money)
            // Table: San Marcos [244090560] (POT_LIMIT OMAHA_HI $0.25/$0.25, Real money)

            int parenIndex = handLines[2].IndexOf('(');

            string substring = handLines[2].Substring(parenIndex + 1, handLines[2].Length - 1 - parenIndex - 1);
            string bettingType = substring.Substring(0, substring.IndexOf(' '));
            string gameType = substring.Substring(bettingType.Length + 1, substring.IndexOf(' ', bettingType.Length + 2) - bettingType.Length - 1);

            switch (bettingType)
            {
                case "LIMIT":
                    switch (gameType)
                    {
                        case "TEXAS_HOLDEM":
                            return GameType.FixedLimitHoldem;
                    }
                    break;
                case "NO_LIMIT":
                    switch (gameType)
                    {
                        case "TEXAS_HOLDEM":
                            return GameType.NoLimitHoldem;
                        case "OMAHA_HI":
                            return GameType.NoLimitOmaha;
                    }
                    break;
                case "POT_LIMIT":
                    switch (gameType)
                    {
                        case "OMAHA_HI":
                            return GameType.PotLimitOmaha;
                        case "TEXAS_HOLDEM":
                            return GameType.PotLimitHoldem;    
                        case "OMAHA_HI_LO":
                            return GameType.PotLimitOmahaHiLo;                            
                    }
                    break;                                    
            }

            throw new UnrecognizedGameTypeException(handLines[2], "Unrecognized game-type");
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            string tableName = ParseTableName(handLines);

            if (tableName.StartsWithFast("[SPEED]"))
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Speed);
            }

            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            // Line 3 is:
            //  Table: San Marcos [244090560] (POT_LIMIT OMAHA_HI $0.25/$0.25, Real money)
            var line = handLines[2];

            int slashIndex = line.LastIndexOf('/');
            int commaIndex = line.IndexOf(',', slashIndex + 1);
            int currencyIndex = line.LastIndexOf(' ', slashIndex) + 1;
            var sbstring = line.Substring(currencyIndex, slashIndex - currencyIndex);
            var bbstring = line.Substring(slashIndex + 1, commaIndex - slashIndex - 1);

            decimal smallBlind = decimal.Parse(sbstring.TrimStart(CurrencyChars), _numberFormatInfo);
            decimal bigBlind = decimal.Parse(bbstring.TrimStart(CurrencyChars), _numberFormatInfo);

            
            var c = GetCurrency(sbstring[0]);

            return Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, c);
        }

        private static Currency GetCurrency(char c)
        {
            switch (c)
            {
                case '$':
                    return Currency.USD;
                case '€':
                    return Currency.EURO;
                default:
                    return Currency.PlayMoney;
            }
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            throw new NotImplementedException();
        }

        public override bool IsValidHand(string[] handLines)
        {
            return handLines[handLines.Length - 1].StartsWithFast("***** End of hand ") &&
                   handLines[0].StartsWithFast("***** History for hand") &&
                   handLines.Count() > 7;
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;
            return IsValidHand(handLines);
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            // this is needed for future uncalledbet fixes
            var handHistory = new HandHistory();
            ParseExtraHandInformation(handLines, handHistory);

            int actionsIndex = GetFirstActionIndex(handLines);

            List<HandAction> handActions = new List<HandAction>();
            Street currentStreet = Street.Preflop;

            for (int i = actionsIndex; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if (handLine.StartsWithFast("Seat ")) // done with actions once we reach the seat line again
                {
                    break;
                }

                bool isAllIn = handLine.EndsWithFast("all in]");
                if (isAllIn)
                {
                    handLine = handLine.Substring(0, handLine.Length - 9); 
                }

                if (handLine[0] == '-')
                {
                    if (handLine == "---")
                    {
                        continue;
                    }

                    // check for a line such as:
                    //  --- Dealing flop [3d, 8h, 6d]
                    // note: We can have players with - so hence the digit check to check for lines like
                    //   ---ich--- raises $21.42 to $22.92 [all in] 
                    if (handLine[handLine.Length - 1] == ']' &&
                        char.IsDigit(handLine[handLine.Length - 2]) == false)
                    {
                        char streetIdentifierChar = handLine[12];
                        switch (streetIdentifierChar)
                        {
                            case 'f':
                                currentStreet = Street.Flop;
                                continue;
                            case 't':
                                currentStreet = Street.Turn;
                                continue;
                            case 'r':
                                currentStreet = Street.River;
                                continue;
                        }    
                    }                                                            
                }

                //Dealing line may be "Dealing pocket cards"
                //or "Dealing to {PlayerName}: [ Xy, Xy ]"
                if (currentStreet == Street.Preflop &&
                    handLine.StartsWithFast("Dealing"))
                {
                    continue;
                }

                if (handLine[handLine.Length - 1] == ':') // check for Summary: line
                {
                    currentStreet = Street.Showdown;
                    actionsIndex = i;
                    break;
                }

                if (handLine[handLine.Length - 1] == ')' && currentStreet != Street.Showdown)
                {
                    handActions.Add(ParseBlindAction(handLine));
                    continue;
                }
                else
                {
                    handActions.Add(ParseRegularActionLine(handLine, currentStreet, isAllIn));
                }
        }
            if (currentStreet == Street.Showdown)
            {
                ParseShowdownActions(handLines, actionsIndex, handActions);
            }

            return handActions;
        }

        public static void ParseShowdownActions(string[] handLines, int actionsIndex, List<HandAction> handActions)
        {
            for (int i = actionsIndex; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line.StartsWithFast("Seat ")) // done with actions once we reach the seat line again
                {
                    break;
                }

                // Main pot: $2.25 won by zatli74 ($2.14)
                // Main pot: $710.00 won by alikator21 ($354.50), McCall901 ($354.50)
                if (line.StartsWithFast("Main pot:"))
                {
                    var nameStart = line.IndexOfFast(" won by") + 7;
                    string winners = line.Substring(nameStart);


                    var splitted = line.Substring(nameStart).Split(')');
                    foreach (var winner in splitted.Where(p => !string.IsNullOrWhiteSpace(p)))
                    {
                        int openParenIndex = winner.LastIndexOf('(');
                        var amountStr = winner.Substring(openParenIndex + 1);
                        decimal amount = amountStr.ParseAmount();

                        string playerName = winner.Substring(1, openParenIndex - 2).Trim();

                        handActions.Add(new WinningsAction(playerName, HandActionType.WINS, amount, 0));
                    }
                }
                // Side pot 1: $12.26 won by iplaymybest ($11.65)
                // Side pot 2: $11.10 won by zatli74 ($5.20), Hurtl ($5.20)
                if (line.StartsWithFast("Side pot "))
                {
                    var nameStart = line.IndexOfFast(" won by") + 7;
                    var splitted = line.Substring(nameStart).Split(')');

                    var potNumber = Int32.Parse(line[9].ToString());
                    foreach (var winner in splitted.Where(p => !string.IsNullOrWhiteSpace(p)))
                    {
                        int openParenIndex = winner.LastIndexOf('(');
                        var amountStr = winner.Substring(openParenIndex + 1);
                        decimal amount = amountStr.ParseAmount();

                        string playerName = winner.Substring(1, openParenIndex - 2).Trim();

                        handActions.Add(new WinningsAction(playerName, HandActionType.WINS_SIDE_POT, amount, potNumber));
                    }

                }
            }
        }

        static int GetFirstActionIndex(string[] handLines)
        {
            for (int i = 6; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if (handLine.StartsWithFast("Seat ") == false)
                {
                    return i;
                }
            }
            throw new ArgumentException("No Start Action Found");
        }

        public static HandAction ParseBlindAction(string line)
        {
            // small-blind & big-blind lines ex:
            //  GlassILass posts small blind ($0.25)
            //  EvilJihnny99 posts big blind ($0.25)
            //  19kb72 posts big blind ($6.50) [all in]

            int openParenIndex = line.LastIndexOf('(');
            decimal amount = line.Substring(openParenIndex + 1, line.Length - openParenIndex - 2).ParseAmount();

            char blindIdentifier = line[openParenIndex - 10];
            if (blindIdentifier == 'b') // big blind
            {
                string playerName = line.Substring(0, openParenIndex - 17);
                return new HandAction(playerName, HandActionType.BIG_BLIND, amount, Street.Preflop);
            }
            else if (blindIdentifier == 'a')  // small-blind
            {
                string playerName = line.Substring(0, openParenIndex - 19);
                return new HandAction(playerName, HandActionType.SMALL_BLIND, amount, Street.Preflop);
            }
            else if (blindIdentifier == 'e') // posts - dead
            {
                string playerName = line.Substring(0, openParenIndex - 18);
                return new HandAction(playerName, HandActionType.POSTS, amount, Street.Preflop);
            }

            throw new HandActionException(line, "Unknown hand-line: " + line);
        }

        public static HandAction ParseRegularActionLine(string line, Street currentStreet, bool isAllIn)
        {
            string playerName;
            // Check for folds & checks
            if (line[line.Length - 1] == 's')
            {
                if (line[line.Length - 2] == 'd') // folds
                {
                    playerName = line.Substring(0, line.Length - 6);
                    return new HandAction(playerName, HandActionType.FOLD, 0, currentStreet);
                }
                else if (line[line.Length - 2] == 'k') // checks
                {
                    playerName = line.Substring(0, line.Length - 7);
                    return new HandAction(playerName, HandActionType.CHECK, 0, currentStreet);
                }
                return null;
            }

            //Old format
            //{playername} calls $18.00

            //New format 
            //{playername} calls $13

            int currencyIndex = line.IndexOfAny(CurrencyChars);
            int valueEndIndex = line.IndexOf(' ', currencyIndex);
            if (valueEndIndex == -1)
            {
                valueEndIndex = line.Length;
            }

            char actionIdentifier = line[currencyIndex - 3];
            var amountstr = line.Substring(currencyIndex, valueEndIndex - currencyIndex);
            decimal amount = amountstr.ParseAmount();
            switch (actionIdentifier)
            {
                case 'l': // calls
                    playerName = line.Substring(0, currencyIndex - 7);
                    return new HandAction(playerName, HandActionType.CALL, amount, currentStreet, isAllIn);
                case 'e': // raises
                    // ex: zatli74 raises $1.00 to $1.00
                    playerName = line.Substring(0, currencyIndex - 8);
                    return new HandAction(playerName, HandActionType.RAISE, amount, currentStreet, isAllIn);
                case 't': // bets
                    playerName = line.Substring(0, currencyIndex - 6);
                    return new HandAction(playerName, HandActionType.BET, amount, currentStreet, isAllIn);
            }

            throw new HandActionException(line, "Unknown hand-line: " + line);
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            // Line 5 onward is:
            //  Players in round: 3
            //  Seat 8: Max Power s ($8.96) 
            //  Seat 9: EvilJihnny99 ($24.73) 
            //  Seat 10: huda22 ($21.50) 

            int numPlayers = ParseNumberOfPlayers(handLines);

            PlayerList playerList = new PlayerList();

            int StartLine = isHeroHandhistory(handLines) ? 6 : 5;

            for (int i = 0; i < numPlayers; i++)
            {
                string handLine = handLines[StartLine + i];
                
                int colonIndex = handLine.IndexOf(':');
                int parenIndex = handLine.IndexOf('(');

                string name = handLine.Substring(colonIndex + 2, parenIndex - 2 - colonIndex - 1);
                int seatNumber = Int32.Parse(handLine.Substring(5, colonIndex - 5));
                string amount = (handLine.Substring(parenIndex + 1, handLine.Length - parenIndex - 2));

                playerList.Add(new Player(name, decimal.Parse(amount.TrimStart(CurrencyChars), _numberFormatInfo), seatNumber));
            }


            var heroDealtToLineIndex = GetHeroCardsIndex(handLines, StartLine + numPlayers);
            if (heroDealtToLineIndex != -1)
            {
                string heroCardsLine = handLines[heroDealtToLineIndex];

                int openSquareIndex = heroCardsLine.LastIndexOf('[');

                string cards = heroCardsLine.Substring(openSquareIndex + 1, heroCardsLine.Length - openSquareIndex - 1 - 1);
                HoleCards holeCards = HoleCards.FromCards(cards.Replace(",", "").Replace(" ", ""));

                string playerName = heroCardsLine.Substring(11, openSquareIndex - 2 - 11);

                Player player = playerList.First(p => p.PlayerName.Equals(playerName));
                player.HoleCards = holeCards;
            }

            // Parse the player showdown hole-cards

            for (int i = handLines.Length - 2; i >= 0; i--)
            {
                // Loop backward looking for lines like:
                //  Seat 4: mr dark hor ($37.87), net: -$15.25, [Ts, 3s]

                string handLine = handLines[i];

                if (handLine.StartsWithFast("Seat") == false)
                {
                    break;
                }

                if (handLine.EndsWithFast("]") == false)
                {
                    continue;
                }

                int colonIndex = handLine.IndexOf(':');
                int parenIndex = handLine.IndexOf('(');
                string name = handLine.Substring(colonIndex + 2, parenIndex - 2 - colonIndex - 1);

                int openSquareIdex = handLine.LastIndexOf('[');
                string holeCards = handLine.Substring(openSquareIdex + 1, handLine.Length - openSquareIdex - 2);

                Player player = playerList.First(p => p.PlayerName.Equals(name));
                player.HoleCards = HoleCards.FromCards(holeCards.Replace(" ", "").Replace(",", ""));
            }

            return playerList;
        }

        static int GetHeroCardsIndex(string[] handLines, int startIndex)
        {
            for (int i = startIndex; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line[line.Length - 1] == ']' && line.StartsWithFast("Dealing to"))
                {
                    return i;
                }
            }
            return -1;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            string boardCards = string.Empty;

            for (int i = 4; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if (handLine.StartsWithFast("---") == false ||
                    handLines.Length == 3 ||
                    handLine.EndsWithFast("]") == false)
                {
                    continue;                    
                }

                int firstSquare = handLine.LastIndexOf('[');
                boardCards += handLine.Substring(firstSquare + 1, handLine.Length - firstSquare - 2);


            }

            return BoardCards.FromCards(boardCards.Replace(" ", "").Replace(",", ""));
        }

        static readonly int HeroNameStartIndex = "User: ".Length;
        protected override string ParseHeroName(string[] handlines)
        {
            string line = handlines[3];
            if (line[0] == 'U')
            {
                return line.Substring(HeroNameStartIndex);
            }
            return null;
        }
    }
}
