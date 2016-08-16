using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Extensions;
using System.Runtime.CompilerServices;
using HandHistories.Objects.Hand;
using System.Globalization;
using HandHistories.Parser.Utils.AllInAction;

namespace HandHistories.Parser.Parsers.FastParser.Winning
{
    public sealed class WinningPokerNetworkFastParserImpl : HandHistoryParserFastImpl
    {
        const int GameIDStartIndex = 9;
        const int actionPlayerNameStartIndex = 7;

        private SiteName _siteName;

        public override SiteName SiteName
        {
            get { return _siteName; }
        }

        public override bool RequiresTotalPotCalculation
        {
            get { return true; }
        }

        public WinningPokerNetworkFastParserImpl()
        {
            _siteName = Objects.GameDescription.SiteName.WinningPoker;
        }

        private static readonly Regex HandSplitRegex = new Regex("Game started at: ", RegexOptions.Compiled);

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                            .Where(s => string.IsNullOrWhiteSpace(s) == false && s.Length > 30)
                            .Select(s => "Game started at: " + s.Trim('\r', '\n'));
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            //Seat 1 is the button
            const int offset = 5;
            int endIndex = handLines[2].IndexOf(' ', offset);
            string dealerLine = handLines[2].Substring(offset, endIndex - offset);
            return int.Parse(dealerLine);
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            //Game started at: 2014/3/8 22:1:43
            const int startindex = 17;
            string dataString = handLines[0].Substring(startindex);
            DateTime time = DateTime.Parse(dataString, System.Globalization.CultureInfo.InvariantCulture);
            return time;
        }

        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            return PokerFormat.CashGame;
        }

        protected override long ParseHandId(string[] handLines)
        {
            //Game ID: 261402571 2/4 Braunite (Omaha)
            int endIndex = handLines[1].IndexOf(' ', GameIDStartIndex);
            long handId = long.Parse(handLines[1].Substring(GameIDStartIndex, endIndex - GameIDStartIndex));
            return handId;
        }

        protected override long ParseTournamentId(string[] handLines)
        {
            throw new NotImplementedException();
        }

        protected override string ParseTableName(string[] handLines)
        {
            //Real money format:
            //Game ID: 261409536 2/4 Braunite (Omaha)
            //Game ID: 258592747 2/4 Gabilite (JP) - CAP - Max - 2 (Hold'em)
            //Play money format:
            //Game ID: 261409536 1/2 Wichita Falls (Omaha)
            //Game ID: 328766507 1/2 Wichita Falls 1/2 - 3 (Hold'em)
            string tablenameLine = handLines[1];
            int StartIndex = tablenameLine.IndexOf('/', GameIDStartIndex) + 2;
            StartIndex = tablenameLine.IndexOf(' ', StartIndex) + 1;
            string tableName = tablenameLine.Substring(StartIndex);

            int GameTypeStartIndex = tableName.LastIndexOf('(');

            return tableName.Remove(GameTypeStartIndex).Trim();
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            int Players = ParsePlayers(handLines).Count;
            SeatType seat = SeatType.FromMaxPlayers(Players, true);
            return seat;
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            string line = handLines[1];
            int startIndex = line.LastIndexOf('(');
            string game = line.Substring(startIndex);
            switch (game)
            {
                case "(Hold'em)":
                    return GameType.NoLimitHoldem;
                case "(Omaha)":
                    return GameType.PotLimitOmaha;
                default:
                    throw new UnrecognizedGameTypeException(line, "GameType: " + game);
            }
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            List<TableTypeDescription> descriptions = new List<TableTypeDescription>();
            if (handLines[1].Contains("(JP)"))
            {
                descriptions.Add(TableTypeDescription.Jackpot);
            }
            if (handLines[1].Contains(" CAP "))
            {
                descriptions.Add(TableTypeDescription.Cap);
            }

            return TableType.FromTableTypeDescriptions(descriptions.ToArray());
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            //Game ID: 261402571 2/4 Braunite (Omaha)
            int startIndex = handLines[1].IndexOf(' ', GameIDStartIndex);
            int endIndex = handLines[1].IndexOf(' ', startIndex + 1);
            string limitText = handLines[1].Substring(startIndex, endIndex - startIndex);
            int splitIndex = limitText.IndexOf('/');
            decimal smallBlind = decimal.Parse(limitText.Remove(splitIndex), System.Globalization.CultureInfo.InvariantCulture);
            decimal bigBlind = decimal.Parse(limitText.Substring(splitIndex + 1), System.Globalization.CultureInfo.InvariantCulture);
            Limit limit = Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, Currency.USD);
            return limit;
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            throw new NotImplementedException();
        }

        public override bool IsValidHand(string[] handLines)
        {
            if (handLines[handLines.Length - 1].StartsWithFast("Game ended at:"))
            {
                return true;
            }
            return false;
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = IsCancelledHand(handLines);
            return IsValidHand(handLines);
        }

        private bool IsCancelledHand(string[] handLines)
        {
            int start = GetActionStart(handLines);

            for (int i = start; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line.EndsWithFast("received a card."))
                {
                    return false;
                }
            }

            return true;
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            const int MinimumLinesWithoutActions = 8;
            //Allocate the full list so we we dont get a reallocation for every add()
            List<HandAction> handActions = new List<HandAction>(handLines.Length - MinimumLinesWithoutActions);
            Street currentStreet = Street.Preflop;

            PlayerList playerList = ParsePlayers(handLines);
            bool PlayerWithSpaces = playerList.FirstOrDefault(p => p.PlayerName.Contains(" ")) != null;

            //Skipping PlayerList
            int ActionsStart = GetActionStart(handLines);

            //Parsing Fixed Actions
            if (PlayerWithSpaces)
            {
                ActionsStart = SkipSitOutLines(handLines, ActionsStart);
                handActions.Add(ParseSmallBlindWithSpaces(handLines[ActionsStart++], playerList));
                ActionsStart = SkipSitOutLines(handLines, ActionsStart);
                handActions.Add(ParseBigBlindWithSpaces(handLines[ActionsStart++], playerList));
            }
            else
            {
                ActionsStart = SkipSitOutLines(handLines, ActionsStart);
                handActions.Add(ParseSmallBlind(handLines[ActionsStart++]));
                ActionsStart = SkipSitOutLines(handLines, ActionsStart);
                handActions.Add(ParseBigBlind(handLines[ActionsStart++]));
            }

            ActionsStart = ParsePosts(handLines, handActions, ActionsStart);

            //Skipping all "received a card."
            ActionsStart = SkipDealer(handLines, ActionsStart);

            //ParseActions
            for (int i = ActionsStart; i < handLines.Length; i++)
            {
                string actionLine = handLines[i];
                if (actionLine[0] == 'P')
                {
                    var action = ParseRegularAction(actionLine, currentStreet, playerList, handActions, PlayerWithSpaces);
                    if (action != null)
                    {
                        handActions.Add(action);
                    }
                }
                else if (actionLine[0] == '*')
                {
                    //*** FLOP ***: [10s 9c 2c]
                    currentStreet = ParseNextStreet(actionLine);
                }
                else if (actionLine[0] == 'U')
                {
                    const string UncalledBet = ") returned to ";
                    string playerName = actionLine.Substring(actionLine.IndexOfFast(UncalledBet) + UncalledBet.Length);
                    handActions.Add(new HandAction(playerName, HandActionType.UNCALLED_BET, ParseActionAmountBeforePlayer(actionLine), currentStreet));
                }
                else if (actionLine[0] == '-')
                {
                    ActionsStart = i++;
                    break;
                }
            }

            //expected Summary
            //------ Summary ------
            //Pot: 14.95. Rake 0.80
            //Board: [5c 8s 4h 10c 10s]
            for (int i = ActionsStart + 2; i < handLines.Length; i++)
            {
                string actionLine = handLines[i];
                //Parse winning action
                if (actionLine[0] == '*')
                {
                    var action = ParseWinningsAction(actionLine, playerList, PlayerWithSpaces);
                    handActions.Add(action);
                }
            }
            return handActions;
        }

        public static HandAction ParseRegularAction(string line, Street currentStreet, PlayerList playerList, List<HandAction> actions, bool PlayerWithSpaces)
        {
            string PlayerName = PlayerWithSpaces ?
                GetPlayerNameWithSpaces(line, playerList) :
                GetPlayerNameWithoutSpaces(line);

            int actionIDIndex = actionPlayerNameStartIndex + PlayerName.Length + 1;
            char actionID = line[actionIDIndex];
            switch (actionID)
            {
                //Player PersnicketyBeagle folds
                case 'f':
                    return new HandAction(PlayerName, HandActionType.FOLD, 0, currentStreet);
                case 'r':
                    return new HandAction(PlayerName, HandActionType.RAISE, ParseActionAmountAfterPlayer(line), currentStreet);
                //checks or calls
                case 'c':
                    //Player butta21 calls (20)
                    if (line[actionIDIndex + 1] == 'h')
                    {
                        return new HandAction(PlayerName, HandActionType.CHECK, 0, currentStreet);
                    }
                    else if (line[actionIDIndex + 1] == 'a')
                    {
                        return new HandAction(PlayerName, HandActionType.CALL, ParseActionAmountAfterPlayer(line), currentStreet);
                    }
                    else
                    {
                        throw new NotImplementedException("HandActionType: " + line);
                    }
                case 'b':
                    if (currentStreet == Street.Preflop)
                    {
                        return new HandAction(PlayerName, HandActionType.RAISE, ParseActionAmountAfterPlayer(line), currentStreet);
                    }
                    else
                    {
                        return new HandAction(PlayerName, HandActionType.BET, ParseActionAmountAfterPlayer(line), currentStreet);
                    }
                //Player PersnicketyBeagle allin (383)
                case 'a':
                    var amount = ParseActionAmountAfterPlayer(line);
                    var actionType = AllInActionHelper.GetAllInActionType(PlayerName, amount, currentStreet, actions);
                    return new HandAction(PlayerName, actionType, amount, currentStreet, true);
                case 'm'://Player PersnicketyBeagle mucks cards
                case 'i'://Player ECU7184 is timed out.
                    return null;
            }
            throw new HandActionException(line, "HandActionType: " + line);
        }

        public static HandAction ParseWinningsAction(string line, PlayerList playerList, bool PlayerWithSpaces)
        {
            string playerName = PlayerWithSpaces ? GetWinnerNameWithSpaces(line, playerList) : GetWinnerNameWithoutSpaces(line);
            int winAmountStartIndex = line.LastIndexOfFast("cts:") + 5;
            int winAmountEndIndex = line.IndexOfFast(". ", winAmountStartIndex);
            string winString = line.Substring(winAmountStartIndex, winAmountEndIndex - winAmountStartIndex);
            decimal winAmount = winString.ParseAmount();
            return new WinningsAction(playerName, HandActionType.WINS, winAmount, 0);
        }

        private int ParsePosts(string[] handLines, List<HandAction> actions, int ActionsStart)
        {
            for (int i = ActionsStart; i < handLines.Length; i++)
            {
                const int PlayerNameStartindex = 7;//"Player ".Length
                string line = handLines[i];
                
                char endChar = line[line.Length - 1];
                bool deadBet = false;

                switch (endChar)
                {
                    //Player bubblebubble wait BB
                    case 'B':
                        continue;//More posts can still occur

                    //Player Aquasces1 received a card.
                    case '.':
                    //Player WP_Hero received card: [6d]
                    case ']': 
                        //No more posts can occur when players start reciving cards
                        return i;

                    //Player TheKunttzz posts (0.25) as a dead bet
                    //Player TheKunttzz posts (0.50)
                    case 't':   
                        deadBet = true;
                        break;

                    //Player Aquasces1 has small blind (2)
                    //Player COMON-JOE-JUG has big blind (4)
                    //Player TheKunttzz posts (0.25)
                    //Player TheKunttzz straddles (0.50)
                    case ')':
                        break;

                    default:
                        throw new HandActionException(line, "Unrecognized endChar \"" + endChar + "\"");
                }

                int playerNameEndIndex = line.IndexOfFast(" posts (");
                if (playerNameEndIndex == -1)
                {
                    playerNameEndIndex = line.IndexOfFast(" straddle (");
                }

                string playerName = line.Substring(PlayerNameStartindex, playerNameEndIndex - PlayerNameStartindex);
                decimal Amount = ParseActionAmountAfterPlayer(line);

                if (deadBet)
                {
                    Amount += ParseActionAmountAfterPlayer(handLines[++i]);
                }

                actions.Add(new HandAction(playerName, HandActionType.POSTS, Amount, Street.Preflop));
            }
            throw new Exception("Did not find start of Dealing of cards");
        }

        private int SkipSitOutLines(string[] handLines, int ActionsStart)
        {
            for (int i = ActionsStart; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line[line.Length - 1] == ')')
                {
                    return i;
                }
            }
            throw new Exception("Did not find end of Sitout Lines");
        }

        private static Street ParseNextStreet(string actionLine)
        {
            //*** FLOP ***: [10s 9c 2c]
            const int StreetIndex = 4;
            switch (actionLine[StreetIndex])
            {
                case 'F':
                    return Street.Flop;
                case 'T':
                    return Street.Turn;
                case 'R':
                    return Street.River;
                default:
                    throw new ArgumentException("Street: " + actionLine);
            }
        }

        static string GetWinnerNameWithoutSpaces(string actionLine)
        {
            int playerNameEndIndex = actionLine.IndexOf(' ', actionPlayerNameStartIndex + 1);
            return actionLine.Substring(actionPlayerNameStartIndex + 1, playerNameEndIndex - actionPlayerNameStartIndex - 1);
        }

        static string GetWinnerNameWithSpaces(string actionLine, PlayerList playerList)
        {
            //Must choose the longest name if one name starts with an other name
            int length = 0;
            string result = null;
            foreach (var player in playerList)
            {
                if (actionLine.StartsWithFast("*Player " + player.PlayerName) && player.PlayerName.Length > length)
                {
                    length = player.PlayerName.Length;
                    result = player.PlayerName;
                }
            }
            return result;
        }

        static string GetPlayerNameWithoutSpaces(string actionLine)
        {
            int endIndex = actionLine.IndexOf(' ', actionPlayerNameStartIndex);
            return actionLine.Substring(actionPlayerNameStartIndex, endIndex - actionPlayerNameStartIndex);
        }

        static string GetPlayerNameWithSpaces(string actionLine, PlayerList playerList)
        {
            //Must choose the longest name if one name starts with an other name
            int length = 0;
            string result = null;
            foreach (var player in playerList)
            {
                if (actionLine.StartsWithFast("Player " + player.PlayerName) && player.PlayerName.Length > length)
                {
                    length = player.PlayerName.Length;
                    result = player.PlayerName;
                }
            }
            return result;
        }

        static int SkipDealer(string[] handLines, int ActionsStart)
        {
            for (int i = ActionsStart; i < handLines.Length; i++)
            {
                //Player LadyStack received a card.
                string Line = handLines[i];
                switch (handLines[i][Line.Length - 1])
                {
                    case 'B':
                    case '.':
                    case ']'://Player {playername} received card: [2h]
                        continue;
                    default:
                        return i;
                }
            }
            throw new ArgumentOutOfRangeException("handlines");
        }

        public static HandAction ParseSmallBlind(string bbPost)
        {
            int playerNameEndIndex = GetPlayerNameEndIndex(bbPost);
            string PlayerName = bbPost.Substring(actionPlayerNameStartIndex, playerNameEndIndex - actionPlayerNameStartIndex);
            return new HandAction(PlayerName, HandActionType.SMALL_BLIND, ParseActionAmountAfterPlayer(bbPost), Street.Preflop);
        }

        public static HandAction ParseBigBlind(string bbPost)
        {
            int playerNameEndIndex = GetPlayerNameEndIndex(bbPost);
            string PlayerName = bbPost.Substring(actionPlayerNameStartIndex, playerNameEndIndex - actionPlayerNameStartIndex);
            return new HandAction(PlayerName, HandActionType.BIG_BLIND, ParseActionAmountAfterPlayer(bbPost), Street.Preflop);
        }

        public static HandAction ParseSmallBlindWithSpaces(string sbPost, PlayerList players)
        {
            string PlayerName = GetPlayerNameWithSpaces(sbPost, players);
            return new HandAction(PlayerName, HandActionType.SMALL_BLIND, ParseActionAmountAfterPlayer(sbPost), Street.Preflop);
        }

        public static HandAction ParseBigBlindWithSpaces(string bbPost, PlayerList players)
        {
            string PlayerName = GetPlayerNameWithSpaces(bbPost, players);
            return new HandAction(PlayerName, HandActionType.BIG_BLIND, ParseActionAmountAfterPlayer(bbPost), Street.Preflop);
        }

        /// <summary>
        /// The amount must be after the player name or it will fail when players have a parathesis in their name
        /// </summary>
        /// <param name="handLine"></param>
        /// <returns></returns>
        static decimal ParseActionAmountAfterPlayer(string handLine)
        {
            int endIndex = handLine.LastIndexOf(')');
            int startIndex = handLine.LastIndexOf('(', endIndex) + 1;
            string text = handLine.Substring(startIndex, endIndex - startIndex);
            return text.ParseAmount();
        }

        /// <summary>
        /// The amount must be before the player name or it will fail when players have a parathesis in their name
        /// </summary>
        /// <param name="handLine"></param>
        /// <returns></returns>
        private decimal ParseActionAmountBeforePlayer(string handLine)
        {
            int startIndex = handLine.IndexOf('(') + 1;
            int endIndex = handLine.IndexOf(')', startIndex);
            string text = handLine.Substring(startIndex, endIndex - startIndex);
            return text.ParseAmount();
        }

        static int GetPlayerNameEndIndex(string handLine)
        {
            return handLine.IndexOf(' ', actionPlayerNameStartIndex);
        }

        static int GetActionStart(string[] handLines)
        {
            for (int i = 3; i < handLines.Length; i++)
            {
                if (handLines[i][0] != 'S')
                {
                    return i;
                }
            }
            throw new ArgumentOutOfRangeException("handlines");
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            //Expected Start 
            //Game started at: 2014/2/28 15:59:51
            //Game ID: 258592968 2/4 Gabilite (JP) - CAP - Max - 2 (Hold'em)
            //Seat 4 is the button
            //Seat 1: xx59704 (159.21).
            //Seat 4: Xavier2500 (110.40).
            //...
            PlayerList playerList = new PlayerList();
            int CurrentLineIndex = 3;

            //The first line after the player list always starts with "Player "
            while (handLines[CurrentLineIndex][0] == 'S')
            {
                string playerLine = handLines[CurrentLineIndex++];

                const int seatNumberStart = 5;
                int colonIndex = playerLine.IndexOf(':', seatNumberStart + 1);
                int SeatNumber = int.Parse(playerLine.Substring(seatNumberStart, colonIndex - seatNumberStart));

                //Parsing playerName
                //PlayerName can contain '(' & ')'
                int NameStartIndex = colonIndex + 2;
                int NameEndIndex = playerLine.LastIndexOfFast(" (");
                string playerName = playerLine.Substring(NameStartIndex, NameEndIndex - NameStartIndex);

                int stackSizeStartIndex = NameEndIndex + 2;
                int stackSizeEndIndex = playerLine.Length - 2;
                string stack = playerLine.Substring(stackSizeStartIndex, stackSizeEndIndex - stackSizeStartIndex);
                //string playerName = playerLine.Substring(NameStartIndex, stackSizeStartIndex - NameStartIndex - 2);
                playerList.Add(new Player(playerName, decimal.Parse(stack, System.Globalization.CultureInfo.InvariantCulture), SeatNumber));
            }

            //HandHistory format:
            //...
            //Player NoahSDsDad has small blind (2)
            //Player xx45809 sitting out
            //Player megadouche sitting out
            //Player xx59704 wait BB

            //Parsing Sitouts and HoleCards
            for (int i = CurrentLineIndex; i < handLines.Length; i++)
            {
                const int NameStartIndex = 7;
                string line = handLines[i];

                bool receivingCards = false;
                int NameEndIndex;
                string playerName;

                //Uncalled bet (20) returned to zz7
                if (line[0] == 'U')
                {
                    break;
                }

                switch (line[line.Length - 1])
                {
                    //Player bubblebubble received card: [2h]
                    case ']':
                        receivingCards = true;
                        break;
                    
                    case '.':
                        //Player bubblebubble is timed out.
                        if (line[line.Length - 2] == 't')
                        {
                            continue;
                        }
                        receivingCards = true;
                        break;
                    case ')':
                        continue;
                    case 'B':
                        //Player bubblebubble wait BB
                        NameEndIndex = line.Length - 8;//" wait BB".Length
                        playerName = line.Substring(NameStartIndex, NameEndIndex - NameStartIndex);
                        playerList[playerName].IsSittingOut = true;
                        break;
                    case 't':
                        //Player xx45809 sitting out
                        if (line[line.Length - 2] == 'u')
                        {
                            NameEndIndex = line.Length - 12;//" sitting out".Length
                            playerName = line.Substring(NameStartIndex, NameEndIndex - NameStartIndex);
                            if (playerName == "")//"Player  sitting out"
                            {
                                continue;
                            }
                            playerList[playerName].IsSittingOut = true;
                            break;
                        }
                        //Player TheKunttzz posts (0.25) as a dead bet
                        else continue;
                    default:
                        throw new ArgumentException("Unhandled Line: " + line);
                }
                if (receivingCards)
                {
                    CurrentLineIndex = i;
                    break;
                }
            }

            //Parse HoleCards
            for (int i = CurrentLineIndex; i < handLines.Length; i++)
            {
                const int NameStartIndex = 7;
                string line = handLines[i];
                char endChar = line[line.Length - 1];

                if (endChar == '.')
                {
                    continue;
                }
                else if (endChar == ']')
                {
                    int NameEndIndex = line.LastIndexOfFast(" rec", line.Length - 12);
                    string playerName = line.Substring(NameStartIndex, NameEndIndex - NameStartIndex);

                    char rank = line[line.Length - 3];
                    char suit = line[line.Length - 2];

                    var player = playerList[playerName];
                    if (!player.hasHoleCards)
                    {
                        player.HoleCards = HoleCards.NoHolecards();
                    }
                    if (rank == '0')
                    {
                        rank = 'T';
                    }

                    player.HoleCards.AddCard(new Card(rank, suit));
                    continue;
                }
                else
                {
                    break;
                }
            }

            //Expected End
            //*Player xx59704 shows: Straight to 8 [7s 4h]. Bets: 110.40. Collects: 220.30. Wins: 109.90.
            //Player Xavier2500 shows: Two pairs. Js and 8s [10h 8c]. Bets: 110.40. Collects: 0. Loses: 110.40.
            //Game ended at: 
            for (int i = handLines.Length - playerList.Count - 1; i < handLines.Length - 1; i++)
            {
                const int WinningStartOffset = 1;
                const int PlayerMinLength = 7; // = "Player ".Length

                string summaryLine = handLines[i];
                
                int playerNameStartIndex = PlayerMinLength + (summaryLine[0] == '*' ? WinningStartOffset : 0);
                int playerNameEndIndex = summaryLine.IndexOf(' ', playerNameStartIndex);

                int ShowIndex = summaryLine.IndexOfFast(" shows: ");
                if (ShowIndex != -1)
                {
                    string playerName = summaryLine.Substring(playerNameStartIndex, ShowIndex - playerNameStartIndex);

                    int pocketStartIndex = summaryLine.IndexOf('[', playerNameEndIndex) + 1;
                    int pocketEndIndex = summaryLine.IndexOf(']', pocketStartIndex);

                    Player showdownPlayer = playerList[playerName];
                    if (!showdownPlayer.hasHoleCards)
                    {
                         string cards = summaryLine.Substring(pocketStartIndex, pocketEndIndex - pocketStartIndex);
                        cards = cards.Replace("10", "T");
                        showdownPlayer.HoleCards = HoleCards.FromCards(cards);
                    }
                }
            }

            return playerList;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            // Expect the end of the hand to have something like this:
            //------ Summary ------
            //Pot: 80. Rake 2
            //Board: [3d 6h 2c Ah]

            BoardCards boardCards = BoardCards.ForPreflop();
            for (int lineNumber = handLines.Length - 5; lineNumber >= 0; lineNumber--)
            {
                string line = handLines[lineNumber];
                if (line[0] == '-')
                {
                    return boardCards;
                }

                if (line[0] != 'B')
                {
                    continue;
                }

                const int firstSquareBracketEnd = 8;
                int lastSquareBracket = line.Length - 1;

                return BoardCards.FromCards(line.Substring(firstSquareBracketEnd, lastSquareBracket - firstSquareBracketEnd).Replace("10", "T"));
            }

            throw new CardException(string.Empty, "Read through hand backwards and didn't find a board or summary.");
        }

        //there is total pot in the hands but they exclude bb and rake, so we calculate it instead
        //protected override void ParseExtraHandInformation(string[] handLines, HandHistorySummary handHistorySummary)
        //{
        //    if (handHistorySummary.Cancelled)
        //    {
        //        return;
        //    }

        //    for (int i = handLines.Length - 1; i > 0; i--)
        //    {
        //        string line = handLines[i];
        //        if (line.StartsWithFast("Pot"))
        //        {
        //            //Pot: 80. Rake 2
        //            const int PotStartIndex = 5;
        //            int PotEndIndex = line.IndexOf(' ', PotStartIndex) - 1;

        //            string TotalPotStr = line.Substring(PotStartIndex, PotEndIndex - PotStartIndex);

        //            handHistorySummary.TotalPot = TotalPotStr.ParseAmount();

        //            int RakeStartIndex = line.LastIndexOf(' ');
        //            string RakeStr = line.Substring(RakeStartIndex);

        //            handHistorySummary.Rake = RakeStr.ParseAmount();
        //            break;
        //        }
        //    }
        //}

        protected override string ParseHeroName(string[] handlines)
        {
            for (int i = 0; i < handlines.Length; i++)
            {
                string line = handlines[i];
                if (line[0] == 'P' && line[line.Length - 1] == ']')
                {
                    const int NameStartIndex = 7;
                    int NameEndIndex = line.LastIndexOfFast(" r");
                    return line.Substring(NameStartIndex, NameEndIndex - NameStartIndex);
                }
            }
            return null;
        }
    }
}
