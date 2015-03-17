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

namespace HandHistories.Writer.Writer.Pacific
{
    public class PacificHandWriter : IHandWriter
    {
        const string NEWLINE = "\r\n";

        static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public SiteName SiteName
        {
            get { return SiteName.Pacific; }
        }

        public string Write(HandHistory hand)
        {
            SetActionNumbers(hand);

            StringBuilder handText = new StringBuilder();

            handText.Append(WriteGameInfo(hand) + NEWLINE);

            handText.Append(WritePlayerList(hand) + NEWLINE);

            handText.Append(WritePostingActions(hand) + NEWLINE);

            handText.Append("** Dealing down cards **" + NEWLINE);

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

        private string GetStreetLine(Street currentStreet, BoardCards board)
        {
            switch (currentStreet)
            {
                case Street.Flop:
                    return string.Format("** Dealing flop ** [ {0}, {1}, {2} ]",
                        board[0],
                        board[1],
                        board[2]);
                case Street.Turn:
                    return string.Format("** Dealing turn ** [ {3} ]",
                        board[0],
                        board[1],
                        board[2],
                        board[3]);
                case Street.River:
                    return string.Format("** Dealing river ** [ {4} ]",
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

            lines.Add("** Summary **");

            var winningActions = hand.HandActions
                .Street(Street.Showdown);

            foreach (var action in winningActions)
            {
                if (action.HandActionType == HandActionType.SHOW)
                {
                    var showAction = hand.HandActions
                   .FirstOrDefault(p => p.PlayerName == action.PlayerName && p.HandActionType == HandActionType.SHOW);

                    if (showAction != null)
                    {
                        var player = hand.Players[action.PlayerName];
                        string cards = string.Join(", ", player.HoleCards.Select(p => p.Rank + p.Suit));

                        string showline = string.Format("{0} shows [ {1} ]",
                            action.PlayerName,
                            cards);

                        lines.Add(showline);
                    }
                }
                else if (action.IsWinningsAction)
                {
                    string collectLine = string.Format("{0} collected [ ${1} ]",
                        action.PlayerName,
                        action.Amount.ToString(InvariantCulture));

                    lines.Add(collectLine);
                        
                }
                else if (action.HandActionType == HandActionType.MUCKS)
                {
                    lines.Add(string.Format("{0} did not show his hand", action.PlayerName));
                }
            }

            return string.Join(NEWLINE, lines);
        }

        static string WriteGameActionLine(HandAction action, HandHistory hand)
        {
            string format;
            switch (action.HandActionType)
            {
                case HandActionType.FOLD:
                    format = "{0} folds";
                    break;
                case HandActionType.CHECK:
                    format = "{0} checks";
                    break;
                case HandActionType.CALL:
                    format = "{0} calls [${1}]";
                    break;
                case HandActionType.BET:
                    format = "{0} bets [${1}]";
                    break;
                case HandActionType.RAISE:
                    format = "{0} raises [${1}]";
                    break;
                default:
                    throw new NotImplementedException("No support for action: " + action.HandActionType);
            }

            return string.Format(format,
                action.PlayerName,
                Math.Abs(action.Amount).ToString(InvariantCulture));
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
            
            return string.Format("{0} {1} [${2}]",
                action.PlayerName,
                actionString,
                Math.Abs(action.Amount).ToString(InvariantCulture));
        }

        static string WritePlayer(Player player)
        {
            return string.Format("Seat {0}: {1} ( ${2} )",
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
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            var EasternTimeDate = TimeZoneInfo.ConvertTimeFromUtc(hand.DateOfHandUtc, timeZone);

            var line1 = string.Format("#Game No : {0}",
                hand.HandId);

            var line2 = string.Format("***** 888poker Hand History for Game {0} *****",
                hand.HandId);

            var line3 = string.Format("${0}/${1} Blinds {2} - *** {3}",
                hand.GameDescription.Limit.SmallBlind.ToString(InvariantCulture),
                hand.GameDescription.Limit.BigBlind.ToString(InvariantCulture),
                GetGameTypeString(hand.GameDescription.GameType),
                EasternTimeDate.ToString("dd MM yyyy H:mm:ss", InvariantCulture));

            var line4 = string.Format("Table {0} {1} Max (Real Money)",
                hand.TableName,
                hand.GameDescription.SeatType.MaxPlayers);

            var line5 = string.Format("Seat {0} is the button",
                hand.DealerButtonPosition);

            var line6 = string.Format("Total number of players : {0}",
                hand.NumPlayersActive);

            return string.Join(NEWLINE, line1, line2, line3, line4, line5, line6);
        }

        private static string GetGameTypeString(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.NoLimitHoldem:
                    return "No Limit Holdem";
                case GameType.NoLimitOmaha:
                    return "No Limit Omaha";
                case GameType.FixedLimitHoldem:
                    return "Limit Holdem";
                case GameType.PotLimitOmaha:
                    return "Pot Limit Omaha";
                case GameType.PotLimitHoldem:
                    return "Pot Limit Holdem";
                case GameType.PotLimitOmahaHiLo:
                    return "Pot Limit Omaha Hi/Lo";
                default:
                    throw new NotImplementedException(string.Format("GameType: {0} not implemented for PokerStarsHandWriter", gameType));
            }
        }
    }
}
