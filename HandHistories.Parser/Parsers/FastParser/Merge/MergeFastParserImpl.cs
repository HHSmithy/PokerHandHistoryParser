using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;

namespace HandHistories.Parser.Parsers.FastParser.Merge
{
    public class MergeFastParserImpl : HandHistoryParserFastImpl
    {
        public override SiteName SiteName
        {
            get { return SiteName.Merge; }
        }

        public override bool RequresAdjustedRaiseSizes
        {
            get { return true; }
        }

        protected XDocument GetXDocumentFromLines(string[] handLines)
        {
            string handString = string.Join("", handLines);

            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.ConformanceLevel = ConformanceLevel.Fragment;

            XDocument doc = new XDocument(new XElement("root"));
            XElement root = doc.Descendants().First();

            using (StringReader fs = new StringReader(handString))
            using (XmlReader xr = XmlReader.Create(fs, xrs))
            {
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        root.Add(XElement.Load(xr.ReadSubtree()));
                    }
                }
            }

            return doc;
        }

        protected XElement GetGameElementFromXDocument(XDocument document)
        {
            XElement gameElement = document.Element("root").Element("game");
            return gameElement;
        }

        protected XElement GetPlayersElementFromXDocument(XDocument document)
        {
            XElement playersElement = document.Element("root").Element("game").Element("players");
            return playersElement;
        }

        private static readonly Regex DescriptionGameRegex = new Regex("(<description.*?</game>)|(<game.*?</game>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.Multiline);
        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            MatchCollection gameMatches = DescriptionGameRegex.Matches(rawHandHistories);
            foreach (Match gameMatch in gameMatches)
            {
                string fullGameString = gameMatch.Value + "\r\n";
                yield return fullGameString;
            }
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            /*
             * <game>
                <players dealer="8">                 
             */
            XDocument document = GetXDocumentFromLines(handLines);
            XElement playersElement = GetPlayersElementFromXDocument(document);

            string dealerSeatString = playersElement.Attribute("dealer").Value;

            return Int32.Parse(dealerSeatString);
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            /*<game id="53363692-2070" starttime="20120417015848" numholecards="2" gametype="2" seats="9" realmoney="true" data="20120417|Yellowstone (53363692)|53363692|53363692-2070|false"> */
            /* or
             *<game id="58318258-794" starttime="2012/07/14 12:47:39 -0700" numholecards="2" gametype="2" seats="6" realmoney="true" data="20120714|Waimea Bay|58318258|58318258-794|false"> */

            XDocument document = GetXDocumentFromLines(handLines);
            XElement gameElement = GetGameElementFromXDocument(document);

            string startTime = gameElement.Attribute("starttime").Value;
            string dateString = startTime;

            if (startTime.Length == 14)
            {
                string yearString = dateString.Substring(0, 4);
                string monthString = dateString.Substring(4, 2);
                string dayString = dateString.Substring(6, 2);
                string hourString = dateString.Substring(8, 2);
                string minuteString = dateString.Substring(10, 2);
                string secondString = dateString.Substring(12, 2);

                return new DateTime(Int32.Parse(yearString), Int32.Parse(monthString), Int32.Parse(dayString),
                                    Int32.Parse(hourString), Int32.Parse(minuteString), Int32.Parse(secondString));
            }
            else
            {
                string yearString = dateString.Substring(0, 4);
                string monthString = dateString.Substring(5, 2);
                string dayString = dateString.Substring(8, 2);
                string hourString = dateString.Substring(11, 2);
                string minuteString = dateString.Substring(14, 2);
                string secondString = dateString.Substring(17, 2);

                return new DateTime(Int32.Parse(yearString), Int32.Parse(monthString), Int32.Parse(dayString),
                                    Int32.Parse(hourString), Int32.Parse(minuteString), Int32.Parse(secondString));
            }
        }

        // For now (4/17/2012) only need Game # in Miner and using Regexes. Will convert to faster mechanism soon.
        private static Regex HandIdRegex = new Regex("(?<=game id=\")[0-9]+[-][0-9]+", RegexOptions.Compiled);
        protected override long ParseHandId(string[] handLines)
        {
            foreach (var handLine in handLines)
            {
                var match = HandIdRegex.Match(handLine);
                if (match.Success)
                {
                    return long.Parse(match.Value.Replace("-", ""));
                }
            }

            throw new HandIdException(handLines[1], "Couldn't find handid");
        }

        protected override string ParseTableName(string[] handLines)
        {
            /*
                <game id="56067014-1756" starttime="20120529031347" numholecards="2" gametype="2" seats="9" realmoney="true" data="20120529|Baja (56067014)|56067014|56067014-1756|false">             
             */
            string gameLine = handLines[1].TrimStart();
            int nameStartIndex = gameLine.IndexOf("|") + 1;
            int nameStartEndIndex = gameLine.IndexOf("|", nameStartIndex);
            string tableName = gameLine.Substring(nameStartIndex, nameStartEndIndex - nameStartIndex);
            return tableName;
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            XDocument document = GetXDocumentFromLines(handLines);

            int numPlayers = GetPlayersElementFromXDocument(document).Elements().Count();

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

        protected override GameType ParseGameType(string[] handLines)
        {
            string gameTypeLine = handLines[0].TrimStart();

            char typeCharacter = gameTypeLine[19];

            if (typeCharacter == 'O')
            {
                //We are Omaha
                char subTypeCharacter = gameTypeLine[34];
                if (subTypeCharacter == 'P')
                {
                    return GameType.PotLimitOmaha;
                }
            }

            if (typeCharacter == 'H')
            {
                //We are Holdem
                char subTypeCharacter = gameTypeLine[35];
                if (subTypeCharacter == 'N')
                {
                    return GameType.NoLimitHoldem;
                }
                else if (subTypeCharacter == 'L')
                {
                    return GameType.FixedLimitHoldem;
                }
            }

            throw new Exception(string.Format("Unknown gametype: {0}" + gameTypeLine));
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            string tableName = ParseTableName(handLines);

            if (tableName.StartsWith("Bad Beat"))
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Jackpot);
            }

            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            //<description type="Holdem" stakes="No Limit ($0.05/$0.10)"/>

            XDocument document = GetXDocumentFromLines(handLines);
            XElement descriptionElement = document.Element("root").Element("description");

            string limitString = descriptionElement.Attribute("stakes").Value;

            int smallBlindStartIndex = limitString.IndexOf('$') + 1;
            int smallBlindEndIndex = limitString.IndexOf('/');
            string smallBlindString = limitString.Substring(smallBlindStartIndex,
                                                            smallBlindEndIndex - smallBlindStartIndex);

            int bigBlindStartIndex = smallBlindEndIndex + 2;
            int bidBlindEndIndex = limitString.LastIndexOf(')');

            //10/20 doesn't end with a ), but it does end the string
            if (bidBlindEndIndex == -1)
            {
                bidBlindEndIndex = limitString.Length;
            }

            string bigBlindString = limitString.Substring(bigBlindStartIndex, bidBlindEndIndex - bigBlindStartIndex);

            //All Merge limits are USD
            return Limit.FromSmallBlindBigBlind(decimal.Parse(smallBlindString), decimal.Parse(bigBlindString),
                                                Currency.USD);
        }

        public override bool IsValidHand(string[] handLines)
        {
            return handLines[handLines.Length - 1].Contains("</game>");
        }

        protected Street GetStreetFromRoundElement(XElement roundElement)
        {
            string roundId = roundElement.Attribute("id").Value;

            switch (roundId)
            {
                case "BLINDS":
                case "PREFLOP":
                    return Street.Preflop;
                case "POSTFLOP":
                    return Street.Flop;
                case "POSTTURN":
                    return Street.Turn;
                case "POSTRIVER":
                    return Street.River;
                case "GAME_CANCELLED":
                case "END_OF_FOLDED_GAME":
                case "END_OF_GAME":
                case "SHOWDOWN":
                    return Street.Showdown;
                default:
                    throw new Exception(string.Format("Unknown round ID: {0}", roundId));
            }
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            List<HandAction> actions = new List<HandAction>();

            PlayerList playerList = ParsePlayers(handLines);

            XDocument document = GetXDocumentFromLines(handLines);
            XElement gameElement = GetGameElementFromXDocument(document);

            Street currentStreet = Street.Null;

            List<XElement> roundElements = gameElement.Elements("round").ToList();

            foreach (XElement roundElement in roundElements)
            {
                currentStreet = GetStreetFromRoundElement(roundElement);
                List<XElement> eventElements = roundElement.Elements("event").ToList();

                foreach (XElement eventElement in eventElements)
                {
                    HandAction action = GetHandActionFromEventElement(eventElement, currentStreet, playerList);
                    actions.Add(action);
                }
            }

            List<XElement> winnerElements = gameElement.Elements("round").Elements("winner").ToList();
            foreach (XElement winnerElement in winnerElements)
            {
                WinningsAction winningsAction = GetWinningsActionFromWinnerElement(winnerElement, playerList);
                actions.Add(winningsAction);
            }

            return actions;
        }

        private WinningsAction GetWinningsActionFromWinnerElement(XElement winnerElement, PlayerList playerList)
        {
            int winnerSeatId = Int32.Parse(winnerElement.Attribute("player").Value);
            int potNumber = Int32.Parse(winnerElement.Attribute("potnumber").Value) - 1;
            decimal amount = decimal.Parse(winnerElement.Attribute("amount").Value);

            Player matchingPlayer = GetPlayerBySeatId(playerList, winnerSeatId);

            return new WinningsAction(matchingPlayer.PlayerName, HandActionType.WINS, amount, potNumber, Int32.MaxValue);
        }

        private Player GetPlayerBySeatId(PlayerList playerList, int seatId)
        {
            Player matchingPlayer = playerList.FirstOrDefault(player => player.SeatNumber == seatId);

            //Special case for handling actions without players attached to them
            if (seatId == -1)
            {
                matchingPlayer = new Player("", 0, -1);
            }
            else if (matchingPlayer == null)
            {
                throw new Exception(string.Format("No player corresponding with seatId {0}", seatId));
            }

            return matchingPlayer;
        }

        private Player GetPlayerByName(PlayerList playerList, string playerName)
        {
            Player matchingPlayer = playerList.FirstOrDefault(player => player.PlayerName.Equals(playerName));

            if (matchingPlayer == null)
            {
                throw new Exception(string.Format("No player corresponding with name {0}", playerName));
            }

            return matchingPlayer;

        }

        private HandAction GetHandActionFromEventElement(XElement eventElement, Street currentStreet, PlayerList playerList)
        {
            string actionString = eventElement.Attribute("type").Value;
            int actionNumber = Int32.Parse(eventElement.Attribute("sequence").Value);
            decimal value = 0;
            if (eventElement.Attribute("amount") != null)
            {
                value = decimal.Parse(eventElement.Attribute("amount").Value);
            }


            Player matchingPlayer = GetPlayerBySeatId(playerList, -1);
            if (eventElement.Attribute("player") != null)
            {
                string playerValue = eventElement.Attribute("player").Value;

                if (playerValue.Length > 1)
                {
                    matchingPlayer = GetPlayerByName(playerList, playerValue);
                }
                else
                {
                    int seatId = Int32.Parse(playerValue);
                    matchingPlayer = GetPlayerBySeatId(playerList, seatId);
                }

            }

            string playerName = matchingPlayer.PlayerName;

            HandActionType actionType;
            switch (actionString)
            {
                case "FOLD":
                    actionType = HandActionType.FOLD;
                    break;
                case "SMALL_BLIND":
                    actionType = HandActionType.SMALL_BLIND;
                    break;
                case "BIG_BLIND":
                    actionType = HandActionType.BIG_BLIND;
                    break;
                case "RETURN_BLIND":
                    actionType = HandActionType.RETURNED;
                    break;
                case "INITIAL_BLIND":
                    actionType = HandActionType.POSTS;
                    break;
                case "CALL":
                    actionType = HandActionType.CALL;
                    break;
                case "CHECK":
                    actionType = HandActionType.CHECK;
                    break;
                case "BET":
                    actionType = HandActionType.BET;
                    break;
                case "ALL_IN":
                    return new AllInAction(playerName, value, currentStreet, false, actionNumber);
                case "SHOW":
                    actionType = HandActionType.SHOW;
                    break;
                case "MUCK":
                    actionType = HandActionType.MUCKS;
                    break;
                case "GAME_CANCELLED":
                    actionType = HandActionType.GAME_CANCELLED;
                    break;
                case "SIT_OUT":
                case "SITTING_OUT":
                    actionType = HandActionType.SITTING_OUT;
                    break;
                case "SIT_IN":
                    actionType = HandActionType.RETURNED;
                    break;
                case "RAISE":
                    actionType = HandActionType.RAISE;
                    break;
                case "RABBIT":
                    actionType = HandActionType.RABBIT;
                    break;
                case "CHAT":
                    actionType = HandActionType.CHAT;
                    break;
                default:
                    throw new Exception(string.Format("Encountered unknown Action Type: {0} w/ line \r\n{1}", actionString, eventElement));
            }
            return new HandAction(playerName, actionType, value, currentStreet, actionNumber);
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            XDocument document = GetXDocumentFromLines(handLines);

            XElement gameElement = GetGameElementFromXDocument(document);
            XElement players = gameElement.Element("players");

            PlayerList playerList = new PlayerList();

            //Build a query for all cards elements which are "SHOWN" or "MUCKED" rather than "COMMUNITY"
            IEnumerable<XElement> cardElements = gameElement.Elements("round").Elements("cards").Where(element => !element.Attribute("type").Value.StartsWith("C")).ToList();

            foreach (XElement playerElement in players.Elements())
            {
                //Player Element looks like:
                //<player seat="0" nickname="GODEXISTSJK" balance="$269.96" dealtin="true" />
                decimal stack = decimal.Parse(playerElement.Attribute("balance").Value.Substring(1));
                string playerName = playerElement.Attribute("nickname").Value;
                int seat = Int32.Parse(playerElement.Attribute("seat").Value);
                Player player = new Player(playerName, stack, seat);

                bool dealtIn = bool.Parse(playerElement.Attribute("dealtin").Value);
                player.IsSittingOut = !dealtIn;

                //<cards type="SHOWN" cards="Ac,4c" player="7"/>
                XElement playerCardElement =
                    cardElements.FirstOrDefault(card => Int32.Parse(card.Attribute("player").Value) == seat);

                if (playerCardElement != null)
                {
                    string cardString = playerCardElement.Attribute("cards").Value;
                    player.HoleCards.AddCards(BoardCards.Parse(cardString));
                }

                playerList.Add(player);
            }

            return playerList;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            XDocument document = GetXDocumentFromLines(handLines);
            XElement gameElement = GetGameElementFromXDocument(document);
            IEnumerable<XElement> cardElements = gameElement.Elements("round").Elements("cards");

            //Find the last <card> element with type="community". This will have our board card list as its cards attribute value
            XElement lastCardElement = cardElements.LastOrDefault(element => element.Attribute("type").Value.StartsWith("C"));

            if (lastCardElement == null)
            {
                return BoardCards.ForPreflop();
            }

            string boardCards = lastCardElement.Attribute("cards").Value;

            return BoardCards.FromCards(boardCards);
        }
    }
}
