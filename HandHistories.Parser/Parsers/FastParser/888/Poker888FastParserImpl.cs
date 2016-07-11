using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Time;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.Strings;
using System.Globalization;

namespace HandHistories.Parser.Parsers.FastParser._888
{
    public sealed class Poker888FastParserImpl : HandHistoryParserFastImpl
    {
        static readonly CultureInfo invariant = CultureInfo.InvariantCulture;

        public override SiteName SiteName
        {
            get { return SiteName.Pacific; }
        }

        public override bool RequiresAllInDetection
        {
            get { return true; }
        }

        public override bool RequiresTotalPotCalculation
        {
            get { return true; }
        }

        private static readonly NumberFormatInfo NumberFormatInfo = new NumberFormatInfo
        {
            NegativeSign = "-",
            CurrencyDecimalSeparator = ".",
            CurrencyGroupSeparator = ",",
            CurrencySymbol = "$"
        };

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            rawHandHistories = rawHandHistories.Replace("\r", "");

            //This was causing an OOM exception so used LazyStringSplit
            //List<string> splitUpHands = rawHandHistories.Split(new char[] {'▄'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            //return splitUpHands.Where(s => s.Equals("\r\n") == false);

            return rawHandHistories.LazyStringSplit("\n\n").Where(s => string.IsNullOrWhiteSpace(s) == false && s.Equals("\r\n") == false);
        }

        private static readonly Regex DealerPositionRegex = new Regex(@"(?<=Seat )\d+", RegexOptions.Compiled);
        protected override int ParseDealerPosition(string[] handLines)
        {
            return Int32.Parse(DealerPositionRegex.Match(handLines[4]).Value);
        }

        private static readonly Regex DateLineRegex = new Regex(@"\d+ \d+ \d+ \d+\:\d+\:\d+", RegexOptions.Compiled);
        private static readonly Regex DateRegex = new Regex(@"(\d+) (\d+) (\d+) ", RegexOptions.Compiled);
        protected override DateTime ParseDateUtc(string[] handLines)
        {
            //Date looks like: 04 02 2012 23:59:48
            string dateString = DateLineRegex.Match(handLines[2]).Value;
            //Change string so it becomes 2012-02-04 23:59:48
            dateString = DateRegex.Replace(dateString, "$3-$2-$1 ");

            DateTime dateTime = DateTime.Parse(dateString);

            DateTime utcTime = TimeZoneUtil.ConvertDateTimeToUtc(dateTime, TimeZoneType.GMT);

            return utcTime;
        }

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            return PokerFormat.CashGame;
        }

        private static readonly Regex HandIdRegex = new Regex(@"(?<=#Game No \: )\d+", RegexOptions.Compiled);
        protected override long ParseHandId(string[] handLines)
        {
            return long.Parse(HandIdRegex.Match(handLines[0]).Value);
        }

        protected override long ParseTournamentId(string[] handLines)
        {
            throw new NotImplementedException();
        }

        private static readonly Regex TableNameRegex = new Regex(@"(?<=Table ).*$", RegexOptions.Compiled);
        protected override string ParseTableName(string[] handLines)
        {
            //"Table Athens 10 Max (Real Money)" -> "Athens"
            var tableName = TableNameRegex.Match(handLines[3]).Value;
            tableName = tableName.Substring(0, tableName.Length - 19).TrimEnd();
            return tableName;
        }

        private static readonly Regex NumPlayersRegex = new Regex(@"(?<=Total number of players : )\d+", RegexOptions.Compiled);
        protected override SeatType ParseSeatType(string[] handLines)
        {
            int seatCount = Int32.Parse(NumPlayersRegex.Match(handLines[5]).Value);

            if (seatCount <= 2)
            {
                return SeatType.FromMaxPlayers(2);
            }
            else if (seatCount <= 6)
            {
                return SeatType.FromMaxPlayers(6);
            }
            else if (seatCount <= 9)
            {
                return SeatType.FromMaxPlayers(9);
            }
            else
            {
                return SeatType.FromMaxPlayers(10);
            }
        }

        private static readonly Regex GameTypeRegex = new Regex(@"(?<=Blinds ).*(?= - )", RegexOptions.Compiled);
        protected override GameType ParseGameType(string[] handLines)
        {
            string gameTypeString = GameTypeRegex.Match(handLines[2]).Value;
            gameTypeString = gameTypeString.Replace(" Jackpot table", "");
            switch (gameTypeString)
            {
                case "No Limit Holdem":
                case "No Limit Holdem Jackpot table":
                    return GameType.NoLimitHoldem;
                case "Fix Limit Holdem":
                    return GameType.FixedLimitHoldem;
                case "Pot Limit Holdem":
                    return GameType.PotLimitHoldem;     
                case "Pot Limit Omaha":
                    return GameType.PotLimitOmaha;      
                case "No Limit Omaha":
                    return GameType.NoLimitOmaha;         
                case "Pot Limit OmahaHL":
                    return GameType.PotLimitOmahaHiLo;
                default:                    
                    throw new NotImplementedException("Unrecognized game type " + gameTypeString);
            }
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            // 888 does not store any information about the tabletype in the hand history

            // we assume the table is push or fold if
            // - there is at least one player with exactly 5bb
            // - the average stack is < 17.5bb 
            // - at least two players have < 10bb
            // - there is no player with > 20bb

            // the min buyin for standard table is > 30bb, so this should work in most cases
            // furthermore if on a regular table the average stack is < 17.5, the play is just like on a push fold table and vice versa
            bool isjackPotTable = handLines[2].Contains(" Jackpot table");
           
            var playerList = ParsePlayers(handLines);
            var limit = ParseLimit(handLines);

            var tableStack = 0m;
            var playersBelow10bb = 0;
            var playersAbove20bb = 0;
            foreach (Player player in playerList)
            {
                tableStack += player.StartingStack;
                if (player.StartingStack / limit.BigBlind == 5m) return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold);
                if (player.StartingStack / limit.BigBlind <= 10m) playersBelow10bb++;
                if (player.StartingStack / limit.BigBlind > 29m) playersAbove20bb++;

                if (playersBelow10bb > 1) return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold);
            }

            if (playersAbove20bb == 0) return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold);

            if (tableStack / limit.BigBlind / playerList.Count <= 17.5m)
            {
                if (isjackPotTable)
                {
                    return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold, TableTypeDescription.Jackpot);
                }
                return TableType.FromTableTypeDescriptions(TableTypeDescription.PushFold);
            }
            if (isjackPotTable)
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Jackpot);
            }
            else
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
            }
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            string line = handLines[2];

            int LimitEndIndex = line.IndexOf(" Blinds", StringComparison.Ordinal);
            string limitString = line.Remove(LimitEndIndex)
                .Replace(" ", "")
                .Replace("$", "")
                ;

            int splitIndex = limitString.IndexOf('/');

            string lowLimitString = limitString.Remove(splitIndex);
            string highLimitString = limitString.Substring(splitIndex + 1);

            decimal lowLimit = ParseAmount(lowLimitString);
            decimal highLimit = ParseAmount(highLimitString);

            return Limit.FromSmallBlindBigBlind(lowLimit, highLimit, Currency.USD);
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            throw new NotImplementedException();
        }

        public override bool IsValidHand(string[] handLines)
        {
            return handLines[handLines.Length - 1].Contains(" collected ");
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;

            var seatedPlayersLine = handLines[5];
            if (seatedPlayersLine[seatedPlayersLine.Length - 1] == '1')
            {
                isCancelled = true;
            }

            return IsValidHand(handLines);
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            Street currentStreet = Street.Preflop;

            List<HandAction> handActions = new List<HandAction>();
            
            for (int i = 6; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if (currentStreet == Street.Preflop)
                {
                    if (handLine.IndexOf(':') != -1)
                    {
                        continue;
                    }
                    else if (handLine[0] == 'D' && handLine.StartsWith("Dealt "))
                    {
                        continue;
                    }
                }

                if (handLine[0] == '*')
                {
                    if (handLine[3] == 'S')
                    {
                        currentStreet = Street.Showdown;
                        continue;
                    }

                    switch (handLine[11])
                    {
                        case 'r':
                            currentStreet = Street.River;
                            break;
                        case 'f':
                            currentStreet = Street.Flop;
                            break;
                        case 't':
                            currentStreet = Street.Turn;
                            break;
                    }
                    
                    continue;
                }

                if (currentStreet == Street.Showdown)
                {
                    // ignore lines such as:
                    //  OprahTiltfre did not show his hand
                    if (handLine[handLine.Length - 1] != ']')
                    {
                        continue;                            
                    }

                    int openSquareIndex = handLine.LastIndexOf('[');

                    // winnings hands have numbers such as:
                    //  OprahTiltfre collected [ $2,500 ]
                    if (char.IsDigit(handLine[handLine.Length - 3]))
                    {                        
                        string amountString = handLine.Substring(openSquareIndex + 1, handLine.Length - openSquareIndex - 1 - 1);
                        decimal amount = ParseAmount(amountString);
                        
                        string playerName = handLine.Substring(0, openSquareIndex - 11);

                        handActions.Add(new WinningsAction(playerName, HandActionType.WINS, amount, 0, i));
                        continue;                        
                    }

                    string action = handLine.Substring(openSquareIndex - 6, 5);
                    if (action.Equals("shows"))
                    {
                        string playerName = handLine.Substring(0, openSquareIndex - 7);
                        handActions.Add(new HandAction(playerName, HandActionType.SHOW, 0, currentStreet, i));
                        continue;
                    }
                    else if (action.Equals("mucks"))
                    {
                        string playerName = handLine.Substring(0, openSquareIndex - 7);
                        handActions.Add(new HandAction(playerName, HandActionType.MUCKS, 0, currentStreet, i));
                        continue;
                    }

                    throw new HandActionException(handLine, "Unparsed");
                }

                if (handLine[handLine.Length - 1] == ']')
                {
                    int openSquareIndex = handLine.LastIndexOf('[');
                    string amountString = handLine.Substring(openSquareIndex + 1, handLine.Length - openSquareIndex - 1 - 1);

                    decimal amount = ParseAmount(amountString);

                    string action = handLine.Substring(openSquareIndex - 8, 7);

                    if (currentStreet == Street.Preflop)
                    {
                        if (action.Equals("l blind", StringComparison.Ordinal)) // small blind
                        {
                            string playerName = handLine.Substring(0, openSquareIndex - 19);
                            amount = ParseAmount(amountString);
                            handActions.Add(new HandAction(playerName, HandActionType.SMALL_BLIND, amount, currentStreet, i));
                            continue;
                        }
                        else if (action.Equals("g blind", StringComparison.Ordinal)) // big blind
                        {
                            string playerName = handLine.Substring(0, openSquareIndex - 17);
                            amount = ParseAmount(amountString);
                            handActions.Add(new HandAction(playerName, HandActionType.BIG_BLIND, amount, currentStreet, i));
                            continue;
                        }
                        else if(action.Equals("d blind", StringComparison.Ordinal))//dead blind
                        {
                            string playerName = handLine.Substring(0, openSquareIndex - 18);
                            amount = ParseDeadBlindAmount(amountString);
                            handActions.Add(new HandAction(playerName, HandActionType.POSTS, amount, currentStreet, i));
                            continue;
                        }
                    }

                    amount = ParseAmount(amountString);
                    
                    if (action.EndsWith("raises"))
                    {
                        string playerName = handLine.Substring(0, openSquareIndex - 8);
                        handActions.Add(new HandAction(playerName, HandActionType.RAISE, amount, currentStreet, i));
                        continue;
                    }
                    else if (action.EndsWith("bets"))
                    {
                        string playerName = handLine.Substring(0, openSquareIndex - 6);
                        handActions.Add(new HandAction(playerName, HandActionType.BET, amount, currentStreet, i));
                        continue;
                    }
                    else if (action.EndsWith("calls"))
                    {
                        string playerName = handLine.Substring(0, openSquareIndex - 7);
                        handActions.Add(new HandAction(playerName, HandActionType.CALL, amount, currentStreet, i));
                        continue;
                    }
                }
                else if (handLine.FastEndsWith("folds"))
                {
                    string playerName = handLine.Substring(0, handLine.Length - 6);
                    handActions.Add(new HandAction(playerName, HandActionType.FOLD, 0, currentStreet, i));
                    continue;
                }
                else if (handLine.EndsWith("checks"))
                {
                    string playerName = handLine.Substring(0, handLine.Length - 7);
                    handActions.Add(new HandAction(playerName, HandActionType.CHECK, 0, currentStreet, i));
                    continue;
                }                

                throw new HandActionException(handLine, "Unknown handline.");
            }

            return FixUncalledBets(handActions, handLines.Length, null, null);
        }

        private static decimal ParseAmount(string amountString)
        {
            // this split helps us parsing dead posts like [$0.10 + $0.05]
            var splittedAmounts = amountString.Split('+');
            var result = 0.0m;

            foreach (var amount in splittedAmounts)
            {
                result += decimal.Parse(amount, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, NumberFormatInfo);
            }

            return result;
        }

        static decimal ParseDeadBlindAmount(string amountString)
        {
            int plusIndex = amountString.IndexOf('+');
            string amountStr1 = amountString.Remove(plusIndex);
            string amountStr2 = amountString.Substring(plusIndex + 1);

            decimal amount1 = ParseAmount(amountStr1);
            decimal amount2 = ParseAmount(amountStr2);

            return amount1 + amount2;
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            int seatCount = Int32.Parse(NumPlayersRegex.Match(handLines[5]).Value);

            PlayerList playerList = new PlayerList();

            for (int i = 0; i < seatCount; i++)
            {
                string handLine = handLines[6 + i];

                // Expected format:
                //  Seat 1: Velmonio ( $1.05 )

                int colonIndex = handLine.IndexOf(':');
                int openParenIndex = handLine.IndexOf('(');

                int seat = int.Parse(handLine.Substring(5, colonIndex - 5));
                string playerName = handLine.Substring(colonIndex + 2, openParenIndex - colonIndex - 3);
                decimal amount = ParseAmount(handLine.Substring(openParenIndex + 3, handLine.Length - openParenIndex - 3 - 2));

                playerList.Add(new Player(playerName, amount, seat));
            }

            // Add hole-card info
            for (int i = handLines.Length - 2; i >= 0; i--)
            {
                string handLine = handLines[i];

                if (handLine[0] == '*')
                {
                    break;                    
                }

                if (handLine.EndsWith("]") &&
                    char.IsDigit(handLine[handLine.Length - 3]) == false)
                {
                    // lines such as:
                    //  slyguyone2 shows [ Jd, As ]

                    int openSquareIndex = handLine.IndexOf('[');

                    string cards = handLine.Substring(openSquareIndex + 2, handLine.Length - openSquareIndex - 2 - 2);
                    HoleCards holeCards = HoleCards.FromCards(cards.Replace(",", "").Replace(" ", ""));

                    string playerName = handLine.Substring(0, openSquareIndex - 7);

                    Player player = playerList.First(p => p.PlayerName.Equals(playerName));
                    player.HoleCards = holeCards;
                }
            }

                return playerList;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            string boardCards = string.Empty;

            for (int i = 0; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                if (handLine[0] != '*')
                {
                    continue;                    
                }

                if (handLine[3] != 'D')
                {
                    continue;
                }

                int openSquareIndex;
                switch (handLine[11])
                {
                    case 'r':
                        openSquareIndex = 20;
                        break;
                    default:
                        openSquareIndex = 19;
                        break;                        
                }

                string cards = handLine.Substring(openSquareIndex + 2, handLine.Length - openSquareIndex - 2 - 2);

                boardCards += cards.Replace(",", "").Replace(" ", "");
            }

            return BoardCards.FromCards(boardCards);
        }

        protected override string ParseHeroName(string[] handlines)
        {
            const string DealText = "Dealt to ";
            for (int i = 0; i < handlines.Length; i++)
            {
                if (handlines[i][0] == 'D' && handlines[i].StartsWith(DealText))
                {
                    string line = handlines[i];
                    int startindex = DealText.Length;
                    int endindex = line.LastIndexOf('[') - 1;
                    return line.Substring(startindex, endindex - startindex);
                }
            }
            return null;
        }

        private List<HandAction> FixUncalledBets(List<HandAction> handActions, int actionNumber, decimal? totalPot, decimal? rake)
        {
            // Pacific does not correctly return uncalled bets, sometimes declaring them as winnings
            // as we calculate the rake manually, we need to make sure uncalledbets are declared correctly

            // this fix only takes place when the TotalPot - Rake != Winnings
            if (totalPot != null && rake != null)
            {
                if (totalPot - rake == handActions.Where(a => a.IsWinningsAction).Sum(a => a.Amount))
                    return handActions;
            }

            if (handActions.Sum(a => a.Amount) == 0m)
                return handActions;


            var playerActions = handActions.Where(a => !a.IsWinningsAction
                                                  && !a.HandActionType.Equals(HandActionType.SHOW)
                                                  && !a.HandActionType.Equals(HandActionType.MUCKS))
                                         .GroupBy(p => p.PlayerName)
                                         .Select(s => s.Select(a => a).ToList());

            HandAction handActionToAdd = null;
            foreach (var actions in playerActions)
            {
                var lastAction = actions[actions.Count - 1];

                if (lastAction.HandActionType == HandActionType.BET || lastAction.HandActionType == HandActionType.RAISE)
                {
                    var totalInvestedAmount = handActions.Where(a => a.PlayerName.Equals(lastAction.PlayerName) && !a.IsWinningsAction).Sum(a => a.Amount);

                    var deadMoneyAction = handActions.FirstOrDefault(a => a.PlayerName.Equals(lastAction.PlayerName) && a.HandActionType.Equals(HandActionType.POSTS));

                    if (deadMoneyAction != null && deadMoneyAction.Amount < handActions.First(a => a.HandActionType.Equals(HandActionType.BIG_BLIND)).Amount)
                    {
                        totalInvestedAmount -= handActions.First(a => a.HandActionType.Equals(HandActionType.SMALL_BLIND)).Amount;
                    }


                    var totalInvestedAmountsByOtherPlayers = handActions.Where(a => a.PlayerName != lastAction.PlayerName && !a.IsWinningsAction)
                                                                        .GroupBy(a => a.PlayerName)
                                                                        .Select(p => new
                                                                                     {
                                                                                         PlayerName = p.Key,
                                                                                         Invested = p.Sum(x => x.Amount)
                                                                                     });

                    var totalInvestedAmountOtherPlayer = 0.0m;
                    foreach (var investedByPlayer in totalInvestedAmountsByOtherPlayers)
                    {
                        var invested = investedByPlayer.Invested;
                        deadMoneyAction = handActions.FirstOrDefault(a => a.PlayerName.Equals(investedByPlayer.PlayerName) && a.HandActionType.Equals(HandActionType.POSTS));

                        if (deadMoneyAction != null && deadMoneyAction.Amount < handActions.First(a => a.HandActionType.Equals(HandActionType.BIG_BLIND)).Amount)
                        {
                            invested -= handActions.First(a => a.HandActionType.Equals(HandActionType.SMALL_BLIND)).Amount;
                        }

                        if (invested < totalInvestedAmountOtherPlayer)
                        {
                            totalInvestedAmountOtherPlayer = invested;
                        }
                    }


                    if (totalInvestedAmountOtherPlayer > totalInvestedAmount)
                    {
                        var uncalledBet = Math.Abs(totalInvestedAmount - totalInvestedAmountOtherPlayer);

                        // only add this if we don't have a similar WIN line
                        var winAction = handActions.FirstOrDefault(h => h.PlayerName == lastAction.PlayerName
                                                                     && h.HandActionType == HandActionType.WINS
                                                                     && h.Amount == uncalledBet);

                        if (winAction != null && handActions.Count(h => h.IsWinningsAction) > 1)
                        {
                            handActions.Remove(winAction);
                        }
                        handActionToAdd = (new HandAction(lastAction.PlayerName, HandActionType.UNCALLED_BET, uncalledBet, Street.Showdown, actionNumber));
                    }
                }
            }

            // only add the uncalled bet if we had a caller
            if (handActionToAdd != null)
                handActions.Add(handActionToAdd);

            return handActions;
        }
    }
}
