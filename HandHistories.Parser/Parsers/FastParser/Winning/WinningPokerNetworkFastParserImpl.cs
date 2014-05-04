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
using HandHistories.Parser.Utils.Strings;
using System.Runtime.CompilerServices;

namespace HandHistories.Parser.Parsers.FastParser.Winning
{
    public class WinningPokerNetworkFastParserImpl : HandHistoryParserFastImpl
    {
        const int GameIDStartIndex = 9;
        const int actionPlayerNameStartIndex = 7;

        private SiteName _siteName;

        public override SiteName SiteName
        {
            get { return _siteName; }
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

        protected override long ParseHandId(string[] handLines)
        {
            //Game ID: 261402571 2/4 Braunite (Omaha)
            int endIndex = handLines[1].IndexOf(' ', GameIDStartIndex);
            long handId = long.Parse(handLines[1].Substring(GameIDStartIndex, endIndex - GameIDStartIndex));
            return handId;
        }

        protected override string ParseTableName(string[] handLines)
        {
            //Game ID: 258592747 2/4 Gabilite (JP) - CAP - Max - 2 (Hold'em)
            int StartIndex = handLines[1].IndexOf(' ', GameIDStartIndex);
            StartIndex = handLines[1].IndexOf(' ', StartIndex + 1);
            string tableName = handLines[1].Substring(StartIndex);
            tableName = tableName.Remove(tableName.IndexOf(" ("));
            return tableName.Trim();
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            int Players = ParsePlayers(handLines).Count;
            SeatType seat = SeatType.FromMaxPlayers(Players, true);
            return seat;
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            int startIndex = handLines[1].LastIndexOf('(');
            string game = handLines[1].Substring(startIndex);
            switch (game)
            {
                case "(Hold'em)":
                    return GameType.NoLimitHoldem;
                case "(Omaha)":
                    return GameType.PotLimitOmaha;
                default:
                    throw new NotImplementedException("GameType: " + game);
            }
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            List<TableTypeDescription> descriptions = new List<TableTypeDescription>();
            if (handLines[1].Contains("(JP)"))
            {
                descriptions.Add(TableTypeDescription.Jackpot);
            }
            if (handLines[1].Contains("CAP"))
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

        public override bool IsValidHand(string[] handLines)
        {
            if (handLines[handLines.Length - 1].StartsWith("Game ended at:"))
            {
                return true;
            }
            return false;
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;
            return IsValidHand(handLines);
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            const int MinimumLinesWithoutActions = 8;
            //Allocate the full list so we we dont get a reallocation for every add()
            List<HandAction> actions = new List<HandAction>(handLines.Length - MinimumLinesWithoutActions);
            Street currentStreet = Street.Preflop;
            int actionNumber = 2;

            PlayerList playerList = ParsePlayers(handLines);
            bool PlayerWithSpaces = playerList.FirstOrDefault(p => p.PlayerName.Contains(" ")) != null;

            //Skipping PlayerList
            int ActionsStart = GetActionStart(handLines);

            //Parsing Fixed Actions
            if (PlayerWithSpaces)
            {
                ActionsStart = SkipSitOutLines(handLines, ActionsStart);
                actions.Add(ParseSmallBlindWithSpaces(handLines[ActionsStart++], playerList));
                ActionsStart = SkipSitOutLines(handLines, ActionsStart);
                actions.Add(ParseBigBlindWithSpaces(handLines[ActionsStart++], playerList));
            }
            else
            {
                ActionsStart = SkipSitOutLines(handLines, ActionsStart);
                actions.Add(ParseSmallBlind(handLines[ActionsStart++]));
                ActionsStart = SkipSitOutLines(handLines, ActionsStart);
                actions.Add(ParseBigBlind(handLines[ActionsStart++]));
            }

            ActionsStart = ParsePosts(handLines, actions, ActionsStart, ref actionNumber);

            //Skipping all "received a card."
            ActionsStart = SkipDealer(handLines, ActionsStart);

            //ParseActions
            for (int i = ActionsStart; i < handLines.Length; i++)
            {
                string actionLine = handLines[i];
                if (actionLine[0] == 'P')
                {
                    string PlayerName = PlayerWithSpaces ?
                        GetPlayerNameWithSpaces(actionLine, playerList) :
                        GetPlayerNameWithoutSpaces(actionLine);
                        
                    int actionIDIndex = actionPlayerNameStartIndex + PlayerName.Length + 1;
                    switch (actionLine[actionIDIndex])
                    {
                        //Player PersnicketyBeagle folds
                        case 'f':
                            actions.Add(new HandAction(PlayerName, HandActionType.FOLD, 0, currentStreet, actionNumber++));
                            break;
                        case 'r':
                            actions.Add(new HandAction(PlayerName, HandActionType.RAISE, ParseActionAmount(actionLine), currentStreet, actionNumber++));
                            break;
                            //checks or calls
                        case 'c':
                            //Player butta21 calls (20)
                            if (actionLine[actionIDIndex + 1] == 'h')
                            {
                                actions.Add(new HandAction(PlayerName, HandActionType.CHECK, 0, currentStreet, actionNumber++));
                            }
                            else if (actionLine[actionIDIndex + 1] == 'a')
                            {
                                actions.Add(new HandAction(PlayerName, HandActionType.CALL, ParseActionAmount(actionLine), currentStreet, actionNumber++));
                            }
                            else
                            {
                                throw new NotImplementedException("HandActionType: " + actionLine);
                            }
                            break;
                        case 'b':
                            actions.Add(new HandAction(PlayerName, HandActionType.BET, ParseActionAmount(actionLine), currentStreet, actionNumber++));
                            break;
                        //Player PersnicketyBeagle allin (383)
                        case 'a':
                            actions.Add(new AllInAction(PlayerName, ParseActionAmount(actionLine), currentStreet, true, actionNumber++));
                            break;
                        case 'm':
                            //Player PersnicketyBeagle mucks cards
                            break;
                        case 'i':
                            //Player ECU7184 is timed out.
                            break;
                        default:
                            throw new NotImplementedException("HandActionType: " + actionLine);
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
                    string playerName = actionLine.Substring(actionLine.IndexOf(UncalledBet) + UncalledBet.Length);
                    actions.Add(new HandAction(playerName, HandActionType.UNCALLED_BET, ParseActionAmount(actionLine), currentStreet, actionNumber++));
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
            for (int i = ActionsStart + 3; i < handLines.Length; i++)
            {
                string actionLine = handLines[i];
                //Parse winning action
                if (actionLine[0] == '*')
                {
                    string playerName = PlayerWithSpaces ? GetWinnerNameWithSpaces(actionLine, playerList) : GetWinnerNameWithoutSpaces(actionLine);
                    int winAmountStartIndex = actionLine.LastIndexOf(':') + 2;
                    string winString = actionLine.Substring(winAmountStartIndex).TrimEnd('.');
                    decimal winAmount = decimal.Parse(winString, System.Globalization.CultureInfo.InvariantCulture);
                    actions.Add(new WinningsAction(playerName, HandActionType.WINS, winAmount, 0, actionNumber++));
                }
            }
            return actions;
        }

        private int ParsePosts(string[] handLines, List<HandAction> actions, int ActionsStart, ref int actionNumber)
        {
            for (int i = ActionsStart; i < handLines.Length; i++)
            {
                const int PlayerNameStartindex = 7;//"Player ".Length
                string actionLine = handLines[i];
                //Player bubblebubble wait BB
                char endChar = actionLine[actionLine.Length - 1];
                if (endChar == '.' || endChar == 'B' || endChar == ']')
                {
                    return i;
                }

                int playerNameEndIndex = actionLine.IndexOf(" posts (");
                if (playerNameEndIndex == -1)
                {
                    playerNameEndIndex = actionLine.IndexOf(" straddle (");
                }

                string playerName = actionLine.Substring(PlayerNameStartindex, playerNameEndIndex - PlayerNameStartindex);
                decimal Amount = ParseActionAmount(actionLine);

                string actionLine2 = handLines[++i];
                //Player bubblebubble wait BB
                if (actionLine2.EndsWith(".") || actionLine2.EndsWith("B") || actionLine2.EndsWith("]"))
                {
                    actions.Add(new HandAction(playerName, HandActionType.POSTS, Amount, Street.Preflop, actionNumber++));
                    return i;
                }

                playerNameEndIndex = actionLine2.IndexOf(" posts (");
                string playerName2 = actionLine2.Substring(PlayerNameStartindex, playerNameEndIndex - PlayerNameStartindex);

                if (playerName2 == playerName)
                {
                    Amount += ParseActionAmount(actionLine2);
                }
                //Player TheKunttzz posts (0.25) as a dead bet
                //Player TheKunttzz posts (0.50)
                //or
                //Player TheKunttzz posts (0.50)
                actions.Add(new HandAction(playerName, HandActionType.POSTS, Amount, Street.Preflop, actionNumber++));
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

        private string GetWinnerNameWithoutSpaces(string actionLine)
        {
            int playerNameEndIndex = actionLine.IndexOf(' ', actionPlayerNameStartIndex + 1);
            return actionLine.Substring(actionPlayerNameStartIndex + 1, playerNameEndIndex - actionPlayerNameStartIndex - 1);
        }

        private string GetWinnerNameWithSpaces(string actionLine, PlayerList playerList)
        {
            //Must choose the longest name if one name starts with an other name
            int length = 0;
            string result = null;
            foreach (var player in playerList)
            {
                if (actionLine.StartsWith("*Player " + player.PlayerName) && player.PlayerName.Length > length)
                {
                    length = player.PlayerName.Length;
                    result = player.PlayerName;
                }
            }
            return result;
        }

        private string GetPlayerNameWithoutSpaces(string actionLine)
        {
            int endIndex = actionLine.IndexOf(' ', actionPlayerNameStartIndex);
            return actionLine.Substring(actionPlayerNameStartIndex, endIndex - actionPlayerNameStartIndex);
        }

        private string GetPlayerNameWithSpaces(string actionLine, PlayerList playerList)
        {
            //Must choose the longest name if one name starts with an other name
            int length = 0;
            string result = null;
            foreach (var player in playerList)
            {
                if (actionLine.StartsWith("Player " + player.PlayerName) && player.PlayerName.Length > length)
                {
                    length = player.PlayerName.Length;
                    result = player.PlayerName;
                }
            }
            return result;
        }

        private int SkipDealer(string[] handLines, int ActionsStart)
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

        private HandAction ParseSmallBlind(string bbPost)
        {
            int playerNameEndIndex = GetPlayerNameEndIndex(bbPost);
            string PlayerName = bbPost.Substring(actionPlayerNameStartIndex, playerNameEndIndex - actionPlayerNameStartIndex);
            return new HandAction(PlayerName, HandActionType.SMALL_BLIND, ParseActionAmount(bbPost), Street.Preflop, 0);
        }

        private HandAction ParseBigBlind(string bbPost)
        {
            int playerNameEndIndex = GetPlayerNameEndIndex(bbPost);
            string PlayerName = bbPost.Substring(actionPlayerNameStartIndex, playerNameEndIndex - actionPlayerNameStartIndex);
            return new HandAction(PlayerName, HandActionType.BIG_BLIND, ParseActionAmount(bbPost), Street.Preflop, 1);
        }

        private HandAction ParseSmallBlindWithSpaces(string sbPost, PlayerList players)
        {
            string PlayerName = GetPlayerNameWithSpaces(sbPost, players);
            return new HandAction(PlayerName, HandActionType.SMALL_BLIND, ParseActionAmount(sbPost), Street.Preflop, 0);
        }

        private HandAction ParseBigBlindWithSpaces(string bbPost, PlayerList players)
        {
            string PlayerName = GetPlayerNameWithSpaces(bbPost, players);
            return new HandAction(PlayerName, HandActionType.BIG_BLIND, ParseActionAmount(bbPost), Street.Preflop, 1);
        }

        private decimal ParseActionAmount(string handLine)
        {
            int endIndex = handLine.LastIndexOf(')');
            int startIndex = handLine.LastIndexOf('(', endIndex) + 1;
            string text = handLine.Substring(startIndex, endIndex - startIndex);
            return decimal.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
        }

        private int GetPlayerNameEndIndex(string handLine)
        {
            return handLine.IndexOf(' ', actionPlayerNameStartIndex);
        }

        private int GetActionStart(string[] handLines)
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
                int NameEndIndex = playerLine.LastIndexOf(" (");
                string playerName = playerLine.Substring(NameStartIndex, NameEndIndex - NameStartIndex);

                int stackSizeStartIndex = NameEndIndex + 2;
                int stackSizeEndIndex = playerLine.Length - 2;
                string stack = playerLine.Substring(stackSizeStartIndex, stackSizeEndIndex - stackSizeStartIndex);
                //string playerName = playerLine.Substring(NameStartIndex, stackSizeStartIndex - NameStartIndex - 2);
                playerList.Add(new Player(playerName, decimal.Parse(stack, System.Globalization.CultureInfo.InvariantCulture), SeatNumber));
            }

            //...
            //Player NoahSDsDad has small blind (2)
            //Player xx45809 sitting out
            //Player megadouche sitting out
            //Player xx59704 wait BB
            CurrentLineIndex++;
            for (int i = 0; i < handLines.Length; i++)
            {
                const int NameStartIndex = 7;
                string sitOutLine = handLines[CurrentLineIndex + i];

                bool receivingCards = false;
                int NameEndIndex;
                string playerName;
                switch (sitOutLine[sitOutLine.Length - 1])
                {
                    //Player bubblebubble received card: [2h]
                    case ']':
                        //TODO: Parse cards here
                        break;
                    case '.':
                        //Player bubblebubble is timed out.
                        if (sitOutLine[sitOutLine.Length - 2] == 't')
                        {
                            continue;
                        }
                        receivingCards = true;
                        break;
                    case ')':
                        continue;
                    case 'B':
                        //Player bubblebubble wait BB
                        NameEndIndex = sitOutLine.Length - 8;//" wait BB".Length
                        playerName = sitOutLine.Substring(NameStartIndex, NameEndIndex - NameStartIndex);
                        playerList[playerName].IsSittingOut = true;
                        break;
                    case 't':
                        //Player xx45809 sitting out
                        if (sitOutLine[sitOutLine.Length - 2] == 'u')
                        {
                            NameEndIndex = sitOutLine.Length - 12;//" sitting out".Length
                            playerName = sitOutLine.Substring(NameStartIndex, NameEndIndex - NameStartIndex);
                            playerList[playerName].IsSittingOut = true;
                            break;
                        }
                        //Player TheKunttzz posts (0.25) as a dead bet
                        else continue;
                    default:
                        throw new ArgumentException("Unhandled Line: " + sitOutLine);
                }
                if (receivingCards)
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

                int ShowIndex = summaryLine.IndexOf(" shows: ");
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
    }
}
