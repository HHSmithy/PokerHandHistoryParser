using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Strings;

namespace HandHistories.Parser.Parsers.FastParser.FullTiltPoker
{
    public class FullTiltPokerFastParserImpl : HandHistoryParserFastImpl
    {
        public override SiteName SiteName
        {
            get { return SiteName.FullTilt; }
        }

        private static readonly Regex HandSplitRegex = new Regex("(Full Tilt Poker Game #)", RegexOptions.Compiled);

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                                 .Where(s => string.IsNullOrWhiteSpace(s) == false && s.Length > 30)
                                 .Select(s => "Full Tilt Poker Game #" + s.Trim('\r', 'n')); 
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            // The button is in seat #5

            for (int i = 2; i < handLines.Length - 1; i++)
            {
                string handLine = handLines[i];

                if (handLine.StartsWith("The button "))
                {
                    int button = int.Parse(handLine[handLine.Length - 1].ToString());

                    if (button == 0)
                    {
                        return 10;
                    }

                    return button;
                }
            }

            throw new InvalidHandException("Couldn't find dealer button line.");
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            // Full Tilt Poker Game #26862429938: Table Adornment (6 max, shallow) - $0.50/$1 - No Limit Hold'em - 16:07:17 ET - 2010/12/31

            string [] split = handLines[0].Split('-');
            string timeString = split[3]; // ' 16:07:17 ET '
            string dateString = split[4];//  ' 2010/12/31 '

            int year = Int32.Parse(dateString.Substring(1, 4));
            int month = Int32.Parse(dateString.Substring(6, 2));
            int day = Int32.Parse(dateString.Substring(9, 2));

            int hour = Int32.Parse(timeString.Substring(1, 2));
            int minute = Int32.Parse(timeString.Substring(4, 2));
            int second = Int32.Parse(timeString.Substring(7, 2));

            DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
            DateTime converted = TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            return converted;
        }

        protected override long ParseHandId(string[] handLines)
        {
            // Full Tilt Poker Game #26862468195: Table Adornment (6 max, shallow) - $0.50/$1 - No Limit Hold'em - 16:09:19 ET - 2010/12/31

            string handLine = handLines[0];

            int hashIndex = 21;
            int colonIndex = handLine.IndexOf(':', hashIndex);

            string handNumber = handLine.Substring(hashIndex + 1, colonIndex - hashIndex - 1);
            return long.Parse(handNumber);
        }

        protected override string ParseTableName(string[] handLines)
        {
            // Full Tilt Poker Game #28617512574: Table Bri (6 max) - $0.25/$0.50 - $15 Cap No Limit Hold'em - 18:46:08 ET - 2011/02/28

            string table = handLines[0].Split('-')[0].Split(':')[1].Split('(')[0];
            table = table.Replace("Table", "");
            table = table.Trim();

            return table;
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            // Full Tilt Poker Game #26862468195: Table Adornment (6 max, shallow) - $0.50/$1 - No Limit Hold'em - 16:09:19 ET - 2010/12/31

            string handLine = handLines[0];
            string [] seatInfo = handLine.Split('(');

            // No ( means its full ring
            if (seatInfo.Length == 1)
            {
                return SeatType.FromMaxPlayers(9);
            }

            if (seatInfo[1].StartsWith("6 max"))
            {
                return SeatType.FromMaxPlayers(6);
            }
            else if (seatInfo[1].StartsWith("heads"))
            {
                return SeatType.FromMaxPlayers(2);
            }
            
            return SeatType.FromMaxPlayers(9);

            throw new NotImplementedException("Seat type not recognized");
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            // OLD: Full Tilt Poker Game #26862468195: Table Adornment (6 max, shallow) - $0.50/$1 - No Limit Hold'em - 16:09:19 ET - 2010/12/31
            // NEW: Full Tilt Poker Game #26862468195: Table Adornment (6 max, shallow) - NL Hold'em - $0.50/$1 - 16:09:19 ET - 2010/12/31

            // we add this code in order to make it work for both starting lines
            string gameTypeString;
            string[] splitter = handLines[0].Split('-');
            if (splitter[2].Contains("/"))
            {
                gameTypeString = splitter[1];
            }
            else
            {
                gameTypeString = splitter[2];
            }

            switch (gameTypeString)
            {
                case " FL Omaha H/L ":
                case " Limit Omaha H/L ":
                    return GameType.FixedLimitOmahaHiLo;
                case " FL Omaha":
                case " Limit Omaha":
                    return GameType.FixedLimitOmaha;
                case " CAP NL Hold'em ":
                case " Cap No Limit Hold'em":
                case " NL Hold'em ":
                case " No Limit Hold'em ":
                    return GameType.NoLimitHoldem;
                case " CAP PL Omaha Hi ":
                case " Cap Pot Limit Omaha Hi ":
                case " PL Omaha Hi ":
                case " Pot Limit Omaha Hi ":
                    return GameType.PotLimitOmaha;
                case " Cap Pot Limit Omaha H/L ":
                case " CAP PL Omaha H/L ":
                case " PL Omaha H/L ":
                case " Pot Limit Omaha H/L ":
                    return GameType.PotLimitOmahaHiLo;
                case " FL Hold'em ":
                case " Limit Hold'em ":
                    return GameType.FixedLimitHoldem;
                case " NL Omaha H/L ":
                case " No Limit Omaha H/L ":
                    return GameType.NoLimitOmahaHiLo;               
                default:
                    throw new UnrecognizedGameTypeException(handLines[0], "Did not recognize.");
            }
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            bool isCap = handLines[0].ToLower().Contains(" cap ");

            return TableType.FromTableTypeDescriptions(isCap ? TableTypeDescription.Cap : TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            // Expect the first line to look like:
            // OLD: Full Tilt Poker Game #28617512574: Table Bri (6 max) - $0.25/$0.50 - $15 Cap No Limit Hold'em - 18:46:08 ET - 2011/02/28
            // NEW: Full Tilt Poker Game #28617512574: Table Bri (6 max) - $15 Cap No Limit Hold'em - $0.25/$0.50- 18:46:08 ET - 2011/02/28
            
            // we add this code in order to make it work for both starting lines
            string limit;
            string[] splitter = handLines[0].Split('-');
            if(splitter[2].Contains("/"))
            {
                limit = splitter[2];
            }
            else
            {
                limit = splitter[1];
            }
            string limitSubstring = limit.Trim();

            char currencySymbol = limitSubstring[0];
            Currency currency;

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
                    throw new LimitException(handLines[0], "Unrecognized currency symbol " + currencySymbol);
            }

            // we don't care for the ante amount, so just throw it away
            if (limitSubstring.Contains("Ante"))
            {
                limitSubstring = limitSubstring.Split(' ')[0];
            }

            int slashIndex = limitSubstring.IndexOf('/');

            decimal small = decimal.Parse(limitSubstring.Substring(1, slashIndex - 1), System.Globalization.CultureInfo.InvariantCulture);
            string bigString = limitSubstring.Substring(slashIndex + 2, limitSubstring.Length - slashIndex - 2);
            decimal big = decimal.Parse(bigString, System.Globalization.CultureInfo.InvariantCulture);

            decimal ante = 0;
            bool isAnte = false;

            return Limit.FromSmallBlindBigBlind(small, big, currency, isAnte, ante);
        }

        public override bool IsValidHand(string[] handLines)
        {
            return (handLines[1].StartsWith("Seat "));
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;
            return IsValidHand(handLines);
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            // TODO: implement

            return new List<HandAction>();
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            PlayerList playerList = new PlayerList();

            for (int i = 1; i < 12; i++)
            {
                string handLine = handLines[i];
                var sittingOut = false;

                if (handLine.StartsWith("Seat ") == false)
                {
                    break;
                }

                if (handLine.EndsWith(")") == false)
                {
                    // handline is like Seat 6: ffbigfoot ($0.90), is sitting out
                    handLine = handLine.Substring(0, handLine.Length - 16);
                    sittingOut = true;
                }

                //Seat 1: CardBluff ($109.65)

                int colonIndex = handLine.IndexOf(':', 5);
                int parenIndex = handLine.IndexOf('(', colonIndex + 2);

                int seat = Int32.Parse(handLine.Substring(colonIndex - 2, 2));
                string name = handLine.Substring(colonIndex + 2, parenIndex - 1 - colonIndex - 2);
                string stackSizeString = handLine.Substring(parenIndex + 2, handLine.Length - 1 - parenIndex - 2);
                decimal amount = decimal.Parse(stackSizeString, System.Globalization.CultureInfo.InvariantCulture);

                playerList.Add(new Player(name, amount, seat)
                                   {
                                       IsSittingOut = sittingOut
                                   });

            }

            // OmahaHiLo has a different way of storing the hands at showdown, so we need to separate
            bool inCorrectBlock = false;
            bool isOmahaHiLo = ParseGameType(handLines).Equals(GameType.PotLimitOmahaHiLo);
            for (int lineNumber = 13; lineNumber < handLines.Length; lineNumber++)
            {
                string line = handLines[lineNumber];

                if (line.StartsWith(@"*** SUM") && isOmahaHiLo)
                {
                    lineNumber = lineNumber + 2;
                    inCorrectBlock = true;
                }
                else if(line.StartsWith(@"*** SH") && !isOmahaHiLo)
                {
                    inCorrectBlock = true;
                }

                if (inCorrectBlock == false)
                {
                    continue;
                }

                int firstSquareBracket = line.LastIndexOf('[');

                if (firstSquareBracket == -1)
                {
                    continue;
                }

                // can show single cards
                if (line[firstSquareBracket + 3] == ']')
                {
                    continue;
                }

                int lastSquareBracket = line.LastIndexLoopsBackward(']', line.Length - 1);
                int colonIndex = line.LastIndexLoopsBackward(':', lastSquareBracket);

                string seat;
                string playerName;
                if (isOmahaHiLo)
                {
                    seat = line.Substring(5, colonIndex-5);
                    playerName = playerList.First(p => p.SeatNumber.Equals(Convert.ToInt32(seat))).PlayerName;
                }
                else
                {
                    int playerNameEndIndex = line.IndexOf(" shows", StringComparison.Ordinal);
                    if (playerNameEndIndex == -1)
                        break;

                    playerName = line.Substring(0, playerNameEndIndex);
                }
                
                string cards = line.Substring(firstSquareBracket + 1, lastSquareBracket - (firstSquareBracket + 1));


                playerList[playerName].HoleCards = HoleCards.FromCards(cards);
            }
            return playerList;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            // Expect the end of the hand to have something like this:
            /**** SUMMARY ***
            Total pot $7.75 | Rake $0.35
            Board: [9h Ts 8h 6c]
            Seat 1: hockenspit50 didn't bet (folded)
            Seat 2: drc40 folded on the Turn
            Seat 3: oggie the fox didn't bet (folded)
            Seat 4: diknek (button) collected ($7.40), mucked
            Seat 5: Naturalblow (small blind) folded before the Flop
            Seat 6: BeerMySole (big blind) folded before the Flop*/

            BoardCards boardCards = BoardCards.ForPreflop();
            for (int lineNumber = handLines.Length - 2; lineNumber >= 0; lineNumber--)
            {
                string line = handLines[lineNumber];
                if (line[0] == '*')
                {
                    return boardCards;
                }

                if (line[0] != 'B')
                {
                    continue;
                }

                int firstSquareBracket = 7;
                int lastSquareBracket = line.Length - 1;

                string cards = line.Substring(firstSquareBracket + 1, lastSquareBracket - (firstSquareBracket + 1));
                return BoardCards.FromCards(cards);
            }

            throw new CardException(string.Empty, "Read through hand backwards and didn't find a board or summary.");
        }

        protected override void ParseExtraHandInformation(string[] handLines, Objects.Hand.HandHistorySummary handHistorySummary)
        {
            for (int i = handLines.Length - 1; i >= 0; i--)
            {
                string handLine = handLines[i];
                if (handLine[0] == '*')
                {
                    return;
                }

                // Total pot $42.90 | Rake $2.10            
                if (handLine[0] == 'T')
                {

                    int lastSpaceIndex = handLine.LastIndexOf(" ", System.StringComparison.Ordinal);
                    int spaceAfterFirstNumber = handLine.IndexOf(" ", 11, System.StringComparison.Ordinal);

                    handHistorySummary.Rake =
                        decimal.Parse(handLine.Substring(lastSpaceIndex + 2, handLine.Length - lastSpaceIndex - 2), System.Globalization.CultureInfo.InvariantCulture);

                    handHistorySummary.TotalPot =
                        decimal.Parse(handLine.Substring(11, spaceAfterFirstNumber - 11), System.Globalization.CultureInfo.InvariantCulture);

                    return;
                }
            }

            throw new Exception("Couldn't find sumamry line.");
        }
    }
}
