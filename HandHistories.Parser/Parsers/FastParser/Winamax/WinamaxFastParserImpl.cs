using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;

namespace HandHistories.Parser.Parsers.FastParser.Winamax
{
    public sealed class WinamaxFastParserImpl : HandHistoryParserFastImpl
    {
        private static readonly Regex HandSplitRegex = new Regex("(Winamax Poker - )", RegexOptions.Compiled);

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                                 .Where(s => string.IsNullOrWhiteSpace(s) == false && s.Length > 30)
                                 .Select(s => "Winamax Poker - " + s.Trim('\r', 'n'));
        }

        public override SiteName SiteName
        {
            get { return SiteName.Winamax; }
        }

        // TODO: CHECK
        public override bool RequiresAdjustedRaiseSizes
        {
            get { return false; }
        }

        // TODO: CHECK
        public override bool RequiresActionSorting
        {
            get { return false; }
        }

        public override bool RequiresAllInDetection
        {
            get { return false; }
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            // Line 2  is:
            // Table: 'Cardiff' 5-max (real money) Seat #4 is the button
            var seatNumberIndex = handLines[1].IndexOf("#", StringComparison.Ordinal) + 1;
            var spaceIndex = handLines[1].IndexOf(" ", seatNumberIndex, StringComparison.Ordinal);

            return Int32.Parse(handLines[1].Substring(seatNumberIndex, spaceIndex - seatNumberIndex));
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            // Line 1 is:
            // Winamax Poker - CashGame - HandId: #5276724-697-1382607755 - Holdem no limit (10€/20€) - 2013/10/24 09:42:35 UTC

            var lineSplit = handLines[0].Split('-');

            var dateStringSplit = lineSplit[lineSplit.Length-1].Split(' ');

            var year = Int32.Parse(dateStringSplit[1].Substring(0, 4));
            var month = Int32.Parse(dateStringSplit[1].Substring(5, 2));
            var day = Int32.Parse(dateStringSplit[1].Substring(8, 2));

            var hour = Int32.Parse(dateStringSplit[2].Substring(0, 2));
            var minute = Int32.Parse(dateStringSplit[2].Substring(3, 2));
            var second = Int32.Parse(dateStringSplit[2].Substring(6, 2));

            var date = new DateTime(year, month, day, hour, minute, second);

            var timeZone = dateStringSplit[3];

            switch (timeZone)
            {
                case "CEST": // Central European Summer Time
                    return date.AddHours(-2);
                case "CET":
                    return date.AddHours(-1);
                case "PST":
                    return date.AddHours(8);
                case "UTC":
                    return date;
                default:
                    throw new ParseHandDateException(timeZone, "Unrecognized time-zone");
            }
        }

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            return PokerFormat.CashGame;
        }

        protected override long ParseHandId(string[] handLines)
        {
            // Line 1 is:
            // Winamax Poker - CashGame - HandId: #5276724-697-1382607755 - Holdem no limit (10€/20€) - 2013/10/24 09:42:35 UTC
            // the full handid is too long. The id contains 3 numbers: #Table-Hand-TotalWinamaxHand
            // for our purposes it should be enough to either use Table-Hand or TotalWinamaxHand
            // with regards to our standards for other sites we use Table-Hand as HandID

            int indexOfHandIdStart = handLines[0].IndexOf('#') + 1;
            int indexOfHandIdEnd = handLines[0].IndexOf('-', indexOfHandIdStart+9); // this makes sure to skip the first appearance of '-'

            string handNumber = handLines[0].Substring(indexOfHandIdStart, indexOfHandIdEnd - indexOfHandIdStart);

            return long.Parse(handNumber.Replace("-", ""));
        }

        protected override long ParseTournamentId(string[] handLines)
        {
            throw new NotImplementedException();
        }

        protected override string ParseTableName(string[] handLines)
        {
            // Line 2  is:
            // Table: 'Cardiff' 5-max (real money) Seat #4 is the button

            int startIndex = handLines[1].IndexOf("'", StringComparison.Ordinal) + 1;
            int endIndex = handLines[1].LastIndexOf("'", StringComparison.Ordinal);
            
            return handLines[1].Substring(startIndex, endIndex - startIndex);
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            // line 4 onward has all seated player
            // Seat 1: ovechkin08 (2000€)
            // Seat 4: R.BAGGIO (2000€)
            // *** ANTE/BLINDS ***

            int numPlayers = 0;
            for(int i = 3; i< handLines.Length; i++)
            {
                if (handLines[i].StartsWith("***"))
                {
                    numPlayers = i - 3;
                    break;
                }
            }

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

        protected override GameType ParseGameType(string[] handLines)
        {
            // Line 1 is
            // Winamax Poker - CashGame - HandId: #5276724-697-1382607755 - Holdem no limit (10€/20€) - 2013/10/24 09:42:35 UTC

            var lineSplit = handLines[0].Split('-');

            var parenIndex = lineSplit[5].IndexOf('(');

            string gameTypeString = lineSplit[5].Substring(1,parenIndex - 2);

            GameType gameType = GameTypeUtils.ParseGameString(gameTypeString);
            
            if(gameType != GameType.Unknown)
            {
                return gameType;
            }
            throw new UnrecognizedGameTypeException(handLines[0], "Unrecognized game-type");
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            // There are only regular tables
            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            // Line 1 is
            // Winamax Poker - CashGame - HandId: #5276724-697-1382607755 - Holdem no limit (10€/20€) - 2013/10/24 09:42:35 UTC

            var lineSplit = handLines[0].Split('-');

            var parenIndex = lineSplit[5].IndexOf('(');

            int slashIndex = lineSplit[5].IndexOf('/');

            decimal smallBlind = decimal.Parse(lineSplit[5].Substring(parenIndex + 1, slashIndex - parenIndex - 2), CultureInfo.InvariantCulture);
            decimal bigBlind = decimal.Parse(lineSplit[5].Substring(slashIndex + 1, lineSplit[5].Length - slashIndex - 4), CultureInfo.InvariantCulture);

            return Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, Currency.EURO);
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            throw new NotImplementedException();
        }

        public override bool IsValidHand(string[] handLines)
        {
            bool isWinamax = false;
            bool hasBlindsPosted = false;
            bool hasSummary = false;

            foreach(var line in handLines)
            {
                if (line.Contains("Winamax Poker")) isWinamax = true;
                if (line.Contains("*** ANTE/BLINDS ***")) hasBlindsPosted = true;
                if (line.Contains("*** SUMMARY ***")) hasSummary = true;
            }

            return (isWinamax && hasBlindsPosted && hasSummary);
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;
            return IsValidHand(handLines);
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            int startOfActionsIndex = -1;

            for (int i = 5; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if (handLine.StartsWith("Seat ") == false)
                {
                    startOfActionsIndex = i;
                    break;
                }
            }

            if (startOfActionsIndex == -1)
            {
                throw new HandActionException(handLines[0], "Couldnt find the start of the actions");
            }

            var handActions = new List<HandAction>();
            var currentStreet = Street.Null;

            for (int i = startOfActionsIndex; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if(handLine.StartsWith("*** SUMMARY ***"))
                {
                    currentStreet = Street.Showdown;
                    i++;
                    continue;
                }

                if(handLine.StartsWith("*** "))
                {
                    if (handLine.StartsWith("*** PRE-FLOP ***"))
                    {
                        currentStreet = Street.Preflop;
                        continue;
                    } 
                    if (handLine.StartsWith("*** FLOP *** ["))
                    {
                        currentStreet = Street.Flop;
                        continue;
                    }
                    if (handLine.StartsWith("*** TURN *** ["))
                    {
                        currentStreet = Street.Turn;
                        continue;
                    }
                    if (handLine.StartsWith("*** RIVER *** ["))
                    {
                        currentStreet = Street.River;
                        continue;
                    }
                    if (handLine.StartsWith("*** SHOW DOWN ***"))
                    {
                        currentStreet = Street.Showdown;
                        continue;
                    }

                    // skip the following lines
                    if (handLine.StartsWith("*** PRE-FLOP ***")
                     || handLine.StartsWith("*** ANTE/BLINDS ***"))
                    {
                        continue;
                    }
                }

                if (currentStreet == Street.Showdown)
                {
                    // lines look like:
                    // Seat 3: xGras (button) won 6.07€
                    // Seat 4: KryptonII (button) showed [Qd Ah] and won 42.32€ with One pair : Aces
                    // Seat 1: Hitchhiker won 0.90€
                    if(handLine.StartsWith("Seat "))
                    {
                        int wonIndex = handLine.IndexOf(" won ", StringComparison.Ordinal);

                        if (wonIndex != -1)
                        {
                            int currencyIndex = handLine.IndexOf("€", wonIndex, StringComparison.Ordinal);

                            decimal amount = decimal.Parse(handLine.Substring(wonIndex + 5, currencyIndex - wonIndex - 5), CultureInfo.InvariantCulture);

                            string name = GetPlayerNameFromHandLine(handLine);

                            handActions.Add(new WinningsAction(name, HandActionType.WINS, amount, 0));  
                        }
                    }

                    // when a player shows his hand it looks like
                    // LEROISALO shows [Kh Ah] (high card : Ace)
                    if(handLine[handLine.Length-1] == ')')
                    {
                        if (handLine.Contains("[") && handLine.Contains("]"))
                        {
                            string name = GetPlayerNameFromHandLine(handLine);

                            handActions.Add(new HandAction(name, HandActionType.SHOW, 0, Street.Showdown));
                        }

                    }
                    
                    continue;
                }

                // Blind posting
                if(currentStreet.Equals(Street.Null))
                {
                    if (handLine.StartsWith("Dealt to "))
                    {
                        continue;
                    }

                    var smallBlindIndex = handLine.IndexOf("posts small blind", StringComparison.Ordinal);
                    var bigBlindIndex = handLine.IndexOf("posts big blind", StringComparison.Ordinal);

                    var amountStartIndex = handLine.LastIndexOf(" ", StringComparison.Ordinal);

                    var amount = decimal.Parse(handLine.Substring(amountStartIndex + 1, handLine.Length - amountStartIndex - 2), CultureInfo.InvariantCulture);
                   
                    if(smallBlindIndex > -1)
                    {
                        var playerName = handLine.Substring(0, smallBlindIndex - 1);
                        handActions.Add(new HandAction(playerName, HandActionType.SMALL_BLIND, amount, Street.Preflop));
                        continue;
                    }

                    if(bigBlindIndex > -1)
                    {
                        var playerName = handLine.Substring(0, bigBlindIndex - 1);
                        handActions.Add(new HandAction(playerName, HandActionType.BIG_BLIND, amount, Street.Preflop));
                        continue;
                    }

                    throw new HandActionException(handLine, "Unknown hand-line: " + handLine);
                }

                // Check for folds & checks
                if (handLine[handLine.Length - 1] == 's')
                {
                    if (handLine[handLine.Length - 2] == 'd') // folds
                    {
                        var playerName = handLine.Substring(0, handLine.Length - 6);
                        handActions.Add(new HandAction(playerName, HandActionType.FOLD, 0, currentStreet));
                    }
                    else if (handLine[handLine.Length - 2] == 'k') // checks
                    {
                        var playerName = handLine.Substring(0, handLine.Length - 7);
                        handActions.Add(new HandAction(playerName, HandActionType.CHECK, 0, currentStreet));
                    }
                    continue;
                }
                else
                {
                    
                    bool isAllIn = handLine.EndsWith("and is all-in");

                    // from here on we can skip lines that don't end on the EURO-symbol AND that are not allins
                    if (handLine[handLine.Length - 1] != '€' && !isAllIn)
                    {
                        continue;
                    }

                    var currencyIndex = handLine.IndexOf('€');
                    var lastCurrencyIndex = handLine.LastIndexOf('€');

                    var amountIndex = -1;
                    var lastAmountIndex = -1;

                    decimal amount;

                    for (int k = 1; k <= currencyIndex; k++)
                    {
                        if (handLine[currencyIndex - k] == ' ')
                        {
                            amountIndex = currencyIndex - k;
                            break;
                        }
                    }

                    if(currencyIndex != lastCurrencyIndex)
                    {
                        for (int k = 1; k <= lastCurrencyIndex; k++)
                        {
                            if (handLine[lastCurrencyIndex - k] == ' ')
                            {
                                lastAmountIndex = lastCurrencyIndex - k;
                                break;
                            }
                        }

                        amount = decimal.Parse(
                            handLine.Substring(lastAmountIndex, lastCurrencyIndex - lastAmountIndex),
                            CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        amount = decimal.Parse(handLine.Substring(amountIndex, currencyIndex - amountIndex), CultureInfo.InvariantCulture);
                    }

                    char actionIdentifier = handLine[amountIndex - 2];
                    
                    string playerName;
                    switch (actionIdentifier)
                    {
                        case 'l': // calls
                            playerName = handLine.Substring(0, amountIndex - 6);
                            if (isAllIn)
                            {
                                handActions.Add(new AllInAction(playerName, amount, currentStreet, false));
                            }
                            else
                            {
                                handActions.Add(new HandAction(playerName, HandActionType.CALL, amount, currentStreet));
                            }
                            continue;
                        case 'e': // raises
                            // ex: r.BAGGIO raises 20€ to 40€
                            playerName = handLine.Substring(0, amountIndex - 7);
                            if (isAllIn)
                            {
                                handActions.Add(new AllInAction(playerName, amount, currentStreet, true));
                            }
                            else
                            {
                                handActions.Add(new HandAction(playerName, HandActionType.RAISE, amount, currentStreet));
                            }
                            continue;
                        case 't': // bets
                            playerName = handLine.Substring(0, amountIndex - 5);
                            if (isAllIn)
                            {
                                handActions.Add(new AllInAction(playerName, amount, currentStreet, false));
                            }
                            else
                            {
                                handActions.Add(new HandAction(playerName, HandActionType.BET, amount, currentStreet));
                            }
                            continue;
                    }
                }

                throw new HandActionException(handLine, "Unknown hand-line: " + handLine);
            }

            return handActions;
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            // line 3 onward has all seated player
            // Seat 1: ovechkin08 (2000€)
            // Seat 4: R.BAGGIO (2000€)
            // *** ANTE/BLINDS ***

            var playerList = new PlayerList();

            for (int i = 2; i < handLines.Length; i++)
            {
                // when the line starts with stars, we already have all players
                if (handLines[i].StartsWith("***")) break;

                int colonIndex = handLines[i].IndexOf(':');
                int parenIndex = handLines[i].IndexOf('(');

                string name = handLines[i].Substring(colonIndex + 2, parenIndex - 2 - colonIndex - 1);
                int seatNumber = Int32.Parse(handLines[i].Substring(5, colonIndex - 5));
                string amount = (handLines[i].Substring(parenIndex + 1, handLines[i].Length - parenIndex - 2 - 1));

                playerList.Add(new Player(name, decimal.Parse(amount, CultureInfo.InvariantCulture), seatNumber));
            }

            for (int i = handLines.Length - 1; i >= 0; i--)
            {
                // Loop backward looking for lines like:
                // Seat 3: xGras (button) won 6.07€
                // Seat 4: KryptonII (button) showed [Qd Ah] and won 42.32€ with One pair : Aces
                // Seat 1: Hitchhiker won 0.90€
                // Seat 3: le parano (big blind) mucked


                string handLine = handLines[i];

                if (!handLine.StartsWith("Seat"))
                {
                    break;
                }

                string name = GetPlayerNameFromHandLine(handLine);

                int openSquareIndex = handLine.LastIndexOf('[');
                int closeSquareIndex = handLine.LastIndexOf(']');
                string holeCards = "";

                if(openSquareIndex > -1 && closeSquareIndex > -1)
                {
                    holeCards = handLine.Substring(openSquareIndex + 1, closeSquareIndex - openSquareIndex -1);   
                }

                Player player = playerList.First(p => p.PlayerName.Equals(name));
                player.HoleCards = HoleCards.FromCards(holeCards.Replace(" ", "").Replace(",", ""));

            }

            return playerList;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            string boardCards = string.Empty;

            // search backwards for the "Board:"-line
            for (int i = handLines.Length-1; i >= 0; i--)
            {
                string handLine = handLines[i];

                if(!handLine.StartsWith("Board: ["))
                {
                    continue;
                }

                int lastSquareOpen = handLine.LastIndexOf('[');

                boardCards += handLine.Substring(lastSquareOpen + 1, handLine.Length - lastSquareOpen - 2);

                // as there is only one possible board line, leave here
                break;
            }

            return BoardCards.FromCards(boardCards.Replace(" ", "").Replace(",", ""));
        }

        private string GetPlayerNameFromHandLine(string handLine)
        {
            int colonIndex = handLine.IndexOf(':');
            int nameStartIndex = 0;
            if(handLine.StartsWith("Seat ") && colonIndex > -1)
            {
                nameStartIndex = colonIndex + 2;
            }
            // TODO: improve this
            // in order to find the end of the name we need to try some things:
            int nameEndIndex = handLine.IndexOf(" (small blind) ", nameStartIndex, StringComparison.Ordinal);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOf(" (big blind) ", nameStartIndex, StringComparison.Ordinal);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOf(" (button) ", nameStartIndex, StringComparison.Ordinal);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOf(" mucked", nameStartIndex, StringComparison.Ordinal);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOf(" showed [", nameStartIndex, StringComparison.Ordinal);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOf(" won ", nameStartIndex, StringComparison.Ordinal);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOf(" shows [", nameStartIndex, StringComparison.Ordinal);

            string name = handLine.Substring(nameStartIndex, nameEndIndex - nameStartIndex);

            return name;
        }

        protected override string ParseHeroName(string[] handlines)
        {
            const string DealtTo = "Dealt to ";

            for (int i = 0; i < handlines.Length; i++)
            {
                string line = handlines[i];
                if (line[0] == 'D' && line.StartsWith(DealtTo))
                {
                    int HeroNameEndIndex = line.LastIndexOf('[') - 1;
                    return line.Substring(DealtTo.Length, HeroNameEndIndex - DealtTo.Length);
                }
            }
            return null;
        }
    }
}
