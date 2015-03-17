using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandHistories.Writer.Writer.PokerStars
{
    public class OnGameHandWriter : IHandWriter
    {
        const string NEWLINE = "\r\n";

        static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public SiteName SiteName
        {
            get { return SiteName.OnGame; }
        }

        public string Write(HandHistory hand)
        {
            SetActionNumbers(hand);

            StringBuilder handText = new StringBuilder();

            handText.Append(WriteGameInfo(hand) + NEWLINE);

            handText.Append(WritePlayerList(hand) + NEWLINE);

            handText.Append(WritePostingActions(hand) + NEWLINE);

            handText.Append("---" + NEWLINE);

            handText.Append("Dealing pocket cards" + NEWLINE);

            handText.Append(WriteHandActions(hand) + NEWLINE);

            handText.Append(WriteSummary(hand));

            return handText.ToString();
        }

        private void SetActionNumbers(HandHistory hand)
        {
            int actionNumber = 0;
            foreach (var action in hand.HandActions)
            {
                action.ActionNumber = actionNumber++;
            }
        }

        private string WriteHandActions(HandHistory hand)
        {
            Street previousStreet = Street.Null;
            Street currentStreet = Street.Preflop;
            var lines = new List<string>();

            foreach (var action in hand.HandActions
                .Where(p => !p.IsBlinds && p.HandActionType != HandActionType.POSTS))
            {
                if (action.Street != currentStreet)
                {
                    previousStreet = currentStreet;
                    currentStreet = action.Street;

                    var line = GetStreetLine(currentStreet, hand.ComumnityCards);
                    if (line != null)
                    {
                        lines.Add(line);
                    }
                    else
                    {
                        var lastAction = hand.HandActions
                            .Where(p => p.IsGameAction)
                            .LastOrDefault();

                        if (lastAction.HandActionType == HandActionType.CALL || 
                            lastAction.HandActionType == HandActionType.CHECK)
                        {
                            if (lastAction.Street == Street.Preflop)
                            {
                                //All in hand
                                lines.Add(GetStreetLine(Street.Flop, hand.ComumnityCards));
                                lines.Add(GetStreetLine(Street.Turn, hand.ComumnityCards));
                                lines.Add(GetStreetLine(Street.River, hand.ComumnityCards));
                            }
                            else if (lastAction.Street == Street.Flop)
                            {
                                //All in hand
                                lines.Add(GetStreetLine(Street.Turn, hand.ComumnityCards));
                                lines.Add(GetStreetLine(Street.River, hand.ComumnityCards));
                            }
                            else if (lastAction.Street == Street.Turn)
                            {
                                //All in hand
                                lines.Add(GetStreetLine(Street.River, hand.ComumnityCards));
                            }
                        }
                    }
                }

                if (action.IsGameAction)
                {
                    lines.Add(WriteGameActionLine(action, hand));
                }
            }

            return string.Join(NEWLINE, lines);
        }

        private string WriteShowDownAction(HandAction action, HandHistory hand)
        {
            string format;
            switch (action.HandActionType)
            {
                case HandActionType.MUCKS:
                    var playerShowDownActions = hand.HandActions
                        .Street(Street.Showdown)
                        .Player(action.PlayerName);

                    bool playerWins = playerShowDownActions
                        .FirstOrDefault(p => p.IsWinningsAction) != null;

                    if (playerWins)
                    {
                        format = "{0}: doesn't show hand ";
                    }
                    else
                    {
                        format = "{0}: mucks hand ";
                    }
                    break;
                case HandActionType.SHOW:
                    return GetShowLine(action, hand);
                case HandActionType.WINS:
                    bool sidePotExist = hand.HandActions
                        .FirstOrDefault(p => p.HandActionType == HandActionType.WINS_SIDE_POT ||
                        p.HandActionType == HandActionType.TIES_SIDE_POT) != null;

                    if (sidePotExist)
                    {
                        format = "{0} collected ${1} from main pot";
                    }
                    else
                    {
                        format = "Main pot: ${1} won by {0} ({1})";
                    }
                    break;
                case HandActionType.WINS_SIDE_POT:
                    format = "{0} collected ${1} from side pot";
                    break;
                default:
                    throw new NotImplementedException("No support for action: " + action.HandActionType);
            }

            return string.Format(format,
                action.PlayerName,
                Math.Abs(action.Amount).ToString(InvariantCulture));
        }

        private string GetShowLine(HandAction action, HandHistory hand)
        {
            var pocket = string.Join(" ", hand.Players[action.PlayerName].HoleCards.Select(p => p.ToString()));

            return string.Format("{0}: shows [{1}] ({2})",
                action.PlayerName,
                pocket,
                "a pair of Jacks");
        }

        private string GetStreetLine(Street currentStreet, BoardCards board)
        {
            switch (currentStreet)
            {
                case Street.Flop:
                    return string.Format("--- Dealing flop [{0}, {1}, {2}]", 
                        board[0],
                        board[1],
                        board[2]);
                case Street.Turn:
                    return string.Format("--- Dealing turn [{3}]",
                        board[0],
                        board[1],
                        board[2],
                        board[3]);
                case Street.River:
                    return string.Format("--- Dealing river [{4}]",
                        board[0],
                        board[1],
                        board[2],
                        board[3],
                        board[4]);
                case Street.Showdown:
                    return null;
                default:
                    throw new ArgumentException("Unknown street: " + currentStreet.ToString());
            }
        }

        private string WriteSummary(HandHistory hand)
        {
            var lines = new List<string>();
            lines.Add("---");
            lines.Add("Summary:");

            var winAction = hand.HandActions.First(p => p.IsWinningsAction);

            var potLine = string.Format("Main pot: ${0} won by {1} (${2})",
                hand.TotalPot.Value.ToString(InvariantCulture),
                winAction.PlayerName,
                winAction.Amount.ToString(InvariantCulture));
                
            lines.Add(potLine);

            lines.Add(string.Format("Rake taken: ${0}",
                hand.Rake.Value.ToString(InvariantCulture)));

            foreach (var player in hand.Players)
            {
                lines.Add(GetPlayerSummaryLine(player, hand));
            }

            lines.Add(string.Format("***** End of hand R{0} *****",
                GetHandIdString(hand.HandId)));

            return string.Join(NEWLINE, lines);
        }

        private string GetPlayerSummaryLine(Player player, HandHistory hand)
        {
            string summaryFormat = "Seat {0}: {1} (${2}), net: {3}";

            var playerActions = hand.HandActions
                .Player(player.PlayerName);

            bool isWinner = playerActions
                .FirstOrDefault(p => p.IsWinningsAction) != null;

            decimal deltaStack = 0;

            if (isWinner)
            {
                Dictionary<string, decimal> playerWagers = hand.Players
                    .ToDictionary(p => p.PlayerName, p => 0m);

                foreach (var action in hand.HandActions
                    .Where(p => p.IsGameAction))
	            {
		            playerWagers[action.PlayerName] += action.Amount;
                }

                decimal calledAmount = playerWagers
                    .OrderBy(p => p.Value)
                    .Skip(1)
                    .First()
                    .Value;

                decimal totalPutInPot = playerActions
                    .Sum(p => p.Amount);

                decimal wonAmount = playerActions
                    .Where(p => p.IsWinningsAction)
                    .Sum(p => p.Amount);

                deltaStack = wonAmount - Math.Abs(calledAmount);
            }
            else
            {
                deltaStack = playerActions
                .Sum(p => p.Amount);
            }
            

            return string.Format(summaryFormat,
                player.SeatNumber,
                player.PlayerName,
                (player.StartingStack + deltaStack).ToString(InvariantCulture),
                (deltaStack > 0 ? "+$" : "-$") + Math.Abs(deltaStack).ToString("F2", InvariantCulture));

            var preflopActions = hand.HandActions.Street(Street.Preflop);
            //var playerActions = hand.HandActions.Where(p => p.PlayerName == player.PlayerName);
            var playerActionsPF = playerActions.Where(p => p.Street == Street.Preflop);

            string seat = string.Format("Seat {0}: {1} ",
                player.SeatNumber,
                player.PlayerName);

            string position = "";
            if (hand.DealerButtonPosition == player.SeatNumber)
	        {
		        position = "(button) ";
            }

            var blindAction = preflopActions
                .FirstOrDefault(p => (p.HandActionType == HandActionType.BIG_BLIND || p.HandActionType == HandActionType.SMALL_BLIND) && p.PlayerName == player.PlayerName);
            
            if (blindAction != null)
	        {
                if (blindAction.HandActionType == HandActionType.SMALL_BLIND)
                {
                    if (hand.Players.Count(p => !p.IsSittingOut) == 2)
                    {
                        position = "(button) (small blind) ";
                    }
                    else
                    {
                        position = "(small blind) ";
                    }
                }
                else if (blindAction.HandActionType == HandActionType.BIG_BLIND)
                {
                    position = "(big blind) ";
                }
            }

            bool folded = playerActions
                .FirstOrDefault(p => p.HandActionType == HandActionType.FOLD) != null;

            if (folded)
            {
                bool didBet = playerActionsPF
                    .FirstOrDefault(p => p.IsAggressiveAction || p.HandActionType == HandActionType.BIG_BLIND || p.HandActionType == HandActionType.SMALL_BLIND) != null;

                string FoldStreet = GetFoldStreet(playerActions.FirstOrDefault(p => p.HandActionType == HandActionType.FOLD).Street);

                return seat + position + "folded " + FoldStreet + (didBet ? "" : " (didn't bet)");
            }
            else
            {
                var winAction = playerActions
                    .FirstOrDefault(p => p.IsWinningsAction);

                var showsHand = playerActions
                    .FirstOrDefault(p => p.HandActionType == HandActionType.SHOW);

                if (winAction != null)
                {
                    if (showsHand != null)
                    {
                        return string.Format("{0}{1}showed [{2}] and won (${3}) with {4}",
                        seat,
                        position,
                        string.Join(" ", player.HoleCards.Select(p => p.ToString())),
                        winAction.Amount.ToString(InvariantCulture),
                        "a pair of Jacks");
                    }
                    else
                    {
                        return seat + position + string.Format("collected (${0})", winAction.Amount.ToString(InvariantCulture));
                    }
                }
                else
                {
                    if (showsHand != null)
                    {
                        return string.Format("{0}{1}showed [{2}] and lost with {3}",
                        seat,
                        position,
                        string.Join(" ", player.HoleCards.Select(p => p.ToString())),
                        "a pair of Jacks");
                    }
                    else
                    {
                        return string.Format("{0}{1} mucked",
                        seat,
                        position);
                    }
                }
            }                   
        }

        private string GetFoldStreet(Street Street)
        {
            switch (Street)
            {
                case Street.Preflop:
                    return "before Flop";
                case Street.Flop:
                    return "on the Flop";
                case Street.Turn:
                    return "on the Turn";
                case Street.River:
                    return "on the River";
                default:
                    throw new ArgumentException("Invalid Street");
            }
        }

        static string WriteGameActionLine(HandAction action, HandHistory hand)
        {
            string format;
            switch (action.HandActionType)
            {
                case HandActionType.FOLD:
                    format = "{0} folds";
                    break;
                case HandActionType.CALL:
                    format = "{0} calls ${1}";
                    break;
                case HandActionType.CHECK:
                    format = "{0} checks";
                    break;
                case HandActionType.RAISE:
                    format = GetRaiseFormatString(action, hand);
                    break;
                case HandActionType.BET:
                    format = "{0} bets ${1}";
                    break;
                default:
                    throw new NotImplementedException("No support for action: " + action.HandActionType);
            }
            return string.Format(format + (action.IsAllIn ? " [all in] " : ""), 
                action.PlayerName, 
                Math.Abs(action.Amount).ToString(InvariantCulture));
        }

        private static string GetRaiseFormatString(HandAction action, HandHistory hand)
        {
            var raiseToAmount = GetRaiseToAmount(action, hand);

            return string.Format("{0} raises ${1} to ${2}",
                "{0}",
                Math.Abs(action.Amount).ToString(InvariantCulture),
                raiseToAmount.ToString(InvariantCulture));
        }

        private static decimal GetRaiseAmount(HandAction action, HandHistory hand)
        {
            var streetActions = hand.HandActions
                .Street(action.Street);

            var playerPutInPot = streetActions
                .Player(action)
                .Where(p => p.ActionNumber < action.ActionNumber)
                .Sum(p => p.Amount);

            var playerWagers = new Dictionary<string, decimal>();

            foreach (var item in streetActions
                .Where(p => p.ActionNumber < action.ActionNumber))
            {
                if (!playerWagers.ContainsKey(item.PlayerName))
                {
                    playerWagers.Add(item.PlayerName, item.Amount);
                }
                else
                {
                    playerWagers[item.PlayerName] += item.Amount;
                }
            }

            var amountToCall = playerWagers.Min(p => p.Value);

            var raiseTo = action.Amount + playerPutInPot;

            return Math.Abs(raiseTo - amountToCall);
        }

        private static decimal GetRaiseToAmount(HandAction action, HandHistory hand)
        {
            var streetActions = hand.HandActions
                .Street(action)
                .Player(action);

            var amount = streetActions
                .Where(p => p.ActionNumber < action.ActionNumber && p.HandActionType != HandActionType.ANTE)
                .Sum(p => p.Amount);

            return Math.Abs(action.Amount + amount);
        }


        private string WritePostingActions(HandHistory hand)
        {
            var postingActions = hand.HandActions.Where(p => p.IsBlinds || p.HandActionType == HandActionType.POSTS);

            return string.Join(NEWLINE, postingActions.Select(p => WriteBlindActionLine(p, hand)));
        }

        static string GetBlindActionString(HandActionType actionType)
        {
            switch (actionType)
            {
                case HandActionType.ANTE:
                    return "posts the ante";
                case HandActionType.POSTS:
                    return "posts";
                case HandActionType.SMALL_BLIND:
                    return "posts small blind";
                case HandActionType.BIG_BLIND:
                    return "posts big blind";
                default:
                    throw new ArgumentException("Not a blind: " + actionType);
            }
        }

        static string WriteBlindActionLine(HandAction action, HandHistory hand)
        {
            string actionString = GetBlindActionString(action.HandActionType);

            return string.Format("{0} {1} (${2})",
                action.PlayerName,
                actionString,
                Math.Abs(action.Amount).ToString(InvariantCulture));
        }

        static string WritePlayer(Player player)
        {
            return string.Format("Seat {0}: {1} (${2}) ",
                player.SeatNumber,
                player.PlayerName,
                player.StartingStack.ToString(InvariantCulture));
        }

        private string WritePlayerList(HandHistory hand)
        {
            var playerLines = hand.Players
                .Select(p => WritePlayer(p));

            return string.Join(NEWLINE, playerLines);
        }

        static string WriteGameInfo(HandHistory hand)
        {
            var EasternTimeDate = hand.DateOfHandUtc.AddHours(1);//Central European Time

            var line1 = string.Format("***** History for hand R{0} *****",// string.Format("PokerStars Game #{0}:  {1} (${2}/${3} USD) - {4} ET",
                GetHandIdString(hand.HandId));

            var line2 = string.Format("Start hand: {0} {1} {2}",
                EasternTimeDate.DayOfWeek.ToString().Remove(3),
                GetMonthString(EasternTimeDate.Month),
                EasternTimeDate.ToString("dd HH:mm:ss CET yyyy", InvariantCulture));

            var line3 = string.Format("Table: {0} ({1} ${2}/${3}, Real money)",
                hand.TableName,
                GetGameTypeString(hand.GameDescription.GameType),
                hand.GameDescription.Limit.SmallBlind.ToString(InvariantCulture),
                hand.GameDescription.Limit.BigBlind.ToString(InvariantCulture));

            var line4 = string.Format("Button: seat {0}",
                hand.DealerButtonPosition);

            var line5 = string.Format("Players in round: {0}",
                hand.NumPlayersSeated);

            return string.Join(NEWLINE, line1, line2, line3, line4, line5);
        }

        private static string GetMonthString(int month)
        {
            switch (month)
            {
                case 1:
                    return "Jan";
                case 2:
                    return "Feb";
                case 3:
                    return "Mar";
                case 4:
                    return "Apr";
                case 5:
                    return "May";
                case 6:
                    return "Jun";
                case 7:
                    return "Jul";
                case 8:
                    return "Aug";
                case 9:
                    return "Sep";
                case 10:
                    return "Oct";
                case 11:
                    return "Nov";
                case 12:
                    return "Dec";
                default:
                    throw new ArgumentOutOfRangeException("Month");
            }
        }

        private static string GetHandIdString(long ID)
        {
            string IDString = ID.ToString();

            IDString = IDString.Insert(IDString.Length - 3, "-");
            IDString = IDString.Insert(1, "-");
            return IDString;
        }

        private static string GetGameTypeString(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.NoLimitHoldem:
                    return "NO_LIMIT TEXAS_HOLDEM";
                case GameType.FixedLimitHoldem:
                    return "Hold'em Limit";
                case GameType.PotLimitOmaha:
                    return "Omaha Pot Limit";
                case GameType.PotLimitHoldem:
                    return "Hold'em Pot Limit";
                case GameType.PotLimitOmahaHiLo:
                    return "Omaha Hi/Lo Pot Limit";
                default:
                    throw new NotImplementedException(string.Format("GameType: {0} not implemented for PokerStarsHandWriter", gameType));
            }
        }
    }
}
