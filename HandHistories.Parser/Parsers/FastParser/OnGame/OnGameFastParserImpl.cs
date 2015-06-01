﻿using System;
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

namespace HandHistories.Parser.Parsers.FastParser.OnGame
{
    public sealed class OnGameFastParserImpl : HandHistoryParserFastImpl
    {
        private readonly SiteName _siteName;

        public override SiteName SiteName
        {
            get { return _siteName; }
        }

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
                    handHistory.Rake = decimal.Parse(handLine.Substring(12), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
                }

                //Main pot: $1.28 won by cristimanea ($1.20)
                //Side pot 1: $0.70 won by cristimanea ($0.66)
                else if (handLine[1] == 'i' || handLine[0] == 'M')
                {
                    int colonIndex = handLine.IndexOf(':');
                    int wonIndex = handLine.IndexOf(" won by", colonIndex, StringComparison.Ordinal);

                    handHistory.TotalPot += decimal.Parse(handLine.Substring(colonIndex + 2, wonIndex - colonIndex - 2), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
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

            int SeatTypeEndIndex = line.EndsWith(")") ? line.IndexOf('(', SeatTypeStartIndex) : line.Length;

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

            if (tableName.StartsWith("[SPEED]"))
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Speed);
            }

            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            // Line 3 is:
            //  Table: San Marcos [244090560] (POT_LIMIT OMAHA_HI $0.25/$0.25, Real money)

            int currencyIndex = handLines[2].IndexOf(_numberFormatInfo.CurrencySymbol, StringComparison.Ordinal);

            int slashIndex = handLines[2].IndexOf('/', currencyIndex + 1);
            int commaIndex = handLines[2].IndexOf(',', slashIndex + 1);
            var sbstring = handLines[2].Substring(currencyIndex, slashIndex - currencyIndex);

            decimal smallBlind = decimal.Parse(sbstring, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
            decimal bigBlind = decimal.Parse(handLines[2].Substring(slashIndex + 1, commaIndex - slashIndex - 1), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);

            return Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, _currency);
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            throw new NotImplementedException();
        }

        public override bool IsValidHand(string[] handLines)
        {
            return handLines[handLines.Length - 1].StartsWith("***** End of hand ") &&
                   handLines[0].StartsWith("***** History for hand") &&
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

            int startOfActionsIndex = -1;
            for (int i = 6; i < handLines.Length; i++)
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

            List<HandAction> handActions = new List<HandAction>();
            Street currentStreet = Street.Preflop;

            for (int i = startOfActionsIndex; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if (handLine.StartsWith("Seat ")) // done with actions once we reach the seat line again
                {
                    break;
                }

                bool isAllIn = handLine.EndsWith("all in]");
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
                    handLine.StartsWith("Dealing"))
                {
                    continue;
                }

                if (handLine[handLine.Length - 1] == ':') // check for Summary: line
                {
                    currentStreet = Street.Showdown;
                }

                if (currentStreet == Street.Showdown)
                {
                    // Main pot: $2.25 won by zatli74 ($2.14)
                    // Main pot: $710.00 won by alikator21 ($354.50), McCall901 ($354.50)
                    if (handLine.StartsWith("Main pot:"))
                    {
                        var nameStart = handLine.IndexOf(" won by", StringComparison.Ordinal) + 7;
                        var splitted = handLine.Substring(nameStart).Split(',');
                        foreach (var winner in splitted)
                        {
                            int openParenIndex = winner.LastIndexOf('(');
                            decimal amount = decimal.Parse(winner.Substring(openParenIndex + 1, winner.Length - openParenIndex - 2), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);

                            string playerName = winner.Substring(1, openParenIndex - 2);

                            handActions.Add(new WinningsAction(playerName, HandActionType.WINS, amount, 0));
                        }
                    }
                    // Side pot 1: $12.26 won by iplaymybest ($11.65)
                    // Side pot 2: $11.10 won by zatli74 ($5.20), Hurtl ($5.20)
                    if (handLine.StartsWith("Side pot "))
                    {
                        var nameStart = handLine.IndexOf(" won by", StringComparison.Ordinal) + 7;
                        var splitted = handLine.Substring(nameStart).Split(',');

                        var potNumber = Int32.Parse(handLine[9].ToString());
                        foreach (var winner in splitted)
                        {
                            int openParenIndex = winner.LastIndexOf('(');
                            decimal amount = decimal.Parse(winner.Substring(openParenIndex + 1, winner.Length - openParenIndex - 2), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);

                            string playerName = winner.Substring(1, openParenIndex - 2);

                            handActions.Add(new WinningsAction(playerName, HandActionType.WINS_SIDE_POT, amount, potNumber));
                        }

                    }

                    continue;
                }

                if (handLine[handLine.Length - 1] == ')' && currentStreet != Street.Showdown)
                {
                    // small-blind & big-blind lines ex:
                    //  GlassILass posts small blind ($0.25)
                    //  EvilJihnny99 posts big blind ($0.25)
                    //  19kb72 posts big blind ($6.50) [all in]

                    int openParenIndex = handLine.LastIndexOf('(');
                    decimal amount = decimal.Parse(handLine.Substring(openParenIndex + 1, handLine.Length - openParenIndex - 2), NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);

                    char blindIdentifier = handLine[openParenIndex - 10];
                    if (blindIdentifier == 'b') // big blind
                    {
                        string playerName = handLine.Substring(0, openParenIndex - 17);
                        handActions.Add(new HandAction(playerName, HandActionType.BIG_BLIND, amount, Street.Preflop));
                        continue;
                    }
                    else if (blindIdentifier == 'a')  // small-blind
                    {
                        string playerName = handLine.Substring(0, openParenIndex - 19);
                        handActions.Add(new HandAction(playerName, HandActionType.SMALL_BLIND, amount, Street.Preflop));
                        continue;
                    }
                    else if (blindIdentifier == 'e') // posts - dead
                    {
                        string playerName = handLine.Substring(0, openParenIndex - 18);
                        handActions.Add(new HandAction(playerName, HandActionType.POSTS, amount, Street.Preflop));
                        continue;
                    }

                    throw new HandActionException(handLine, "Unknown hand-line: " + handLine);
                }

                // Check for folds & checks
                if (handLine[handLine.Length - 1] == 's')
                {
                    if (handLine[handLine.Length - 2] == 'd') // folds
                    {
                        string playerName = handLine.Substring(0, handLine.Length - 6);
                        handActions.Add(new HandAction(playerName, HandActionType.FOLD, 0, currentStreet));
                    }
                    else if (handLine[handLine.Length - 2] == 'k') // checks
                    {
                        string playerName = handLine.Substring(0, handLine.Length - 7);
                        handActions.Add(new HandAction(playerName, HandActionType.CHECK, 0, currentStreet));
                    }
                    continue;
                }
                else
                {
                    //Old format
                    //{playername} calls $18.00

                    //New format 
                    //{playername} calls $13

                    int currencyIndex = handLine.IndexOf(_numberFormatInfo.CurrencySymbol, StringComparison.Ordinal);

                    int valueEndIndex = handLine.IndexOf(' ', currencyIndex);
                    if (valueEndIndex == -1)
                    {
                        valueEndIndex = handLine.Length;
                    }

                    char actionIdentifier = handLine[currencyIndex - 3];

                    var amountstr = handLine.Substring(currencyIndex, valueEndIndex - currencyIndex);
                    string playerName;
                    decimal amount = decimal.Parse(amountstr, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo);
                    switch (actionIdentifier)
                    {
                        case 'l': // calls
                            playerName = handLine.Substring(0, currencyIndex - 7);
                            if (isAllIn)
                            {
                                handActions.Add(new AllInAction(playerName, amount, currentStreet, false));   
                            }
                            else
                            {
                                handActions.Add(new HandAction(playerName, HandActionType.CALL, amount, currentStreet));
                            }
                            break;
                        case 'e': // raises
                            // ex: zatli74 raises $1.00 to $1.00
                            playerName = handLine.Substring(0, currencyIndex - 8);
                            if (isAllIn)
                            {
                                handActions.Add(new AllInAction(playerName, amount, currentStreet, true));   
                            }
                            else
                            {
                                handActions.Add(new HandAction(playerName, HandActionType.RAISE, amount, currentStreet));
                            }
                            break;
                        case 't': // bets
                            playerName = handLine.Substring(0, currencyIndex - 6);
                            if (isAllIn)
                            {
                                handActions.Add(new AllInAction(playerName, amount, currentStreet, false));   
                            }
                            else
                            {
                                handActions.Add(new HandAction(playerName, HandActionType.BET, amount, currentStreet));    
                            }
                            break;
                    }
                    continue;
                }

                throw new HandActionException(handLine, "Unknown hand-line: " + handLine);
        }

            return FixUncalledBets(handActions, handHistory.TotalPot, handHistory.Rake);
        }

        private List<HandAction> FixUncalledBets(List<HandAction> handActions, decimal? totalPot, decimal? rake)
        {
            // this fix only takes place when the TotalPot - Rake != Winnings
            if (totalPot != null && rake != null)
            {
                var wagered = handActions.Where(a => !a.IsWinningsAction).Sum(a => a.Amount);
                if (totalPot + wagered == 0m)
                {
                    return handActions;
                }

                // the player we need to return the money, is always the player who invested the most ( exclude ante + post )
                var playerToReturnTo = handActions.Where(a => a.IsGameAction && !a.HandActionType.Equals(HandActionType.POSTS) && !a.HandActionType.Equals(HandActionType.ANTE))
                    .GroupBy(a => a.PlayerName)
                    .Select(p => new
                    {
                        PlayerName = p.Key,
                        Invested = p.Sum(x => x.Amount)
                    }).OrderBy(x => x.Invested).First().PlayerName;

                // take a look at this players last action
                var lastAction = handActions.Last(a => a.IsGameAction && a.PlayerName.Equals(playerToReturnTo));
                var playerInvested = handActions.Where(a => a.Street == lastAction.Street
                                                      && !a.HandActionType.Equals(HandActionType.POSTS) && !a.HandActionType.Equals(HandActionType.ANTE) // ignore dead money
                                                      && a.PlayerName.Equals(playerToReturnTo)).Sum(a => a.Amount);

                // the amount to return is the largest invested amount on the lastaction.street from the other players
                var returnAmount = playerInvested - handActions.Where(a => a.Street == lastAction.Street
                    && !a.HandActionType.Equals(HandActionType.POSTS) && !a.HandActionType.Equals(HandActionType.ANTE) // make sure to ignore dead money
                    && !a.PlayerName.Equals(playerToReturnTo)).GroupBy(a => a.PlayerName)
                    .Select(p => new
                    {
                        PlayerName = p.Key,
                        Invested = p.Sum(x => x.Amount)
                    }).OrderBy(x => x.Invested).First().Invested;


                // if the return amount is bigger than the last betting, the hand is corrupt -> don't fix anything
                if (Math.Abs(returnAmount) <= Math.Abs(playerInvested) && returnAmount != 0m)
                    handActions.Add(new HandAction(playerToReturnTo, HandActionType.UNCALLED_BET, returnAmount, Street.Showdown));

            }

            return handActions;
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

                playerList.Add(new Player(name, decimal.Parse(amount, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, _numberFormatInfo), seatNumber));
            }

            // Parse the player showdown hole-cards

            for (int i = handLines.Length - 2; i >= 0; i--)
            {
                // Loop backward looking for lines like:
                //  Seat 4: mr dark hor ($37.87), net: -$15.25, [Ts, 3s]

                string handLine = handLines[i];

                if (handLine.StartsWith("Seat") == false)
                {
                    break;
                }

                if (handLine.EndsWith("]") == false)
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

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            string boardCards = string.Empty;

            for (int i = 4; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if (handLine.StartsWith("---") == false ||
                    handLines.Length == 3 ||
                    handLine.EndsWith("]") == false)
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
