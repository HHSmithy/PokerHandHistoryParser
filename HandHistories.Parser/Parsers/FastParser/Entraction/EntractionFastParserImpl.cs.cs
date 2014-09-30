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
using HandHistories.Parser.Utils.Time;

namespace HandHistories.Parser.Parsers.FastParser.Entraction
{
    class EntractionFastParserImpl : HandHistoryParserFastImpl
    {
        public override SiteName SiteName
        {
            get { return SiteName.Entraction; }
        }

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            //Single Hand Identification Regex = Game #.*?Game.*?\w\w\w(\+|-)(\d+\:\d+)
            var matches = Regex.Matches(rawHandHistories, @"Game #.*?Game.*?\w\w\w(\+|-)(\d+\:\d+)",
                                        RegexOptions.Multiline | RegexOptions.Singleline);

            foreach (var match in matches)
            {
                yield return match.ToString() + "\r\n";
            }
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            PlayerList players = ParsePlayers(handLines);

            for (int i = 3; i < handLines.Length - 1; i++)
            {
                string handLine = handLines[i];

                if (handLine.StartsWith("Dealer:"))
                {
                    string dealerName = handLine.Replace("Dealer:", "").TrimStart(' ');

                    return players.First(p => p.PlayerName.Equals(dealerName)).SeatNumber;
                }
            }

            throw new PlayersException(handLines[0], "Couldn't find a dealer position.");
        }

        private static readonly Regex DateRegex = new Regex(@"\d+-\d+-\d+ \d+\:\d+\:\d+", RegexOptions.Compiled);
        protected override DateTime ParseDateUtc(string[] handLines)
        {
            string dateString = DateRegex.Match(handLines[handLines.Length - 1]).Value;

            DateTime dateTime = DateTime.Parse(dateString);

            DateTime utcTime = TimeZoneUtil.ConvertDateTimeToUtc(dateTime, TimeZoneType.GMT1);

            return utcTime;
        }

        private static readonly Regex HandIdRegex = new Regex(@"(?<=Game # )\d+", RegexOptions.Compiled);
        protected override long ParseHandId(string[] handLines)
        {
            return long.Parse(HandIdRegex.Match(handLines[0]).Value);
        }

        private static readonly Regex TableNameRegex = new Regex("(?<=Table \").*?(?=\")", RegexOptions.Compiled);
        protected override string ParseTableName(string[] handLines)
        {
            return TableNameRegex.Match(handLines[0]).Value;            
        }

        private static readonly Regex SeatTypeRegex = new Regex(@"(?<=max )\d+", RegexOptions.Compiled);
        protected override SeatType ParseSeatType(string[] handLines)
        {
            int seatCount = Int32.Parse(SeatTypeRegex.Match(handLines[1]).Value);
            return SeatType.FromMaxPlayers(seatCount);
        }

        private static readonly Regex GameTypeRegex = new Regex( @"(?<=\d - ).*?(?= \w\w\w \d)", RegexOptions.Compiled);
        protected override GameType ParseGameType(string[] handLines)
        {
            string gameTypeString = GameTypeRegex.Match(handLines[0]).Value;

            switch (gameTypeString)
            {
                case "Omaha High Pot Limit":
                    return GameType.PotLimitOmaha;
                case "Texas Hold'em Pot Limit":
                    return GameType.PotLimitHoldem;
                case "Omaha Hi/Lo Fixed Limit":
                    return GameType.FixedLimitOmahaHiLo;
                case "Texas Hold'em No Limit":
                    return GameType.NoLimitHoldem;
                case "Texas Hold'em Fixed Limit":
                    return GameType.FixedLimitHoldem;
                case "5-Card Omaha High Pot Limit":
                    return GameType.FiveCardPotLimitOmaha;
                case "5-Card Omaha Hi/Lo Pot Limit":
                    return GameType.FiveCardPotLimitOmahaHiLo;
                default:
                    throw new UnrecognizedGameTypeException(handLines[0], "Unrecognized game-type.");
            }
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        private static readonly Regex LimitRegex = new Regex(@"\d+\.\d+/\d+\.\d+", RegexOptions.Compiled);
        private static readonly Regex CurrencyRegex = new Regex(@"\w+(?= \d+\.\d)", RegexOptions.Compiled);
        protected override Limit ParseLimit(string[] handLines)
        {
            string limitString = LimitRegex.Match(handLines[0]).Value;

            decimal lowLimit = decimal.Parse(limitString.Split('/')[0], System.Globalization.CultureInfo.InvariantCulture);
            decimal highLimit = decimal.Parse(limitString.Split('/')[1], System.Globalization.CultureInfo.InvariantCulture);

            string currencyString = CurrencyRegex.Match(handLines[0]).Value;

            //Fix for parsing EUR
            if (currencyString.Equals("EUR"))
            {
                currencyString = "EURO";
            }

            Currency currency;
            if (!Enum.TryParse(currencyString, true, out currency))
            {
                throw new LimitException(handLines[0], string.Format("Failed to parse Currency. Currency String is: {0}", currencyString));
            }

            return Limit.FromSmallBlindBigBlind(lowLimit, highLimit, currency);
        }

        public override bool IsValidHand(string[] handLines)
        {
            return handLines[handLines.Length - 1].StartsWith("Game ended ");
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;
            return IsValidHand(handLines);
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            List<HandAction> handActions = new List<HandAction>();

            Street currentStreet = Street.Null;

            for (int i = 3; i < handLines.Length - 1; i++)
            {
                string handLine = handLines[i]; 
                string name = string.Empty;
                decimal amount = 0;

                if (string.IsNullOrWhiteSpace(handLine))
                {
                    continue;
                }

                if (currentStreet == Street.Null)
                {
                    if (handLine.StartsWith("Dealer:"))
                    {
                        currentStreet = Street.Preflop;
                    }
                    continue;
                }

                if (handLine.Contains(" didn't show hand"))
                {
                    currentStreet = Street.Showdown;
                    continue;
                }
                if (handLine.Contains(" shows: "))
                {
                    int firstSpaceIndexOf = handLine.IndexOf(' ');                    
                    name = handLine.Substring(0, firstSpaceIndexOf);
                    currentStreet = Street.Showdown;
                    handActions.Add(new HandAction(name, HandActionType.SHOW, 0, currentStreet));
                    continue;
                }
                if (handLine.Contains(" wins: "))
                {
                    //stook wins:                 EUR 23.04
                    currentStreet = Street.Showdown;
                    int firstSpaceIndexOf = handLine.IndexOf(' ');
                    int lastSpaceIndex = handLine.LastIndexOf(' ');

                    name = handLine.Substring(0, firstSpaceIndexOf);
                    amount = decimal.Parse(handLine.Substring(lastSpaceIndex + 1, handLine.Length - lastSpaceIndex - 1), System.Globalization.CultureInfo.InvariantCulture);

                    handActions.Add(new WinningsAction(name, HandActionType.WINS, amount, 0));
                    continue;
                }

                if (handLine.StartsWith("Flop "))
                {
                    currentStreet = Street.Flop;
                    continue;
                }
                if (handLine.StartsWith("Turn "))
                {
                    currentStreet = Street.Turn;
                    continue;
                }
                if (handLine.StartsWith("River "))
                {
                    currentStreet = Street.River;
                    continue;
                }
                if (handLine.StartsWith("Rake: "))
                {
                    break;
                }

                int firstSpaceIndex = handLine.IndexOf(' ');
                int openParenIndex = handLine.IndexOf('(');
                int spaceAfterAction = handLine.IndexOf(' ', 28);
                if (spaceAfterAction == -1)
                {
                    spaceAfterAction = handLine.Length;
                }
                int colonIndex = handLine.IndexOf(':');               

                if (openParenIndex != -1)
                {
                    amount = decimal.Parse(handLine.Substring(openParenIndex + 1, handLine.Length - openParenIndex - 2), System.Globalization.CultureInfo.InvariantCulture);
                }

                if (currentStreet == Street.Preflop)
                {                    
                    // Check for lines like:
                    //  Small Blind:                wELMA       (0.50)
                    //  Big Blind:                  Vrddhi      (1.00)
                    if (colonIndex != -1)
                    {                        
                        name = handLine.Substring(28, spaceAfterAction - 28);
                        if (handLine[0] == 'S')
                        {
                            handActions.Add(new HandAction(name, HandActionType.SMALL_BLIND, amount, currentStreet));
                            continue;
                        }
                        if (handLine[0] == 'B')
                        {
                            handActions.Add(new HandAction(name, HandActionType.BIG_BLIND, amount, currentStreet));
                            continue;
                        }
                        
                        throw new HandActionException(handLine, "Unrecognized preflop action.");
                    }                   
                }

                string action = handLine.Substring(28, spaceAfterAction - 28);
                name = handLine.Substring(0, firstSpaceIndex);

                switch (action)
                {
                    case "Raise":
                        handActions.Add(new HandAction(name, HandActionType.RAISE, amount, currentStreet));
                        continue;
                    case "Check":
                        handActions.Add(new HandAction(name, HandActionType.CHECK, amount, currentStreet));
                        continue;
                    case "Fold":
                        handActions.Add(new HandAction(name, HandActionType.FOLD, amount, currentStreet));
                        continue;
                    case "Bet":
                        handActions.Add(new HandAction(name, HandActionType.BET, amount, currentStreet));
                        continue;
                    case "Call":
                        handActions.Add(new HandAction(name, HandActionType.CALL, amount, currentStreet));
                        continue;
                    case "Payback":
                        handActions.Add(new HandAction(name, HandActionType.UNCALLED_BET, amount, currentStreet));
                        continue;
                    case "All-In":                        
                        handActions.Add(new AllInAction(name, amount, currentStreet, false));
                        continue;
                }

                throw new HandActionException(handLine, "Unrecognized preflop action.");
            }

            
            return handActions;
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            PlayerList playerList = new PlayerList();
            for (int i = 2; i <= handLines.Length - 1; i++)
            {
                string handLine = handLines[i];

                if (string.IsNullOrWhiteSpace(handLine) ||
                    handLine.StartsWith("Dealer:"))
                {
                    break;
                }

                int firstSpaceIndex = handLine.IndexOf(' ');
                int openParenIndex = handLine.LastIndexOf('(');

                string seatInfo = handLine.Substring(openParenIndex + 1, handLine.Length - openParenIndex - 2);
                string[] seatData = seatInfo.Split(' ');

                int seatNumber = int.Parse(seatData[seatData.Length - 1]);
                decimal amount = decimal.Parse(seatData[1], System.Globalization.CultureInfo.InvariantCulture);
                string playerName = handLine.Substring(0, firstSpaceIndex);

                playerList.Add(new Player(playerName, amount, seatNumber));
            }

            // Get the hole cards
            for (int i = handLines.Length - 1; i >= 0; i--)
            {
                string handLine = handLines[i];

                if (string.IsNullOrWhiteSpace(handLine))
                {
                    continue;                    
                }

                if (handLine.StartsWith("Dealer:"))
                {
                    break;
                }

                //IFeelFree shows:            Kh - Tc
                if (handLine.Contains("shows:"))
                {
                    int colonIndex = handLine.IndexOf(':');

                    string holeCards = handLine.Substring(colonIndex + 1, handLine.Length - colonIndex - 1);
                    string playerName = handLine.Substring(0, handLine.IndexOf(' '));

                    Player player = playerList.First(p => p.PlayerName.Equals(playerName));
                    player.HoleCards = HoleCards.FromCards(holeCards.Replace(" ", "").Replace("-", ""));
                }
                //hellowkit didn't show hand (3h - Ad)
                else if (handLine[handLine.Length - 1] == ')' && handLine.Contains("didn't show hand"))
                {
                    int openParenIndex = handLine.LastIndexOf('(');

                    string holeCards = handLine.Substring(openParenIndex + 1, handLine.Length - openParenIndex - 2);
                    string playerName = handLine.Substring(0, handLine.IndexOf(' '));

                    Player player = playerList.First(p => p.PlayerName.Equals(playerName));
                    player.HoleCards = HoleCards.FromCards(holeCards.Replace(" ", "").Replace("-", ""));
                }
            }

            return playerList;
        }


        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            for (int i = handLines.Length - 1; i >= 0; i--)
            {
                string handLine = handLines[i];

                if (string.IsNullOrWhiteSpace(handLine))
                {
                    continue;
                }

                if (handLine.StartsWith("Dealer:"))
                {
                    break;
                }

                if (handLine.StartsWith("River ") || handLine.StartsWith("Flop ") || handLine.StartsWith("Turn "))
                {
                    int firstSpaceIndex = handLine.IndexOf(' ');
                    string board = handLine.Substring(firstSpaceIndex, handLine.Length - firstSpaceIndex);

                    return BoardCards.FromCards(board.Replace(" ", "").Replace("-", ""));
                }                
            }

            return BoardCards.ForPreflop();
        }

        protected override string ParseHeroName(string[] handlines)
        {
            throw new NotImplementedException();
        }
    }
}
