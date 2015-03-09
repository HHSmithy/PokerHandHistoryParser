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
    public class PokerStarsHandWriter : IHandWriter
    {
        const string NEWLINE = "\r\n";

        static readonly CultureInfo NumberCulture = CultureInfo.InvariantCulture;

        public SiteName SiteName
        {
            get { return SiteName.PokerStars; }
        }

        public string Write(HandHistory hand)
        {
            SetActionNumbers(hand);

            StringBuilder handText = new StringBuilder();

            handText.Append(WriteGameInfo(hand) + NEWLINE);

            handText.Append(WritePlayerList(hand) + NEWLINE);

            handText.Append(WritePostingActions(hand) + NEWLINE);

            handText.Append("*** HOLE CARDS ***" + NEWLINE);

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
            Street currentStreet = Street.Preflop;
            var lines = new List<string>();

            foreach (var action in hand.HandActions
                .Where(p => !p.IsBlinds && p.HandActionType != HandActionType.POSTS))
            {
                if (action.Street != currentStreet)
                {
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

                        if (lastAction.HandActionType == HandActionType.CALL)
                        {
                            lines.Add("*** SHOW DOWN ***");
                        }
                    }
                }

                if (action.IsGameAction)
                {
                    lines.Add(WriteGameActionLine(action, hand));
                }
                if (action.IsShowdownAction)
                {
                    lines.Add(WriteShowDownAction(action, hand));
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
                    format = "{0}: doesn't show hand ";
                    break;
                case HandActionType.SHOW:
                    return GetShowLine(action, hand);
                case HandActionType.WINS:
                    format = "{0} collected ${1} from pot";
                    break;
                default:
                    throw new NotImplementedException("No support for action: " + action.HandActionType);
            }

            return string.Format(format,
                action.PlayerName,
                Math.Abs(action.Amount).ToString(NumberCulture));
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
                    return string.Format("*** FLOP *** [{0} {1} {2}]", 
                        board[0],
                        board[1],
                        board[2]);
                case Street.Turn:
                    return string.Format("*** TURN *** [{0} {1} {2}] [{3}]",
                        board[0],
                        board[1],
                        board[2],
                        board[3]);
                case Street.River:
                    return string.Format("*** RIVER *** [{0} {1} {2} {3}] [{4}]",
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
            lines.Add("*** SUMMARY ***");

            var potLine = string.Format("Total pot ${0} | Rake ${1} ",
                hand.TotalPot.Value.ToString(NumberCulture),
                hand.Rake.Value.ToString(NumberCulture));

            lines.Add(potLine);

            if (hand.ComumnityCards.Count != 0)
            {
                var board = string.Join(" ", hand.ComumnityCards.Select(p => p.ToString()));
                lines.Add(string.Format("Board [{0}]", board));
            }

            foreach (var player in hand.Players)
            {
                lines.Add(GetPlayerSummaryLine(player, hand));
            }

            return string.Join(NEWLINE, lines);
        }

        private string GetPlayerSummaryLine(Player player, HandHistory hand)
        {
            var preflopActions = hand.HandActions.Street(Street.Preflop);
            var playerActions = hand.HandActions.Where(p => p.PlayerName == player.PlayerName);
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
                        winAction.Amount.ToString(NumberCulture),
                        "a pair of Jacks");
                    }
                    else
                    {
                        return seat + position + string.Format("collected (${0})", winAction.Amount.ToString(NumberCulture));
                    }
                }
                else
                {
                    return string.Format("{0}{1}showed [{2}] and lost with {3}",
                    seat,
                    position,
                    string.Join(" ", player.HoleCards.Select(p => p.ToString())),
                    "a pair of Jacks");
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
                    return "Flop";
                case Street.Turn:
                    return "Turn";
                case Street.River:
                    return "River";
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
                    format = "{0}: folds ";
                    break;
                case HandActionType.CALL:
                    format = "{0}: calls ${1}";
                    break;
                case HandActionType.CHECK:
                    format = "{0}: checks ";
                    break;
                case HandActionType.ANTE:
                    format = "{0}: posts the ante ${1}";
                    break;
                case HandActionType.RAISE:
                    decimal raiseAmount = GetRaiseAmount(action, hand);
                    format = "{0}: raises ${1} to $" + raiseAmount.ToString(NumberCulture);
                    break;
                case HandActionType.BET:
                    format = "{0}: bets ${1}";
                    break;
                case HandActionType.SMALL_BLIND:
                    format = "{0}: posts small blind ${1}";
                    break;
                case HandActionType.BIG_BLIND:
                    format = "{0}: posts big blind ${1}";
                    break;
                case HandActionType.UNCALLED_BET:
                    format = "Uncalled bet (${1}) returned to {0}";
                    break;
                case HandActionType.POSTS:
                    format = "{0}: posts small blind ${1}";
                    break;
                case HandActionType.ALL_IN:
                    format = "{0}: raises $0.0 to ${1} and is all-in";
                    break;
                default:
                    throw new NotImplementedException("No support for action: " + action.HandActionType);
            }
            return string.Format(format, 
                action.PlayerName, 
                Math.Abs(action.Amount).ToString(NumberCulture));
        }

        private static decimal GetRaiseAmount(HandAction action, HandHistory hand)
        {
            var bets = hand.HandActions.Street(action.Street)
                .Where(p => p.IsAggressiveAction || p.HandActionType == HandActionType.BIG_BLIND);

            var amount = bets
                .Last(p => p.ActionNumber < action.ActionNumber).Amount;

            return Math.Abs(action.Amount + amount);
        }

        private string WritePostingActions(HandHistory hand)
        {
            var postingActions = hand.HandActions.Where(p => p.HandActionType == HandActionType.SMALL_BLIND
                || p.HandActionType == HandActionType.BIG_BLIND
                || p.HandActionType == HandActionType.ANTE
                || p.HandActionType == HandActionType.POSTS);

            return string.Join(NEWLINE, postingActions.Select(p => WriteGameActionLine(p, hand)));
        }

        static string WritePlayer(Player player)
        {
            return string.Format("Seat {0}: {1} (${2} in chips) ",
                player.SeatNumber,
                player.PlayerName,
                player.StartingStack.ToString(NumberCulture));
        }

        private string WritePlayerList(HandHistory hand)
        {
            var playerLines = hand.Players
                .Select(p => WritePlayer(p));

            return string.Join(NEWLINE, playerLines);
        }

        static string WriteGameInfo(HandHistory hand)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var EasternTimeDate = TimeZoneInfo.ConvertTimeFromUtc(hand.DateOfHandUtc, timeZone);

            var line1 = string.Format("PokerStars Game #{0}:  {1} (${2}/${3} USD) - {4} ET",
                hand.HandId,
                GetGameTypeString(hand.GameDescription.GameType),
                hand.GameDescription.Limit.SmallBlind.ToString(NumberCulture),
                hand.GameDescription.Limit.BigBlind.ToString(NumberCulture),
                EasternTimeDate.ToString("yyyy\\/MM\\/dd H:mm:ss", NumberCulture));

            var line2 = string.Format("Table '{0}' {1}-max Seat #{2} is the button",
                hand.TableName,
                hand.GameDescription.SeatType.MaxPlayers,
                hand.DealerButtonPosition);

            return line1 + NEWLINE + line2;
        }

        private static string GetGameTypeString(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.NoLimitHoldem:
                    return "Hold'em No Limit";
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
