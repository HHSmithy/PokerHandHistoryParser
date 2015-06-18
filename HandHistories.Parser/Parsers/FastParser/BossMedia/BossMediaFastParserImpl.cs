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
using System.Globalization;
using HandHistories.Parser.Utils.AllInAction;
using HandHistories.Parser.Utils.FastParsing;
using HandHistories.Parser.Utils.Extensions;

namespace HandHistories.Parser.Parsers.FastParser.BossMedia
{
    public sealed class BossMediaFastParserImpl : HandHistoryParserFastImpl
    {
        static readonly CultureInfo provider = CultureInfo.InvariantCulture;

        public override SiteName SiteName
        {
            get { return Objects.GameDescription.SiteName.BossMedia; }
        }

        public override bool RequiresAdjustedRaiseSizes
        {
            get { return true; }
        }

        public override bool RequiresUncalledBetFix
        {
            get { return true; }
        }

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return rawHandHistories.Split(new string[] { "<HISTORY " }, StringSplitOptions.None)
                .Where(p => p.Length > 2 && p[1] != '?')
                .Select(p => "<HISTORY " + p);
        }

        public override IEnumerable<string[]> SplitUpMultipleHandsToLines(string rawHandHistories)
        {
            var allLines = rawHandHistories.Split('\n');

            List<string> handLines = new List<string>(50);

            bool handFound = false;

            foreach (var item in allLines)
            {
                string line = item.TrimEnd('\r', ' ');

                if (line.StartsWith("<HISTORY ", StringComparison.Ordinal))
                {
                    handFound = true;
                    if (handLines.Count > 0)
                    {
                        yield return handLines.ToArray();
                        handLines = new List<string>(50);
                    }
                }

                if (handFound)
	            {
		            handLines.Add(line);
                }
            }

            if (handLines.Count > 0)
            {
                yield return handLines.ToArray();
            }
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            for (int i = 0; i < 10; i++)
            {
                //<PLAYER NAME="fatima1975" SEAT="6" AMOUNT="4.27" DEALER="Y"></PLAYER>
                string Line = handLines[i + 1];
                if (Line[1] != 'P')
                {
                    break;
                }

                if (GetXMLAttributeValue(Line, "DEALER") == "Y")
                {
                    string dealerID = GetXMLAttributeValue(Line, "SEAT");
                    return int.Parse(dealerID);
                }
            }
            throw new ArgumentException("No dealer found");
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            //POSIX Time:
            //http://en.wikipedia.org/wiki/Unix_time
            long ticks = long.Parse(GetXMLAttributeValue(handLines[0], "DATE"));

            DateTime POSIX_EPOCH = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            DateTime time = POSIX_EPOCH + TimeSpan.FromSeconds(ticks);

            return time;
        }
        
        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            var format = GetXMLAttributeValue(handLines[0], "GAMEKIND");

            switch (format)
            {
                case "GAMEKIND_CASH":
                    return PokerFormat.CashGame;

                case "GAMEKIND_SITGO":
                    return PokerFormat.SitAndGo;

                case "GAMEKIND_TOURNAMENT":
                    return PokerFormat.MultiTableTournament;

                default:
                    throw new ArgumentOutOfRangeException("Unknown PokerFormat: " + format);
            }

        }

        protected override long ParseHandId(string[] handLines)
        {
            string ID = GetXMLAttributeValue(handLines[0], "ID");

            return long.Parse(ID);
        }

        protected override long ParseTournamentId(string[] handLines)
        {
            throw new NotImplementedException();
        }

        protected override string ParseTableName(string[] handLines)
        {
            return GetXMLAttributeValue(handLines[0], "TABLE");
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            return SeatType.FromMaxPlayers(ParsePlayers(handLines).Count, false);
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            string game = GetXMLAttributeValue(handLines[0], "GAME").Substring(5);
            string limit = GetXMLAttributeValue(handLines[0], "LIMIT");
            switch (limit)
            {
                case "PL":
                    switch (game)
                    {
                        case "OMA":
                            return GameType.PotLimitOmaha;
                        case "OMAHL":
                            return GameType.PotLimitOmahaHiLo;
                        case "THM":
                            return GameType.PotLimitHoldem;
                    }
                    break;
                case "NL":
                    switch (game)
                    {
                        case "OMA":
                            return GameType.NoLimitOmaha;
                        case "OMAHL":
                            return GameType.NoLimitOmahaHiLo;
                        case "THM":
                            return GameType.NoLimitHoldem;
                    }
                    break;
                case "FL":
                    switch (game)
                    {
                        case "OMA":
                            return GameType.FixedLimitOmaha;
                        case "OMAHL":
                            return GameType.FixedLimitOmahaHiLo;
                        case "THM":
                            return GameType.FixedLimitHoldem;
                    }
                    break;
            }
            throw new ArgumentException("UNKOWN GameType: Limit: " + limit + " Game: " + game);
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            string limitStr = GetXMLAttributeValue(handLines[0], "STAKES");

            int splitIndex = limitStr.IndexOf('/');

            string smallBlind = limitStr.Substring(0, splitIndex);
            string bigBlind = limitStr.Substring(splitIndex + 1);

            Limit limit = Limit.FromSmallBlindBigBlind(decimal.Parse(smallBlind, provider), decimal.Parse(bigBlind, provider), Currency.USD);
            return limit;
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            return null;
        }

        public override bool IsValidHand(string[] handLines)
        {
            if (handLines[0].StartsWith("ID=\"") || handLines[0].StartsWith("<HIST"))
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
            List<HandAction> actions = new List<HandAction>();
            int lineParseIndex = getHandActionsStartIndex(handLines);
            Street currentStreet = Street.Preflop;
            int showdownLine = -1;

            for (int i = lineParseIndex; i < handLines.Length; i++)
            {
                string Line = handLines[i];
                char firstChar = Line[1];
                //<ACTION TYPE="HAND_BLINDS" PLAYER="xxpppxx" KIND="HAND_SB" VALUE="100.00"></ACTION>
                if (firstChar == 'A')
                {
                    char actionType = Line[14];
                    switch (actionType)
                    {
                        //<ACTION TYPE="ACTION_
                        case 'A':
                            actions.Add(ParseAction(Line, currentStreet, actions));
                            break;

                        //<ACTION TYPE="HAND_
                        case 'H':
                             //Possible types:
                            //HAND_BOARD
                            //HAND_DEAL
                            //HAND_BLINDS
                            //HAND_ANTE
                            char handAction = Line[20]; //The 7th character is used for identification
                            switch (handAction)
                            {
                                //<ACTION TYPE="HAND_BOARD" VALUE="BOARD_RIVER" POT="29.26" RAKE="0.74" MAINPOT="29.26" LEFTPOT="" RIGHTPOT="">
                                case 'O':
                                    currentStreet = ParseNextStreet(Line);
                                    break;

                                //HAND_DEAL - this is dealt with in ParsePlayers
                                case 'E':
                                    continue;

                                case 'N':
                                    actions.Add(ParseAnte(Line));
                                    break;

                                //HAND_BLINDS
                                case 'L':
                                    actions.Add(ParseBlinds(Line));
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("Unkown HAND_ action: " + handAction + " - " + Line);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Unkown actiontype: " + actionType + " - " + Line);
                    }
                    continue;
                }
                //<CARD LINK="13"></CARD>
                else if (firstChar == 'C')
                {
                    continue;
                }
                //<SHOWDOWN NAME="HAND_SHOWDOWN" POT="29.26" RAKE="0.74" MAINPOT="29.26" LEFTPOT="" RIGHTPOT="">
                else if (firstChar == 'S')
                {
                    showdownLine = i;
                    break;
                }
                else throw new NotImplementedException("Unhandled first char: " + firstChar  + " - " + handLines[i]);
            }

            if (showdownLine != -1)
            {
                //Parse Winners
                for (int i = showdownLine + 1; i < handLines.Length; i++)
                {
                    string Line = handLines[i];
                    //Normal
                    //<RESULT PLAYER="ammms" WIN="3.64" HAND="$(STR_G_WIN_PAIR) $(STR_G_CARDS_NINES)">
                    //OmahaHiLo
                    //<RESULT WINTYPE="WINTYPE_HILO" PLAYER="ItalyToast" WIN="105.08" HAND="$(STR_BY_DEFAULT)" WINCARDS="" HANDEXT=" 8,7,5,2,A">
                    if (Line[1] == 'R')
                    {
                        string playerName = GetXMLAttributeValue(Line, "PLAYER");

                        decimal amount = Decimal.Parse(GetXMLAttributeValue(Line, "WIN"), provider);

                        if (amount > 0)
                        {
                            actions.Add(new WinningsAction(playerName, HandActionType.WINS, amount, 0));
                        }
                        else
                        {
                            const string MuckHand =  "$(STR_G_MUCK)";
                            string hand = GetXMLAttributeValue(Line, "HAND");
                            if (hand == MuckHand)
                            {
                                actions.Add(new HandAction(playerName, HandActionType.MUCKS, amount, Street.Showdown));
                            }
                        }
                    }
                }
            }

            return actions;
        }

        static HandAction ParseAnte(string line)
        {
            var playerName = GetXMLAttributeValue(line, "PLAYER");

            var amountStr = GetXMLAttributeValue(line, "VALUE");

            var amount = decimal.Parse(amountStr, provider);

            return new HandAction(playerName, HandActionType.ANTE, amount, Street.Preflop);
        }

        static Street ParseNextStreet(string Line)
        {
            //<ACTION TYPE="HAND_BOARD" VALUE="BOARD_RIVER" POT="29.26" RAKE="0.74" MAINPOT="29.26" LEFTPOT="" RIGHTPOT="">
            const int StreetIdentiFierIndex = 39;
            char streetIdentifier = Line[StreetIdentiFierIndex];
            switch (streetIdentifier)
            {
                case 'F':
                    return Street.Flop;
                case 'T':
                    return Street.Turn;
                case 'R':
                    return Street.River;
                default:
                    throw new ArgumentException("Unknown street identifier: " + streetIdentifier + " - " + Line);
            }
        }

        public static HandAction ParseBlinds(string Line)
        {
            //<ACTION TYPE="HAND_BLINDS" PLAYER="fatima1975" KIND="HAND_SB" VALUE="0.02"></ACTION>
            //<ACTION TYPE="HAND_BLINDS" PLAYER="gasmandean" KIND="HAND_BB" VALUE="0.04"></ACTION>
            const int playerNameStartIndex = 35;

            int playerNameEndIndex = Line.IndexOf('\"', playerNameStartIndex);
            string playerName = Line.Substring(playerNameStartIndex, playerNameEndIndex - playerNameStartIndex);

            string amountStr = GetXMLAttributeValue(Line, "VALUE");
            decimal amount = decimal.Parse(amountStr, provider);

            char blindType = GetXMLAttributeValue(Line, "KIND")[5];
            switch (blindType)
            {
                case 'S':
                    return new HandAction(playerName, HandActionType.SMALL_BLIND, amount, Street.Preflop);
                case 'B':
                    return new HandAction(playerName, HandActionType.BIG_BLIND, amount, Street.Preflop);
                case 'D':
                    return new HandAction(playerName, HandActionType.POSTS, amount, Street.Preflop);
                default:
                    throw new ArgumentException("Unknown blindType: " + blindType + " - " + Line);
            }
        }

        static HandAction ParseAction(string Line, Street currentStreet, List<HandAction> actions)
        {
            const int playerHandActionStartIndex = 21;
            const int fixedAmountDistance = 9;

            char handActionType = Line[playerHandActionStartIndex];
            int playerNameStartIndex;
            string playerName;

            switch (handActionType)
            {
                //<ACTION TYPE="ACTION_ALLIN" PLAYER="SAMERRRR" VALUE="15972.51"></ACTION>
                case 'A':
                    playerNameStartIndex = playerHandActionStartIndex + 15;
                    playerName = GetActionPlayerName(Line, playerNameStartIndex);
                    decimal amount = GetActionAmount(Line, playerNameStartIndex + playerName.Length + fixedAmountDistance);
                    HandActionType allInType = AllInActionHelper.GetAllInActionType(playerName, amount, currentStreet, actions);
                    if (allInType == HandActionType.CALL)
                    {
                        amount = AllInActionHelper.GetAdjustedCallAllInAmount(amount, actions.Player(playerName));
                    }

                    return new HandAction(playerName, allInType, amount, currentStreet, true);

                //<ACTION TYPE="ACTION_BET" PLAYER="ItalyToast" VALUE="600.00"></ACTION>
                case 'B':
                    playerNameStartIndex = playerHandActionStartIndex + 13;
                    playerName = GetActionPlayerName(Line, playerNameStartIndex);
                    return new HandAction(
                        playerName,
                        HandActionType.BET,
                        GetActionAmount(Line, playerNameStartIndex + playerName.Length + fixedAmountDistance),
                        currentStreet
                        );

                //<ACTION TYPE="ACTION_CHECK" PLAYER="gasmandean"></ACTION>
                //<ACTION TYPE="ACTION_CALL" PLAYER="fatima1975" VALUE="0.04"></ACTION>
                case 'C':
                    if (Line[playerHandActionStartIndex + 1] == 'H')
                    {
                        playerNameStartIndex = playerHandActionStartIndex + 15;
                        playerName = GetActionPlayerName(Line, playerNameStartIndex);
                        return new HandAction(
                        playerName,
                        HandActionType.CHECK,
                        0,
                        currentStreet
                        );
                    }
                    else
                    {
                        playerNameStartIndex = playerHandActionStartIndex + 14;
                        playerName = GetActionPlayerName(Line, playerNameStartIndex);
                        return new HandAction(
                        playerName,
                        HandActionType.CALL,
                        GetActionAmount(Line, playerNameStartIndex + playerName.Length + fixedAmountDistance),
                        currentStreet
                        );
                    }

                //<ACTION TYPE="ACTION_FOLD" PLAYER="Belanak"></ACTION>
                case 'F':
                    playerNameStartIndex = playerHandActionStartIndex + 14;
                    playerName = GetActionPlayerName(Line, playerNameStartIndex);
                    return new HandAction(
                        playerName,
                        HandActionType.FOLD,
                        0,
                        currentStreet
                        );

                //<ACTION TYPE="ACTION_RAISE" PLAYER="ItalyToast" VALUE="400.00"></ACTION>
                case 'R':
                    playerNameStartIndex = playerHandActionStartIndex + 15;
                    playerName = GetActionPlayerName(Line, playerNameStartIndex);
                    return new HandAction(
                        playerName,
                        HandActionType.RAISE,
                        GetActionAmount(Line, playerNameStartIndex + playerName.Length + fixedAmountDistance),
                        currentStreet
                        );

                default:
                    throw new ArgumentOutOfRangeException("Unkown hand action: " + handActionType + " - " + Line);
            }
        }

        static decimal GetActionAmount(string Line, int startIndex)
        {
            int endIndex = Line.IndexOf('\"', startIndex);
            string amountString = Line.Substring(startIndex, endIndex - startIndex);
            return decimal.Parse(amountString, provider);
        }

        static string GetActionPlayerName(string Line, int startIndex)
        {
            int endIndex = Line.IndexOf('\"', startIndex);
            return Line.Substring(startIndex, endIndex - startIndex);
        }

        private int getHandActionsStartIndex(string[] handLines)
        {
            for (int i = 1; i < handLines.Length; i++)
            {
                char firstChar = handLines[i][1];
                if (firstChar != 'P')
                {
                    return i;
                }
            }
            throw new ArgumentOutOfRangeException("Did not find start of hand actions");
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            int currentLine = 1;
            PlayerList plist = new PlayerList();
            //Parsing playerlist
            for (int i = 0; i < 10; i++)
            {
                //<PLAYER NAME="fatima1975" SEAT="6" AMOUNT="4.27"></PLAYER>
                string Line = handLines[i + 1];
                if (Line[1] != 'P')
                {
                    currentLine = i + 1;
                    break;
                }

                const int playerNameStartIndex = 14;
                int playerNameEndIndex = Line.IndexOf('\"', playerNameStartIndex);
                string playerName = Line.Substring(playerNameStartIndex, playerNameEndIndex - playerNameStartIndex);

                if (playerName == "UNKNOWN")
                {
                    continue;
                }

                int seatStartIndex = playerNameEndIndex + 8;
                int seatEndIndex = Line.IndexOf('\"', seatStartIndex);
                int seatNumber = int.Parse(Line.Substring(seatStartIndex, seatEndIndex - seatStartIndex));

                int stackStartIndex = seatEndIndex + 10;
                int stackEndIndex = Line.IndexOf('\"', stackStartIndex);
                decimal stack = decimal.Parse(Line.Substring(stackStartIndex, stackEndIndex - stackStartIndex), provider);

                plist.Add(new Player(playerName, stack, seatNumber));
            }

            //Parsing dealt cards
            for (int i = currentLine; i < handLines.Length; i++)
            {
                string Line = handLines[i];
                char firstChar = Line[1];
                if (firstChar == 'A')
                {
                    //<ACTION TYPE="HAND_BLINDS" PLAYER="ItalyToast" KIND="HAND_BB" VALUE="200.00"></ACTION>
                    //<ACTION TYPE="HAND_DEAL" PLAYER="AllinAnna">
                    const int actionTypeCharIndex = 19;
                    char actionTypeChar = Line[actionTypeCharIndex];
                    if (actionTypeChar == 'D')
                    {
                        string playerName = GetXMLAttributeValue(Line, "PLAYER");
                        ParseDealtHand(handLines, i, plist[playerName]);
                    }
                }
                if (firstChar == 'S')
                {
                    currentLine = i + 1;
                    break;
                }
            }

            //Parse Showdown cards
            for (int i = handLines.Length - 1; i > currentLine; i--)
            {
                string Line = handLines[i];
                char firstChar = Line[1];

                if (firstChar == 'C')
                {
                    continue;
                }

                //<RESULT PLAYER="ItalyToast" WIN="10.00" HAND="$(STR_BY_DEFAULT)" WINCARDS="14 1 50 5 14 ">
                if (firstChar == 'R')
                {
                    const int playerNameStartIndex = 16;
                    int playerNameEndIndex = Line.IndexOf('\"', playerNameStartIndex);
                    string playerName = GetXMLAttributeValue(Line, "PLAYER");
                    Player player = plist[playerName];

                    if (!player.hasHoleCards)
                    {
                        for (int cardIndex = i + 1; cardIndex <= i + 4 && cardIndex < handLines.Length; cardIndex++)
                        {
                            string cardLine = handLines[cardIndex];
                            if (cardLine[1] != 'C')
                            {
                                break;
                            }

                            Card parsedCard = ParseCard(cardLine);
                            if (!parsedCard.isEmpty)
                            {
                                if (player.HoleCards == null)
                                {
                                    player.HoleCards = HoleCards.NoHolecards();
                                }
                                player.HoleCards.AddCard(parsedCard);
                            }
                        }
                    }
                }

                if (firstChar == 'S')
                {
                    break;
                }
            }
            return plist;
        }

        static void ParseDealtHand(string[] handLines, int currentLine, Player player)
        {
            const int maxCards = 4;
            for (int i = 1; i <= maxCards; i++)
            {
                string Line = handLines[currentLine + i];
                if (Line[1] != 'C')
                {
                    break;
                }

                Card parsedCard = ParseCard(Line);
                if (!parsedCard.isEmpty)
                {
                    if (player.HoleCards == null)
                    {
                        player.HoleCards = HoleCards.NoHolecards();
                    }
                    player.HoleCards.AddCard(parsedCard);
                }
            }
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            //<ACTION TYPE="HAND_BOARD" VALUE="BOARD_RIVER" POT="3.64">
            //<CARD LINK="30"></CARD>
            //<CARD LINK="35"></CARD>
            //<CARD LINK="20"></CARD>
            //<CARD LINK="8"></CARD>
            //<CARD LINK="44"></CARD></ACTION>
            BoardCards board = BoardCards.ForPreflop();
            for (int i = handLines.Length - 1; i > 1; i--)
            {
                string Line = handLines[i];
                if (Line[1] == 'A' && Line[14] == 'H' && Line[20] == 'O')
                {
                    const int maxCards = 5;
                    for (int cardIndex = i + 1; cardIndex <= i + maxCards; cardIndex++)
                    {
                        if (handLines[cardIndex][1] != 'C')
                        {
                            break;
                        }
                        board.AddCard(ParseCard(handLines[cardIndex]));
                    }
                    break;
                }
            }
            return board;
        }

        static Card[] BossCardLookup = new Card[]
        {
            #region Diamonds 0-12
            new Card('A', 'd'),
		    new Card('2', 'd'),
            new Card('3', 'd'),
            new Card('4', 'd'),
            new Card('5', 'd'),
            new Card('6', 'd'),
            new Card('7', 'd'),
            new Card('8', 'd'),
            new Card('9', 'd'),
            new Card('T', 'd'),
            new Card('J', 'd'),
            new Card('Q', 'd'),
            new Card('K', 'd'),
	        #endregion
            #region Clubs 13-25
            new Card('A', 'c'), 
		    new Card('2', 'c'),
            new Card('3', 'c'),
            new Card('4', 'c'),
            new Card('5', 'c'),
            new Card('6', 'c'),
            new Card('7', 'c'),
            new Card('8', 'c'),
            new Card('9', 'c'),
            new Card('T', 'c'),
            new Card('J', 'c'),
            new Card('Q', 'c'),
            new Card('K', 'c'),
	        #endregion
            #region Hearts 26-38
            new Card('A', 'h'), 
		    new Card('2', 'h'),
            new Card('3', 'h'),
            new Card('4', 'h'),
            new Card('5', 'h'),
            new Card('6', 'h'),
            new Card('7', 'h'),
            new Card('8', 'h'),
            new Card('9', 'h'),
            new Card('T', 'h'),
            new Card('J', 'h'),
            new Card('Q', 'h'),
            new Card('K', 'h'),
	        #endregion
            #region Spades 39-51
            new Card('A', 's'),
		    new Card('2', 's'),
            new Card('3', 's'),
            new Card('4', 's'),
            new Card('5', 's'),
            new Card('6', 's'),
            new Card('7', 's'),
            new Card('8', 's'),
            new Card('9', 's'),
            new Card('T', 's'),
            new Card('J', 's'),
            new Card('Q', 's'),
            new Card('K', 's'),
	        #endregion
        };

        static Card ParseCard(string Line)
        {
            //<CARD LINK="b"></CARD> is unkown card
            //<CARD LINK="13"></CARD>
            //LINK 1 - 13 is diamonds
            //LINK 14 - 26 is clubs
            //LINK 27 - 39 is hearts
            //LINK 40 - 52 is spades
            const int cardIDStartIndex = 12;
            int cardIDEndIndex = Line.IndexOf('\"', cardIDStartIndex);
            string cardString = Line.Substring(cardIDStartIndex, cardIDEndIndex - cardIDStartIndex);
            if (cardString == "b")
	        {
		        return new Card();
            }
            int cardID = int.Parse(cardString);

            return BossCardLookup[cardID];
        }

        /// <summary>
        /// tries to find a XML attribute
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="Name"></param>
        /// <returns>return the value of the attribute, if an attribute is not found, it returns null</returns>
        static string GetXMLAttributeValue(string Line, string Name)
        {
            string search = " " + Name + "=\"";
            int startIndex = Line.IndexOf(search);

            if (startIndex == -1)
            {
                return null;
            }

            startIndex += search.Length;
            int endIndex = Line.IndexOf('\"', startIndex);
            return Line.Substring(startIndex, endIndex - startIndex);
        }

        protected override void ParseExtraHandInformation(string[] handLines, Objects.Hand.HandHistorySummary handHistorySummary)
        {
            //Expected Line: <SHOWDOWN NAME="HAND_SHOWDOWN" POT="3.64" RAKE="0.40">

            for (int i = handLines.Length - 1; i > 0; i--)
            {
                string line = handLines[i];
                if (line.StartsWith("<SHOWDOWN", StringComparison.Ordinal))
                {
                    var TotalPot = GetXMLAttributeValue(line, "POT");
                    var Rake = GetXMLAttributeValue(line, "RAKE");

                    handHistorySummary.TotalPot = decimal.Parse(TotalPot, provider);
                    handHistorySummary.Rake = decimal.Parse(Rake, provider);
                }
            }
        }

        protected override string ParseHeroName(string[] handlines)
        {
            //Known card
            //<CARD LINK="2"></CARD>
            //Unkwown card
            //<CARD LINK="b"></CARD>

            for (int i = 0; i < handlines.Length; i++)
            {
                string line = handlines[i];
                if (line[1] == 'C' && line[12] != 'b')
                {
                    string HeroNameLine = handlines[i - 1];
                    return GetXMLAttributeValue(HeroNameLine, "PLAYER");
                }
            }
            return null;
        }
    }
}
