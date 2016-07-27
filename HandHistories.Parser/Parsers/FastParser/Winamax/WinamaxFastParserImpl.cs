using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Extensions;

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

        public override bool RequiresAdjustedRaiseSizes
        {
            get { return true; }
        }

        public override bool RequiresActionSorting
        {
            get { return false; }
        }

        public override bool RequiresUncalledBetFix
        {
            get { return true; }
        }

        public override bool RequiresTotalPotAdjustment
        {
            get { return true; }
        }
        
        private readonly NumberFormatInfo _numberFormatInfo = new NumberFormatInfo
            {
                NegativeSign = "-",
                CurrencyDecimalSeparator = ".",
                CurrencyGroupSeparator = ",",
                CurrencySymbol = "€"
            };

        protected override int ParseDealerPosition(string[] handLines)
        {
            string line = handLines[1];
            // Line 2  is:
            // Table: 'Cardiff' 5-max (real money) Seat #4 is the button
            var seatNumberIndex = line.LastIndexOfFast("#") + 1;
            var spaceIndex = line.IndexOfFast(" ", seatNumberIndex);

            string dealerStr = line.Substring(seatNumberIndex, spaceIndex - seatNumberIndex);
            return Int32.Parse(dealerStr);
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
            string line = handLines[0];

            if (line.StartsWithFast("Winamax Poker - Tournament "))
            {
                var tableName = ParseTableName(handLines);

                if (tableName.StartsWithFast("Sit&Go"))
                {
                    return PokerFormat.SitAndGo;
                }
                else
                {
                    return PokerFormat.MultiTableTournament;
                }
            }

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

            int startIndex = handLines[1].IndexOfFast("'") + 1;
            int endIndex = handLines[1].LastIndexOfFast("'");
            
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
                if (handLines[i].StartsWithFast("***"))
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

            string gameLimitStr = lineSplit[5];

            int limitStartIndex = gameLimitStr.IndexOf('(') + 1;
            int limitEndIndex = gameLimitStr.LastIndexOf(')');

            var limitStr = gameLimitStr.Substring(limitStartIndex, limitEndIndex - limitStartIndex);

            var limitItems = limitStr.Split('/');

            decimal smallBlind = limitItems[limitItems.Length - 2].ParseAmount();
            decimal bigBlind = limitItems[limitItems.Length - 1].ParseAmount();

            Currency currency = ParseCurrency(limitItems);

            if (limitItems.Length == 3)
            {
                decimal ante = limitItems[0].ParseAmount();
                return Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, currency, true, ante);
            }
            else
            {
                return Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, currency);
            }
        }

        static Currency ParseCurrency(string[] limitItems)
        {
            Currency currency;
            string str = limitItems[0];
            switch (str[str.Length - 1])
            {
                case '€':
                    currency = Currency.EURO;
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    currency = Currency.CHIPS;
                    break;
                default:
                    throw new ArgumentException("CurrencyNotSupported: " + limitItems[0][0]);
            }
            return currency;
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            return Buyin.FromBuyinRake(0, 0, Currency.CHIPS);
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
            var handActions = new List<HandAction>();
            var currentStreet = Street.Preflop;
            decimal smallBlindValue;

            int startOfActionsIndex = GetFirstActionIndex(handLines);

            startOfActionsIndex = ParseBlindActions(handLines, handActions, startOfActionsIndex, out smallBlindValue);

            for (int i = startOfActionsIndex; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line.StartsWithFast("*** "))
                {
                    if (line.StartsWithFast("*** SUMMARY ***"))
                    {
                        currentStreet = Street.Showdown;
                        startOfActionsIndex = i + 1;
                        break;
                    }
                    if (line.StartsWithFast("*** PRE-FLOP ***"))
                    {
                        currentStreet = Street.Preflop;
                        continue;
                    }
                    if (line.StartsWithFast("*** FLOP *** ["))
                    {
                        currentStreet = Street.Flop;
                        continue;
                    }
                    if (line.StartsWithFast("*** TURN *** ["))
                    {
                        currentStreet = Street.Turn;
                        continue;
                    }
                    if (line.StartsWithFast("*** RIVER *** ["))
                    {
                        currentStreet = Street.River;
                        continue;
                    }
                    if (line.StartsWithFast("*** SHOW DOWN ***"))
                    {
                        currentStreet = Street.Showdown;
                        startOfActionsIndex = i + 1;
                        break;
                    }

                    // skip the following lines
                    if (line.StartsWithFast("*** PRE-FLOP ***")
                     || line.StartsWithFast("*** ANTE/BLINDS ***"))
                    {
                        continue;
                    }
                }

                //we are not parsing winners until we get to the summary line
                //sined20 collected 6.26€ from pot
                //nico86190 shows [Qh Qc] (One pair : Queens)
                if (line.EndsWith(")") || line.EndsWith("pot"))
                {
                    continue;
                }

                var action = ParseRegularAction(line, currentStreet, handActions, smallBlindValue);
                if (action != null)
                {
                    handActions.Add(action);
                }
            }

            if (currentStreet == Street.Showdown)
            {
                //Parsing showdown actions
                for (int i = startOfActionsIndex; i < handLines.Length; i++)
                {
                    string line = handLines[i];
                    // lines look like:
                    // Seat 3: xGras (button) won 6.07€
                    // Seat 4: KryptonII (button) showed [Qd Ah] and won 42.32€ with One pair : Aces
                    // Seat 1: Hitchhiker won 0.90€
                    if (line.StartsWithFast("Seat "))
                    {
                        int wonIndex = line.IndexOfFast(" won ");

                        if (wonIndex != -1)
                        {
                            wonIndex += 5;
                            int currencyIndex = line.IndexOf(' ', wonIndex);
                            if (currencyIndex == -1)
                            {
                                currencyIndex = line.Length - 1;
                            }

                            var amountStr = line.Substring(wonIndex, currencyIndex - wonIndex);
                            decimal amount = amountStr.ParseAmount();

                            string name = GetPlayerNameFromHandLine(line);

                            handActions.Add(new WinningsAction(name, HandActionType.WINS, amount, 0));
                        }
                    }

                    // when a player shows his hand it looks like
                    // LEROISALO shows [Kh Ah] (high card : Ace)
                    if (line[line.Length - 1] == ')')
                    {
                        if (line.Contains("[") && line.Contains("]"))
                        {
                            string name = GetPlayerNameFromHandLine(line);

                            handActions.Add(new HandAction(name, HandActionType.SHOW, 0, Street.Showdown));
                        }

                    }
                }
            }
            
            return handActions;
        }

        public static HandAction ParseRegularAction(string line, Street currentStreet, List<HandAction> handActions, decimal smallBlindValue)
        {
            bool isAllIn = line.EndsWithFast("and is all-in");
            if (isAllIn)
            {
                line = line.Substring(0, line.Length - 14);
            }

            // Check for folds & checks
            char lastChar = line[line.Length - 1];
            if(line.StartsWithFast("Uncalled bet of "))
            {
                const int amountStart = 16;
                int amountEnd = line.IndexOf(' ', amountStart);

                string amountStr = line.Substring(amountStart, amountEnd - amountStart);
                string playerName = line.Substring(amountEnd + 13);

                return new HandAction(playerName, HandActionType.UNCALLED_BET, amountStr.ParseAmount(), currentStreet);
            }
            else if (lastChar == 's')
            {
                if (line[line.Length - 2] == 'd') // folds
                {
                    var playerName = line.Substring(0, line.Length - 6);
                    return new HandAction(playerName, HandActionType.FOLD, currentStreet);
                }
                else if (line[line.Length - 2] == 'k') // checks
                {
                    var playerName = line.Substring(0, line.Length - 7);
                    return new HandAction(playerName, HandActionType.CHECK, currentStreet);
                }
                throw new ArgumentException("Unhandled action line: " + line);
            }
            else
            {
                int lastSpaceIndex = line.LastIndexOf(' ');
                char actionIdentifier = line[lastSpaceIndex - 3];

                string amountStr = line.Substring(lastSpaceIndex + 1);
                decimal amount = amountStr.ParseAmount();

                string playerName;
                switch (actionIdentifier)
                {
                    case 'l': // calls
                        playerName = line.Substring(0, lastSpaceIndex - 6);
                        return new HandAction(playerName, HandActionType.CALL, amount, currentStreet, isAllIn);

                    case ' ': // raises
                        // ex: r.BAGGIO raises 20€ to 40€
                        int nameEndIndex = line.LastIndexOfFast(" raise", lastSpaceIndex);
                        playerName = line.Remove(nameEndIndex);

                        // 30/04/2015: if a player posts dead money (SB), his raise size is too high by the amount of the small blind
                        //             this can be verified by taking a look at pot and call sizes of other players
                        if (currentStreet == Street.Preflop)
                        {
                            var deadMoneyAction = handActions.Any(h => h.PlayerName.Equals(playerName)
                                                                       && h.HandActionType.Equals(HandActionType.POSTS)
                                                                       && Math.Abs(h.Amount).Equals(smallBlindValue));

                            if (deadMoneyAction)
                            {
                                amount -= smallBlindValue;
                            }
                        }
                        return new HandAction(playerName, HandActionType.RAISE, amount, currentStreet, isAllIn);

                    case 'e': // bets
                        playerName = line.Substring(0, lastSpaceIndex - 5);
                        return new HandAction(playerName, HandActionType.BET, amount, currentStreet, isAllIn);
                }
            }

            throw new ArgumentException("Could not parse Regular Action: " + line);
        }

        public int ParseBlindActions(string[] handLines, List<HandAction> handActions, int firstActionIndex, out decimal smallBlindValue)
        {
            HandAction LastDeadAction = null;
            // we store this value for future deadmoney detection
            smallBlindValue = 0.0m;

            for (int i = firstActionIndex; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line[0] == '*' && line.EndsWithFast(" ***"))
                {
                    return i + 1;
                }

                var action = ParseBlindAction(line, handActions, ref LastDeadAction, ref smallBlindValue);
                if (action != null)
                {
                    handActions.Add(action);
                }
            }

            throw new ArgumentException("Blinds did not end");
        }

        public static HandAction ParseBlindAction(string line, List<HandAction> handActions, ref HandAction LastDeadAction, ref decimal smallBlindValue)
        {
            if (line.StartsWithFast("Dealt to "))
            {
                return null;
            }

            // skip
            // Nhat60 denies big blind
            if (line[line.Length - 1] == 'd')
            {
                return null;
            }

            var smallBlindIndex = line.IndexOfFast("posts small blind");
            var bigBlindIndex = line.IndexOfFast("posts big blind");
            var anteIndex = line.IndexOfFast("posts ante");

            // bkk2015 posts small blind 0.25€ out of position
            // bkk2015 posts big blind 0.50€ out of position
            var deadMoney = isDeadMoney(line);
            if (deadMoney)
            {
                line = line.Substring(0, line.Length - 16);
            }

            var amountStartIndex = line.LastIndexOfFast(" ");

            var amount = line.Substring(amountStartIndex + 1, line.Length - amountStartIndex - 1).ParseAmount();

            if (smallBlindIndex > -1)
            {
                var handActionType = deadMoney ? HandActionType.POSTS : HandActionType.SMALL_BLIND;

                var playerName = line.Substring(0, smallBlindIndex - 1);
                var action = new HandAction(playerName, handActionType, amount, Street.Preflop, false);

                smallBlindValue = amount;
                if (deadMoney)
                {
                    if (LastDeadAction != null && LastDeadAction.PlayerName == playerName)
                    {
                        if (Math.Abs(LastDeadAction.Amount) > Math.Abs(action.Amount))
                        {
                            action = new HandAction(action.PlayerName, HandActionType.POSTS_DEAD, action.Amount, action.Street);
                        }
                    }
                    LastDeadAction = action;
                    return action;
                }
                else
                {
                    return action;
                }
            }

            if (bigBlindIndex > -1)
            {
                var handActionType = deadMoney ? HandActionType.POSTS : HandActionType.BIG_BLIND;

                var playerName = line.Substring(0, bigBlindIndex - 1);
                var action = new HandAction(playerName, handActionType, amount, Street.Preflop, false);


                if (deadMoney)
                {
                    if (LastDeadAction != null && LastDeadAction.PlayerName == playerName)
                    {
                        if (Math.Abs(LastDeadAction.Amount) < Math.Abs(action.Amount))
                        {
                            ConvertLastActionTo(handActions, HandActionType.POSTS_DEAD);
                        }
                    }
                    LastDeadAction = action;
                    return action;
                }
                else
                {
                    return action;
                }
            }

            if (anteIndex > -1)
            {
                var playerName = line.Substring(0, anteIndex - 1);
                return new HandAction(playerName, HandActionType.ANTE, amount, Street.Preflop, false);
            }

            throw new HandActionException(line, "Unknown hand-line: " + line);
        }

        static int GetFirstActionIndex(string[] handLines)
        {
            for (int i = 4; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line[0] == '*' && line.EndsWithFast(" ***"))
                {
                    return i + 1;
                }
            }

            throw new HandActionException(handLines[0], "Couldnt find the start of the actions");
        }

        static void ConvertLastActionTo(List<HandAction> handActions, HandActionType handActionType)
        {
            var lastAction = handActions[handActions.Count - 1];
            handActions.RemoveAt(handActions.Count - 1);
            handActions.Add(new HandAction(lastAction.PlayerName, handActionType, lastAction.Amount, lastAction.Street, lastAction.IsAllIn));
        }

        private static bool isDeadMoney(string handLine)
        {
            var deadMoney = handLine[handLine.Length - 1] == 'n';
            return deadMoney;
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            // line 3 onward has all seated player
            // Seat 1: ovechkin08 (2000€)
            // Seat 4: R.BAGGIO (2000€)
            // *** ANTE/BLINDS ***

            var playerList = new PlayerList();
            int playerListEndLine = 0;
            for (int i = 2; i < handLines.Length; i++)
            {
                string line = handLines[i];
                // when the line starts with stars, we already have all players
                if (line.StartsWithFast("***"))
                {
                    playerListEndLine = i;
                    break;
                }

                int colonIndex = line.IndexOf(':');
                int parenIndex = line.IndexOf('(');

                string name = line.Substring(colonIndex + 2, parenIndex - 2 - colonIndex - 1);
                int seatNumber = Int32.Parse(line.Substring(5, colonIndex - 5));
                string amount = (line.Substring(parenIndex + 1, line.Length - parenIndex - 2));

                if (amount == "")
                {
                    playerList.Add(new Player(name, 0, seatNumber)
                    {
                        IsSittingOut = true
                    });
                }
                else
                {
                    playerList.Add(new Player(name, amount.ParseAmount(), seatNumber));
                }
            }

            int heroCardsIndex = GetHeroCardsFirstLineIndex(handLines, playerListEndLine + 1);

            if (heroCardsIndex != -1)
            {
                string line = handLines[heroCardsIndex];

                string cards = GetCardStringFromLine(line);

                const int nameStartIndex = 9;
                int nameEndIndex = line.LastIndexOf('[') - 1;
                string name = line.Substring(nameStartIndex, nameEndIndex - nameStartIndex);

                playerList[name].HoleCards = HoleCards.FromCards(cards);
            }

            int ShowDownLineIndex = GetShowDownLineIndex(handLines, 2 + playerList.Count, handLines.Length - 3);
            if (ShowDownLineIndex != -1)
            {
                for (int i = ShowDownLineIndex; i < handLines.Length; i++)
                {
                    string line = handLines[i];

                    if (line.EndsWithFast(")") && line.Contains(" shows ["))
                    {
                        string name = GetPlayerNameFromHandLine(line);

                        Player player = playerList.First(p => p.PlayerName.Equals(name));
                        player.HoleCards = ParseHoleCards(line);
                    }
                }
            }

            for (int i = handLines.Length - 1; i >= 0; i--)
            {
                // Loop backward looking for lines like:
                // Seat 3: xGras (button) won 6.07€
                // Seat 4: KryptonII (button) showed [Qd Ah] and won 42.32€ with One pair : Aces
                // Seat 1: Hitchhiker won 0.90€
                // Seat 3: le parano (big blind) mucked


                string handLine = handLines[i];

                if (!handLine.StartsWithFast("Seat"))
                {
                    break;
                }

                string name = GetPlayerNameFromHandLine(handLine);

                Player player = playerList.First(p => p.PlayerName.Equals(name));
                if (player.HoleCards == null)
                {
                    player.HoleCards = ParseHoleCards(handLine);
                }
            }
            return playerList;
        }

        private int GetHeroCardsFirstLineIndex(string[] handLines, int index)
        {
            for (int i = index; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line.EndsWithFast(" ***"))
                {
                    return -1;
                }

                if (line[line.Length - 1] == ']' && line.StartsWithFast("Dealt to "))
                {
                    return i;
                }
            }
            return -1;
        }

        static HoleCards ParseHoleCards(string line)
        {
            int openSquareIndex = line.LastIndexOf('[') + 1;
            int closeSquareIndex = line.LastIndexOf(']');

            if (openSquareIndex == -1 || closeSquareIndex == -1)
            {
                return null;
            }

            string holeCards = line.Substring(openSquareIndex, closeSquareIndex - openSquareIndex);

            return HoleCards.FromCards(holeCards.Replace(" ", ""));
        }

        static int GetShowDownLineIndex(string[] handLines, int startIndex, int endIndex)
        {
            for (int i = endIndex; i > startIndex; i--)
            {
                string line = handLines[i];

                if (line.StartsWithFast("*** SHOW DOWN ***"))
                {
                    return i + 1;
                }
            }
            return -1;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            string boardCards = string.Empty;

            // search backwards for the "Board:"-line
            for (int i = handLines.Length-1; i >= 0; i--)
            {
                string handLine = handLines[i];

                if(!handLine.StartsWithFast("Board: ["))
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
            if (handLine.StartsWithFast("Seat ") && colonIndex > -1)
            {
                nameStartIndex = colonIndex + 2;
            }
            // TODO: improve this
            // in order to find the end of the name we need to try some things:
            int nameEndIndex = handLine.IndexOfFast(" (small blind) ", nameStartIndex);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOfFast(" (big blind) ", nameStartIndex);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOfFast(" (button) ", nameStartIndex);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOfFast(" mucked", nameStartIndex);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOfFast(" showed [", nameStartIndex);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOfFast(" won ", nameStartIndex);

            if (nameEndIndex == -1)
                nameEndIndex = handLine.IndexOfFast(" shows [", nameStartIndex);

            string name = handLine.Substring(nameStartIndex, nameEndIndex - nameStartIndex);

            return name;
        }

        protected override void ParseExtraHandInformation(string[] handLines, HandHistorySummary handHistorySummary)
        {
            if (handHistorySummary.Cancelled)
            {
                return;
            }

            for (int i = handLines.Length - 1; i >= 0; i--)
            {
                string line = handLines[i];
                

                // Check for summary line:
                //  *** SUMMARY ***
                if (line[0] == '*' && line[4] == 'S')
                {
                    // Line after summary line is:
                    //  Total pot 3€ | No rake
                    // or
                    //  Total pot 62.50€ | Rake 1.50€
                    string totalLine = handLines[i + 1];

                    int lastSpaceIndex = totalLine.LastIndexOf(' ');
                    int spaceAfterFirstNumber = totalLine.IndexOf(' ', 11);

                    string rake = totalLine.Substring(lastSpaceIndex + 1, totalLine.Length - lastSpaceIndex - 1);

                    if (totalLine.EndsWithFast("No rake"))
                    {
                        handHistorySummary.Rake = 0;
                    }
                    else
                    {
                        handHistorySummary.Rake = rake.ParseAmount();
                    }
                    
                    string totalPot = totalLine.Substring(10, spaceAfterFirstNumber - 10);

                    handHistorySummary.TotalPot = totalPot.ParseAmount();

                    // the pot in the hand history already deducted the rake, so we need to re-add it
                    handHistorySummary.TotalPot += handHistorySummary.Rake;
                    break;
                }
            }
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

        static string GetCardStringFromLine(string line)
        {
            int startIndex = line.LastIndexOf('[') + 1;
            int endIndex = line.IndexOf(']', startIndex);

            return line.Substring(startIndex, endIndex - startIndex);
        }
    }
}
