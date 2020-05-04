using HandHistories.Parser.Parsers.LineCategoryParser.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Utils.FastParsing;
using System.Text.RegularExpressions;
using HandHistories.Parser.Utils.RaiseAdjuster;

namespace HandHistories.Parser.Parsers.LineCategoryParser.WinningPokerV2
{
    sealed class WinningPokerNetworkV2LineCatParserImpl : HandHistoryParserLineCatImpl
    {
        public override SiteName SiteName => SiteName.WinningPokerV2;

        public override bool RequiresAdjustedRaiseSizes => true;

        public override void Categorize(Categories cat, string[] lines)
        {
            cat.Clear();
            cat.Add(LineCategory.Header, lines[0]);
            cat.Add(LineCategory.Header, lines[1]);

            int i = 2;

            //Seats
            for (; i < lines.Length; i++)
            {
                var line = lines[i];
                var lastChar = line[line.Length - 1];

                if (!line.StartsWith('S')) break;
                if (char.IsNumber(lastChar)) break;

                cat.Add(LineCategory.Seat, line);
            }

            //Pregame actions
            for (; i < lines.Length; i++)
            {
                var line = lines[i];
                var lastchar = line.Last();
                if (lastchar == '*') break;
                //<playername> waits for big blind
                //<playername> sits out
                if (lastchar == 'd' || lastchar == 't')
                {
                    cat.Add(LineCategory.Seat, line);
                }
                else
                {
                    cat.Add(LineCategory.Action, line);
                }
            }
            //Actions
            for (; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWithFast("Main pot ") && line.Contains('|')) continue;
                if (line.StartsWithFast("Side pot(") && line.Contains('|')) continue;

                if (isDealtToLine(line))
                {
                    cat.Add(LineCategory.Other, line);
                }
                else if (line == "*** SUMMARY ***")
                {
                    cat.Add(LineCategory.Other, line);
                    i++;
                    break;
                }
                else
                {
                    cat.Add(LineCategory.Action, line);
                }
            }

            //Summary
            for (; i < lines.Length; i++)
            {
                cat.Add(LineCategory.Summary, lines[i]);
            }
        }

        protected override BoardCards ParseCommunityCards(List<string> summary)
        {
            var board = summary[1];
            if (!(board.StartsWithFast("Board [") && board.EndsWith(']')))
            {
                return BoardCards.ForPreflop();
            }
            board = board.SubstringBetween(7, board.Length - 1);
            return BoardCards.FromCards(board);
        }

        protected override DateTime ParseDateUtc(List<string> header)
        {
            var line = header[0];
            var date = line.Substring(line.LastIndexOfFast(" - ") + 3);
            date = date.Remove(date.LastIndexOfFast(" "));
            return DateTime.Parse(date);
        }

        protected override int ParseDealerPosition(List<string> header)
        {
            var line = header[1];
            var dealer = FastInt.Parse(line, line.IndexOf('#') + 1);
            return dealer;
        }

        protected override GameType ParseGameType(List<string> header)
        {
            var line = header[0];
            var items = line.Split(new string[] { " - " }, StringSplitOptions.None);
            var gameTypeStr = items[1];
            var splitIndex = gameTypeStr.IndexOf('(');
            var gameStr = gameTypeStr.Remove(splitIndex);
            var limitStr = gameTypeStr.SubstringBetween(splitIndex + 1, gameTypeStr.Length - 1);
            return new GameType(ParseLimitEnum(limitStr), ParseGameEnum(gameStr));
        }

        static GameEnum ParseGameEnum(string game)
        {
            switch (game)
            {
                case "Omaha": return GameEnum.Omaha;
                case "Holdem": return GameEnum.Holdem;
                default:
                    throw new ArgumentException("Unknown gametype: " + game);
            }
        }

        static GameLimitEnum ParseLimitEnum(string game)
        {
            switch (game)
            {
                case "Pot Limit": return GameLimitEnum.PotLimit;
                case "No Limit": return GameLimitEnum.NoLimit;
                default:
                    throw new ArgumentException("Unknown gametype: " + game);
            }
        }

        protected override List<HandAction> ParseHandActions(List<string> actions, out List<WinningsAction> winners)
        {
            int i = 0;
            var currentStreet = Street.Preflop;
            var handactions = new List<HandAction>();
            winners = new List<WinningsAction>();

            //blinds
            for (; i < actions.Count; i++)
            {
                var line = actions[i];
                if (line.EndsWith('*'))
                {
                    i++;
                    break;
                }
                if (line.EndsWith('d'))//Player6 waits for big blind
                {
                    continue;
                }

                handactions.Add(ParseBlindAction(line));
            }
            
            //actions
            for (; i < actions.Count; i++)
            {
                var line = actions[i];

                if (line.StartsWithFast("Uncalled bet ("))
                {
                    const int amountStartIndex = 14; //"Uncalled bet (".Length
                    var amountEndIndex = line.IndexOf(')');
                    var nameStartIndex = amountEndIndex + 14;
                    var name = line.Substring(nameStartIndex);
                    var amount = line.SubstringBetween(amountStartIndex, amountEndIndex);
                    handactions.Add(new HandAction(name, HandActionType.UNCALLED_BET, amount.ParseAmount(), currentStreet));
                    continue;
                }

                bool allin = false;
                var lastChar = line[line.Length - 1];
                if (lastChar == 'n')//<Playername> calls $13.00 and is all-in
                {
                    allin = true;
                    line = line.Remove(line.Length - 14);
                    lastChar = line[line.Length - 1];
                }

                switch (lastChar)
                {
                    case '*': //*** SHOW DOWN ***
                        if (line == "*** SHOW DOWN ***")
                        {
                            i++;
                            goto PARSE_SHOWDOWN;
                        }
                        goto SUMMARY;
                    case ']'://*** TURN *** [Jc 7h 6s] [9h]
                        currentStreet = GetStreet(line);
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
                        handactions.Add(ParseActionWithAmount(line, currentStreet, allin));
                        break;
                    case 's':
                        handactions.Add(ParseActionWithoutAmount(line, currentStreet));
                        break;
                    case 'w'://<playername> does not show
                        break;
                    default:
                        throw new Exception("Unhandled action: " + line);
                }
            }

            PARSE_SHOWDOWN:
            //Showdown
            for (; i < actions.Count; i++)
            {
                var line = actions[i];
                switch (line.Last())
                {
                    case ')'://<playername> shows [Kd Qd 5s Js] (a pair of Jacks [Js Jc Kd 9h 8c])
                        {
                            var nameEndIndex = line.LastIndexOfFast(" shows ");
                            var name = line.Remove(nameEndIndex);
                            handactions.Add(new HandAction(name, HandActionType.SHOW, Street.Showdown));
                        }
                        break;
                    case 't'://<playername> collected $1.67 from main pot
                        //IGNORED - We parse this info from summary
                        //{
                        //    var amountEndIndex = line.Length - 14;
                        //    var amountStartIndex = line.LastIndexOf(' ', amountEndIndex - 1) + 1;
                        //    var amount = line.SubstringBetween(amountStartIndex, amountEndIndex);
                        //    var name = line.Remove(amountStartIndex - 11);
                        //    winners.Add(new WinningsAction(name, WinningsActionType.WINS, amount.ParseAmount(), 0));
                        //}
                        break;
                    case '1'://<playername> collected $75.62 from side pot-1
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                        break;
                    case 'd'://<playername> mucks hand
                        {
                            var name = line.Remove(line.Length - 11);
                            handactions.Add(new HandAction(name, HandActionType.MUCKS, Street.Showdown));
                        }
                        break;
                    default:
                        throw new Exception("Unhandled showdown line: " + line);
                }
            }

            SUMMARY:
            foreach (var line in Lines.Summary)
            {
                if (!line.StartsWith('S')) continue;
                
                switch (line.Last())
                {
                    //Seat 1: Player1 did not show and won $4.75
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
                        {
                            var amountStartIndex = line.LastIndexOf(' ') + 1;
                            var amount = line.Substring(amountStartIndex);
                            var nameStartIndex = line.IndexOf(':') + 2;
                            var nameEndIndex = amountStartIndex - 22;//" did not show and won ".Length
                            var name = line.SubstringBetween(nameStartIndex, nameEndIndex);
                            winners.Add(new WinningsAction(name, WinningsActionType.WINS, amount.ParseAmount(), 0));
                        }
                        break;

                    //Seat 2: Player 2 (button) showed [Ah Jh 3d Td] and won $79.50 with a straight, Jack high [Jh Td 9h 8c 7h]
                    case ']':
                        {
                            if (!line.Contains("] and won "))
                            {
                                continue;
                            }

                            var amountStartIndex = line.LastIndexOfFast(" won ") + 5;
                            var amountEndIndex = line.IndexOf(' ', amountStartIndex);
                            var amount = line.SubstringBetween(amountStartIndex, amountEndIndex);
                            var nameStartIndex = line.IndexOf(':') + 2;
                            var nameEndIndex = line.LastIndexOfFast(" showed", amountEndIndex);//" did not show and won ".Length
                            if (line[nameEndIndex - 1] == ')')
                            {
                                nameEndIndex = line.LastIndexOf(' ', nameEndIndex - 1);
                            }
                            var name = line.SubstringBetween(nameStartIndex, nameEndIndex);
                            winners.Add(new WinningsAction(name, WinningsActionType.WINS, amount.ParseAmount(), 0));
                        }
                        break;
                }
            }
            return FixPostsDead(handactions);
        }

        List<HandAction> FixPostsDead(List<HandAction> handactions)
        {
            decimal BB = 0;
            for (int i = 0; i < handactions.Count; i++)
            {
                var action = handactions[i];
                if (action.Street != Street.Preflop) break;

                if (action.HandActionType == HandActionType.BIG_BLIND)
                {
                    BB = action.Absolute;
                }
                if (action.HandActionType == HandActionType.POSTS_DEAD && action.Absolute > BB)
                {
                    var postAmount = BB;
                    var deadAmount = action.Amount - BB;

                    action.DecreaseAmount(postAmount);
                    handactions.Insert(i, new HandAction(action.PlayerName, HandActionType.POSTS, postAmount, Street.Preflop, i + 1));
                    i++;
                }
            }
            return handactions;
        }

        static HandAction ParseBlindAction(string line)
        {
            var nameEndIndex = line.LastIndexOfFast(" posts");
            var name = line.Remove(nameEndIndex);
            var amountStartIndex = line.LastIndexOf(' ') + 1;
            var amount = line.Substring(amountStartIndex);
            
            HandActionType action;
            switch (amountStartIndex - nameEndIndex)
            {
                case 23://<playername> posts the small blind $0.10
                    action = HandActionType.SMALL_BLIND;
                    break;
                case 21://<playername> posts the big blind $0.10
                    action = HandActionType.BIG_BLIND;
                    break;
                case 12://<playername> posts dead $0.15
                    action = HandActionType.POSTS_DEAD;
                    break;
                case 7://<playername> posts $0.25
                    action = HandActionType.POSTS;
                    break;
                default:
                    throw new ArgumentException("actiontype: " + line);
            }

            return new HandAction(name, action, amount.ParseAmount(), Street.Preflop);
        }

        static HandAction ParseActionWithoutAmount(string line, Street currentStreet)
        {
            var typeid = line[line.Length - 2];
            HandActionType action;
            switch (typeid)
            {
                case 'k':
                    action = HandActionType.CHECK;
                    break;
                case 'd':
                    action = HandActionType.FOLD;
                    break;
                default:
                    throw new ArgumentException("actiontype: " + line);
            }

            var name = line.Remove(line.LastIndexOf(' '));
            return new HandAction(name, action, 0, currentStreet);
        }

        static HandAction ParseActionWithAmount(string line, Street currentStreet, bool allin)
        {
            var amountStartIndex = line.LastIndexOf(' ') + 1;
            var amount = line.Substring(amountStartIndex);

            int nameEndIndex;
            var typeid = line[amountStartIndex - 4];
            HandActionType action;
            switch (typeid)
            {
                case ' ':
                    action = HandActionType.RAISE;
                    nameEndIndex = line.LastIndexOfFast(" raises ");
                    break;
                case 'e':
                    action = HandActionType.BET;
                    nameEndIndex = amountStartIndex - 6;
                    break;
                case 'l':
                    action = HandActionType.CALL;
                    nameEndIndex = amountStartIndex - 7;
                    break;
                default:
                    throw new ArgumentException("actiontype: " + line);
            }
            var name = line.Remove(nameEndIndex);
            return new HandAction(name, action, amount.ParseAmount(), currentStreet, AllInAction: allin);
        }

        protected override long[] ParseHandId(List<string> header)
        {
            var line = header[0];
            var idStr = line.SubstringBetween(6, line.IndexOf(' ', 6));

            return HandID.Parse(idStr);
        }

        protected override string ParseHeroName(List<string> other)
        {
            var hero = other.FirstOrDefault(isDealtToLine);
            if (hero == null)
            {
                return null;
            }
            return hero.SubstringBetween(9, hero.LastIndexOfFast(" ["));
        }

        protected override Limit ParseLimit(List<string> header)
        {
            var line = header[0];
            var stake = line.Split(new string[] { " - " }, StringSplitOptions.None)[2];

            var splitIndex = stake.IndexOf('/');
            var sb = stake.Remove(splitIndex);
            var bb = stake.Substring(splitIndex + 1);
            return Limit.FromSmallBlindBigBlind(sb.ParseAmount(), bb.ParseAmount(), Currency.USD);
        }


        static readonly Regex SummaryMuckRegex = new Regex(@"Seat \d{1,2}: (.+?) (\(button\) |\(big blind\) |\(small blind\) )?mucked \[(.+)\]", RegexOptions.Compiled);
        protected override PlayerList ParsePlayers(List<string> seats)
        {
            PlayerList playerList = new PlayerList();
            
            foreach (var line in seats)
            {
                if (line.EndsWith('n'))
                {
                    int seat = FastInt.Parse(line, 5);
                    int nameEndIndex = line.Length - 42;//" will be allowed to play after the button".Length
                    var name = line.SubstringBetween(line.IndexOf(':') + 2, nameEndIndex);
                    playerList.Add(new Player(name, 0, seat)
                    {
                        IsSittingOut = true,
                    });
                }
                else if (line.EndsWith('d'))//<playername> waits for big blind
                {
                    var name = line.Remove(line.Length - 20);//" waits for big blind".Length
                    playerList[name].IsSittingOut = true;
                }
                else
                {
                    bool sitOut = line.EndsWith('t');
                    if (sitOut && !line.StartsWithFast("Seat"))
                    {
                        var name = line.Remove(line.Length - 9);
                        playerList[name].IsSittingOut = true;
                    }
                    else
                    {
                        var stackEndIndex = sitOut ? line.Length - 16 : line.Length - 1;
                        playerList.Add(ParseSeatLine(line, stackEndIndex, sitOut));
                    }
                }
            }

            //Parse Shows
            for (int i = Lines.Action.Count - 1; i > 0; i--)
            {
                var line = Lines.Action[i];
                var lastchar = line[line.Length - 1];
                if (lastchar == '*') break;
                if (lastchar != ')') continue;

                var showIndex = line.LastIndexOfFast(" shows [");
                if (showIndex == -1) continue;

                var name = line.Remove(showIndex);
                var cards = line.SubstringBetween(showIndex + 8, line.IndexOf(']', showIndex));

                playerList[name].HoleCards = HoleCards.FromCards(cards);
            }

            //Parse Dealt to
            foreach (var line in Lines.Other.Where(isDealtToLine))//Dealt to <playername> [Ad Ad Kh Kd]
            {
                var cardStartIndex = line.LastIndexOf('[');
                var name = line.SubstringBetween(9, cardStartIndex - 1);
                var cards = line.SubstringBetween(cardStartIndex + 1, line.Length - 1);

                playerList[name].HoleCards = HoleCards.FromCards(cards);
            }

            //Parse Mucks
            foreach (var line in Lines.Summary.Where(line => line[0] == 'S' && line.EndsWith(']') && line.Contains(" mucked [")))
            {
                var muck = SummaryMuckRegex.Match(line);

                playerList[muck.Groups[1].Value].HoleCards = HoleCards.FromCards(muck.Groups[3].Value);
            }

            return playerList;
        }

        static Player ParseSeatLine(string line, int stackEndIndex, bool sitout)
        {
            bool sitOut = line.EndsWith('t');

            int seat = FastInt.Parse(line, 5);
            int nameEndIndex = line.LastIndexOfFast(" (", stackEndIndex);
            var name = line.SubstringBetween(line.IndexOf(':') + 2, nameEndIndex);
            var stack = line.SubstringBetween(nameEndIndex + 2, stackEndIndex);
            return new Player(name, stack.ParseAmount(), seat)
            {
                IsSittingOut = sitOut,
            };
        }

        protected override PokerFormat ParsePokerFormat(List<string> header)
        {
            return PokerFormat.CashGame;
        }

        protected override SeatType ParseSeatType(List<string> header)
        {
            var line = header[1];
            var maxPlayers = FastInt.Parse(line, line.IndexOf(' '));
            return SeatType.FromMaxPlayers(maxPlayers);
        }

        protected override string ParseTableName(List<string> header)
        {
            var line = header[1];
            return line.Remove(line.IndexOf(' '));
        }

        protected override TableType ParseTableType(List<string> header)
        {
            return new TableType();
        }

        private static readonly Regex HandSplitRegex = new Regex("Hand #", RegexOptions.Compiled);

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                            .Where(s => string.IsNullOrWhiteSpace(s) == false && s.Length > 30)
                            .Select(s => "Hand #" + s.Trim('\r', '\n'));
        }

        protected override bool IsValidHand(Categories lines)
        {
            return lines.Other.Contains("*** SUMMARY ***");
        }

        protected override bool IsValidOrCancelledHand(Categories lines, out bool isCancelled)
        {
            isCancelled = false;
            return IsValidHand(lines);
        }

        protected override void ParseExtraHandInformation(Categories lines, HandHistorySummary handHistorySummary)
        {
            var line = lines.Summary[0];
            var items = line.Split(new string[] { " | " }, StringSplitOptions.None);
            handHistorySummary.Rake = 0;

            foreach (var item in items)
            {
                switch (item[0])
                {
                    case 'T'://Total pot $79.50
                        handHistorySummary.TotalPot = item.Substring(10).ParseAmount();
                        break;
                    case 'R'://Rake $0.46
                        var rake = item.Substring(5).ParseAmount();
                        handHistorySummary.Rake = rake;
                        handHistorySummary.TotalPot += rake;
                        break;
                    case 'J'://JP Fee $0.04
                        var jp = item.Substring(7).ParseAmount();
                        handHistorySummary.Rake += jp;
                        handHistorySummary.TotalPot += jp;
                        break;
                    default:
                        throw new ArgumentException("Unhandled Summary item");
                }
            }
        }

        static Street GetStreet(string line)
        {
            switch (line[4])
            {
                case 'F': return Street.Flop;
                case 'T': return Street.Turn;
                case 'R': return Street.River;
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Dealt to playername [Ad Ac Kh Kd]
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static bool isDealtToLine(string line)
        {
            return line.StartsWithFast("Dealt to ") && line.EndsWith(']');
        }
    }
}
