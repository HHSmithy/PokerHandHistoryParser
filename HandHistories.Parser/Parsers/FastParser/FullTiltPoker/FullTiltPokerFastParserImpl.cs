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
            // Full Tilt Poker Game #26862468195: Table Adornment (6 max, shallow) - $0.50/$1 - No Limit Hold'em - 16:09:19 ET - 2010/12/31

            string handLine = handLines[0];

            string gameTypeString = handLine.Split('-')[2];

            switch (gameTypeString)
            {
                case " No Limit Hold'em ":
                    return GameType.NoLimitHoldem;                
                case " Pot Limit Omaha Hi ":
                    return GameType.PotLimitOmaha;
                case " Limit Hold'em ":
                    return GameType.FixedLimitHoldem;
                case " Cap Pot Limit Omaha Hi ":
                    return GameType.CapPotLimitOmaha;                
                default:
                    if (gameTypeString.EndsWith("Cap No Limit Hold'em "))
                    {
                        return GameType.CapNoLimitHoldem;                        
                    }
                    else if (gameTypeString.EndsWith("Cap Pot Limit Omaha Hi "))
                    {
                        return GameType.CapPotLimitOmaha;    
                    }
                    throw new UnrecognizedGameTypeException(handLine, "Did not recognize.");
            }
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            // Expect the first line to look like:
            // Full Tilt Poker Game #28617512574: Table Bri (6 max) - $0.25/$0.50 - $15 Cap No Limit Hold'em - 18:46:08 ET - 2011/02/28

            string limit = handLines[0].Split('-')[1];
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

            int slashIndex = limitSubstring.IndexOf('/');

            decimal small = decimal.Parse(limitSubstring.Substring(1, slashIndex - 1), System.Globalization.CultureInfo.InvariantCulture);
            string bigString = limitSubstring.Substring(slashIndex + 2, limitSubstring.Length - slashIndex - 2);
            decimal big = decimal.Parse(bigString, System.Globalization.CultureInfo.InvariantCulture);

            // If it is an ante table we expect to see an ante line after the big blind
            decimal ante = 0;
            bool isAnte = false;

            return Limit.FromSmallBlindBigBlind(small, big, currency, isAnte, ante);
        }

        public override bool IsValidHand(string[] handLines)
        {
            return (handLines[1].StartsWith("Seat "));
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            // tood: implement

            return new List<HandAction>();
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            PlayerList playerList = new PlayerList();

            for (int i = 1; i < 12; i++)
            {
                string handLine = handLines[i];

                if (handLine.StartsWith("Seat ") == false)
                {
                    return playerList;
                }

                if (handLine.EndsWith(")") == false)
                {
                    // handline is like Seat 6: ffbigfoot ($0.90), is sitting out
                    handLine = handLine.Substring(0, handLine.Length - 16);
                }

                //Seat 1: CardBluff ($109.65)

                int colonIndex = handLine.IndexOf(':', 5);
                int parenIndex = handLine.IndexOf('(', colonIndex + 2);

                int seat = Int32.Parse(handLine.Substring(colonIndex - 2, 2));
                string name = handLine.Substring(colonIndex + 2, parenIndex - 1 - colonIndex - 2);
                string stackSizeString = handLine.Substring(parenIndex + 2, handLine.Length - 1 - parenIndex - 2);
                decimal amount = decimal.Parse(stackSizeString, System.Globalization.CultureInfo.InvariantCulture);

                playerList.Add(new Player(name, amount, seat));

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
                    return;
                }
            }

            throw new Exception("Couldn't find sumamry line.");
        }
    }
}
