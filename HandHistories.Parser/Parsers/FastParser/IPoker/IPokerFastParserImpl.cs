using HandHistories.Objects;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.FastParsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace HandHistories.Parser.Parsers.FastParser.IPoker
{
    public sealed partial class IPokerFastParserImpl : HandHistoryParserFastImpl
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly bool _isIpoker2;

        public IPokerFastParserImpl(bool isIpoker2 = false)
        {
            _isIpoker2 = isIpoker2;
        }

        public override SiteName SiteName
        {
            get { return (_isIpoker2) ? SiteName.IPoker2 : SiteName.IPoker; }
        }

        public override bool RequiresAdjustedRaiseSizes => true;

        public override bool RequiresActionSorting => true;

        public override bool RequiresAllInDetection => true;

        public override bool RequiresAllInUpdates => true;

        public override bool RequiresTotalPotCalculation => true;

        public override bool RequiresUncalledBetFix => true;

        public override bool RequiresUncalledBetWinAdjustment => true;

        protected override string[] SplitHandsLines(string handText)
        {
            return Utils.XMLHandLineSplitter.Split(handText);
        }

        /*
            <player seat="3" name="RodriDiaz3" chips="$2.25" dealer="0" win="$0" bet="$0.08" rebuy="0" addon="0" />
            <player seat="8" name="Kristi48ru" chips="$6.43" dealer="1" win="$0.23" bet="$0.16" rebuy="0" addon="0" />
        */

        static int GetSeatNumberFromPlayerLine(string playerLine)
        {
            int seatOffset = playerLine.IndexOfFast(" s") + 7;
            return FastInt.Parse(playerLine, seatOffset);            
        }

        static bool IsPlayerLineDealer(string playerLine)
        {
            int dealerOffset = playerLine.IndexOfFast(" d") + 9;
            int dealerValue = FastInt.Parse(playerLine[dealerOffset]);
            return dealerValue == 1;
        }

        static decimal GetStackFromPlayerLine(string playerLine)
        {
            int stackStartPos = playerLine.IndexOfFast(" c") + 8;
            int stackEndPos = playerLine.IndexOf('"', stackStartPos) - 1;
            string stackString = playerLine.Substring(stackStartPos, stackEndPos - stackStartPos + 1);
            return stackString.ParseAmount();
        }

        static decimal GetWinningsFromPlayerLine(string playerLine)
        {
            int stackStartPos = playerLine.IndexOfFast(" w") + 6;
            int stackEndPos = playerLine.IndexOf('"', stackStartPos) - 1;
            string stackString = playerLine.Substring(stackStartPos, stackEndPos - stackStartPos + 1);
            if (stackString == "")
            {
                return 0;
            }
            return stackString.ParseAmount();
        }

        static string GetNameFromPlayerLine(string playerLine)
        {
            int nameStartPos = playerLine.IndexOfFast(" n") + 7;
            int nameEndPos = playerLine.IndexOf('"', nameStartPos) - 1;
            string name = playerLine.Substring(nameStartPos, nameEndPos - nameStartPos + 1);
            return name;
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            List<string> playerLines = GetPlayerLinesFromHandLines(handLines);

            for (int i = 0; i < playerLines.Count; i++)
            {
                string playerLine = playerLines[i];
                if (IsPlayerLineDealer(playerLine))
                {
                    return GetSeatNumberFromPlayerLine(playerLine);                    
                }
            }

            throw new Exception("Could not locate dealer");
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            //<startdate>2012-05-28 16:52:05</startdate>
            string dateLine = GetStartDateFromHandLines(handLines);
            int startPos = dateLine.IndexOf('>') + 1;
            int endPos = dateLine.LastIndexOf('<') - 1;
            string dateString = dateLine.Substring(startPos, endPos - startPos + 1);
            DateTime dateTime = DateTime.Parse(dateString);
            //TimeZoneUtil.ConvertDateTimeToUtc(dateTime, TimeZoneType.GMT);

            return dateTime;
        }

        private static readonly Regex SessionGroupRegex = new Regex("<session.*?session>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex GameGroupRegex = new Regex("<game.*?game>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.Multiline);
        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            //Remove XML headers if necessary 
            rawHandHistories = rawHandHistories.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n", "");

            //Two Cases - Case 1, we have a single <session> tag holding multiple <game> tags
            //            Case 2, we have multiple <session> tags each holding a single <game> tag.
            //            We need our enumerable to have only case 2 representations for parsing

            if (rawHandHistories.IndexOfFast("<session") == rawHandHistories.LastIndexOfFast("<session"))
            {
                //We are case 1 - convert to case 2

                int endOfGeneralInfoIndex = rawHandHistories.IndexOfFast("</general>");

                if (endOfGeneralInfoIndex == -1)
                {
                    // log the first 1000 chars of the file, so we can at least guess what's the problem
                    logger.Fatal("IPokerFastParserImpl.SplitUpMultipleHands(): Encountered a weird file\r\n{0}", rawHandHistories.Substring(0,1000));
                }

                string generalInfoString = rawHandHistories.Substring(0, endOfGeneralInfoIndex + 10);

                MatchCollection gameMatches = GameGroupRegex.Matches(rawHandHistories, endOfGeneralInfoIndex + 9);
                foreach (Match gameMatch in gameMatches)
                {
                    string fullGameString = generalInfoString + "\r\n" + gameMatch.Value + "\r\n</session>";
                    yield return fullGameString;
                }
            }
            else
            {
                //We are case 2
                MatchCollection matches = SessionGroupRegex.Matches(rawHandHistories);

                foreach (Match match in matches)
                {
                    yield return match.Value;
                }                
            }
        }
        
        protected override PokerFormat ParsePokerFormat(string[] handLines)
        {
            for (int i = 3; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line[1] == '/')
                {
                    return PokerFormat.CashGame;
                }
                if (line.StartsWithFast("<tournamentname>"))
                {
                    if (line.Contains("SNG"))
                    {
                        return PokerFormat.SitAndGo;
                    }
                    else
                    {
                        return PokerFormat.MultiTableTournament;
                    }
                }
            }

            return PokerFormat.CashGame;
        }

        // For now (4/17/2012) only need Game # in Miner and using Regexes. Will convert to faster mechanism soon.
        private static readonly Regex HandIdRegex = new Regex("(?<=gamecode=\")\\d+", RegexOptions.Compiled);
        protected override long[] ParseHandId(string[] handLines)
        {
            foreach (var handLine in handLines)
            {
                var match = HandIdRegex.Match(handLine);
                if (match.Success)
                {
                    return HandID.Parse(match.Value.Replace("-", ""));
                }
            }

            throw new HandIdException(handLines[1], "Couldn't find handid");
        }

        protected override string ParseTableName(string[] handLines)
        {
            //<tablename>Ambia, 98575671</tablename>
            string tableNameLine = GetTableNameLineFromHandLines(handLines);
            int tableNameStartIndex = tableNameLine.IndexOf('>') + 1;
            int tableNameEndIndex = tableNameLine.LastIndexOf('<') - 1;

            string tableName = tableNameLine.Substring(tableNameStartIndex, tableNameEndIndex - tableNameStartIndex + 1);

            return tableName;
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            List<string> playerLines = GetPlayerLinesFromHandLines(handLines);
            int numPlayers = playerLines.Count;

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

        string GetGameTypeLineFromHandLines(string[] handLines)
        {
            //With the new format the gametype isnt neccesaryily on the same line
            //The old format is on line with index 3
            //Version 17 is is on line with index 6
            for (int i = 3; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line[1] == 'g')
                {
                    return line;
                }
            }

            throw new ArgumentOutOfRangeException("GameType line not found");
        }

        string GetTableNameLineFromHandLines(string[] handLines)
        {
            //With the new format the TableName isnt neccesaryily on the same line
            //The old format is on line with index 4
            //Version 17 is is on line with index 7
            for (int i = 3; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line[1] == 't')
                {
                    return line;
                }
            }

            throw new ArgumentOutOfRangeException("TableName line not found");
        }

        string GetStartDateFromHandLines(string[] handLines)
        {
            // in order to find the exact date of the hand we search the startdate of the hand ( and not the table )
            
            for(int i = 0; i <= handLines.Length; i++)
            {
                if(handLines[i].Contains("gamecode=\""))
                {
                    return handLines[i + 2];
                }
            }

            // if we're unable to find the dateline for the hand, we just use the date for the table
            return handLines[8];
        }

        static List<string> GetPlayerLinesFromHandLines(string[] handLines)
        {
            /*
              Returns all of the detail lines between the <players> tags
              <players> <-- Line offset 22
                <player seat="1" name="Leichenghui" chips="£1,866.23" dealer="1" win="£0" bet="£5" rebuy="0" addon="0" />
                ...
                <player seat="10" name="CraigyColesBRL" chips="£2,297.25" dealer="0" win="£15" bet="£10" rebuy="0" addon="0" />
              </players>             
             */
            int offset = GetFirstPlayerIndex(handLines);
            List<string> playerLines = new List<string>();

            string line = handLines[offset];
            line = line.TrimStart();
            while (offset < handLines.Count() && line[1] != '/')
            {
                playerLines.Add(line);
                offset++;

                if (offset >= handLines.Count())
                    break;

                line = handLines[offset];
                line = line.TrimStart();
            }

            return playerLines;
        }

        static int GetFirstPlayerIndex(string[] handLines)
        {
            for (int i = 18; i < handLines.Length; i++)
            {
                string line = handLines[i];
                if (line[1] == 'p' && line[4] == 'y')
                {
                    return i + 1;
                }
            }
            throw new IndexOutOfRangeException("Did not find first player");
        }

        List<string> GetCardLinesFromHandLines(string[] handLines)
        {
            List<string> cardLines = new List<string>();
            for (int i = 0; i < handLines.Length; i++)
            {
                string handLine = handLines[i];

                //If we don't have these letters at these positions, we're not a hand line
                if (handLine[1] != 'c' || handLine[2] != 'a')
                {
                    continue;
                }

                cardLines.Add(handLine);
            }

            return cardLines;
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            /*
             * NLH <gametype>Holdem NL $2/$4</gametype>  
             * NLH <gametype>Holdem PL $2/$4</gametype>    
             * FL  <gametype>Holdem L $2/$4</gametype>
             * PLO <gametype>Omaha PL $0.50/$1</gametype>
             */

            string gameTypeLine = GetGameTypeLineFromHandLines(handLines);

            //If this is an H we're a Holdem, if O, Omaha
            char gameTypeChar = gameTypeLine[10]; 

            if (gameTypeChar == 'O')
            {
                return GameType.PotLimitOmaha;
            }

            //If this is an N, we're NLH, if L - FL
            char holdemTypeChar = gameTypeLine[17]; 

            if (holdemTypeChar == 'L')
            {
                return GameType.FixedLimitHoldem;
            }

            if (holdemTypeChar == 'N')
            {
                return GameType.NoLimitHoldem;
            }

            if (holdemTypeChar == 'P')
            {
                return GameType.PotLimitHoldem;
            }

            throw new UnrecognizedGameTypeException(gameTypeLine, "Could not parse GameType for hand.");
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            string tableName = ParseTableName(handLines);

            if (tableName.StartsWithFast("(Shallow)"))
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Shallow);
            }

            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            //Limit line format:
            //<gametype>Holdem NL $0.02/$0.04</gametype>
            //Alternative1:
            //<gametype>Holdem NL 0.02/0.04</gametype>
            //<currency>USD</currency>
            string gameTypeLine = GetGameTypeLineFromHandLines(handLines);
            int limitStringBeginIndex = gameTypeLine.LastIndexOf(' ') + 1;
            int limitStringEndIndex = gameTypeLine.LastIndexOf('<') - 1;
            string limitString = gameTypeLine.Substring(limitStringBeginIndex,
                                                        limitStringEndIndex - limitStringBeginIndex + 1);

            char currencySymbol = limitString[0];
            Currency currency;

            switch (currencySymbol)
            {
                case '$':
                    currency = Currency.USD;
                    break;
                case '€':
                    currency = Currency.EURO;
                    break;
                case '£':
                    currency = Currency.GBP;
                    break;
                default:
                    string tagValue = GetCurrencyTagValue(handLines);
                    switch (tagValue)
                    {
                        case "USD":
                            currency = Currency.USD;
                            break;
                        case "GBP":
                            currency = Currency.GBP;
                            break;
                        case "EUR":
                            currency = Currency.EURO;
                            break;
                        default:
                            throw new LimitException(handLines[0], "Unrecognized currency symbol " + currencySymbol);
                    }
                    break;
            }

            int slashIndex = limitString.IndexOf('/');

            if (slashIndex == -1)
            {
                //Tournaments dont have a limit string so we get the blinds from the BB action and SB action
                //this is done in FinalizeHandHistory()
                return null;
            }

            string smallString = limitString.Remove(slashIndex);
            decimal small = smallString.ParseAmount();
 
            string bigString = limitString.Substring(slashIndex + 1);
            decimal big = bigString.ParseAmount();

            return Limit.FromSmallBlindBigBlind(small, big, currency);
        }

        private string GetCurrencyTagValue(string[] handLines)
        {
            for (int i = 0; i < 11; i++)
            {
                string handline = handLines[i];
                if (handline[1] == 'c' && handline[2] == 'u')
                {
                    int endIndex = handline.IndexOf('<', 10);
                    return handline.Substring(10, endIndex - 10);
                }
            }
            return "";
        }

        public override bool IsValidHand(string[] handLines)
        {
            //Check 1 - Are we in a Session Tag
            if (!handLines[0].StartsWithFast("<session"))
            {
                return false;
            }

            //Check 2 - check the end tag
            string lastLine = handLines[handLines.Length - 1];
            if (!lastLine.StartsWithFast("</session")
                && !lastLine.StartsWithFast("</game"))
            {
                return false;
            }

            //Check 3 - Do we have between 2 and 10 players?
            List<string> playerLines = GetPlayerLinesFromHandLines(handLines);
            if (playerLines.Count < 2 || playerLines.Count > 10)
            {
                return false;
            }

            //todo add more checks related to action parsing
            return true;
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            isCancelled = false;
            return IsValidHand(handLines);
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType, out List<WinningsAction> winners)
        {
            List<HandAction> actions = new List<HandAction>();
            winners = new List<WinningsAction>();

            int startRow = GetRoundStart(handLines);// offset + playerLines.Length + 2;

            Street currentStreet = Street.Null;
            
            for (int i = startRow; i < handLines.Length - 2; i++)
            {
                string handLine = handLines[i].TrimStart();

                //If we are starting a new round, update the current street 
                if (handLine[1] == 'r')
                {
                    int roundNumber = GetRoundNumberFromLine(handLine);
                    switch (roundNumber)
                    {
                        case 0:
                        case 1:
                            currentStreet = Street.Preflop;
                            break;
                        case 2:
                            currentStreet = Street.Flop;
                            break;
                        case 3:
                            currentStreet = Street.Turn;
                            break;
                        case 4:
                            currentStreet = Street.River;
                            break;
                        default:
                            throw new Exception("Encountered unknown round number " + roundNumber);
                    }
                }
                //If we're an action, parse the action and act accordingly
                else if (handLine[1] == 'a')
                {
                    HandAction action = ParseHandAction(handLine, currentStreet);
                    if (action != null)
                    {
                        actions.Add(action);
                    }
                }
            }

            //Generate the show card + winnings actions
            actions.AddRange(GetWinningAndShowCardActions(handLines, actions, winners));

            // we need to fix dead money postings at the end
            return FixDeadMoneyPosting(actions);
        }

        private int GetRoundStart(string[] handLines)
        {
            for (int i = 0; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line[1] == 'r')
                {
                    return i;
                }
            }
            throw new ArgumentOutOfRangeException("Round not Found");
        }

        private List<HandAction> FixDeadMoneyPosting(List<HandAction> actions)
        {
            
            var BBActions = actions.Count(p => p.HandActionType == HandActionType.BIG_BLIND);

            if (BBActions >= 2)
            {
                // sort the actions, because regular SB + BB actions are always the first actions ( although might not be the first in the hand history )
                var bigBlindValue = 0.0m;
                actions = actions.OrderBy(t => t.ActionNumber).ToList();

                for (int i = 0; i < actions.Count; i++)
			    {
			        var action = actions[i];

                    if (action.HandActionType != HandActionType.BIG_BLIND)
                    {
                        continue;
                    }

                    if (bigBlindValue == 0.0m)
                    {
                        bigBlindValue = action.Absolute;
                        continue;
                    }

                    if (action.Absolute > bigBlindValue)
                    {
                        var post_amount = bigBlindValue;
                        var dead_amount = action.Absolute - bigBlindValue;

                        var postAction = new HandAction(action.PlayerName, HandActionType.POSTS, post_amount, action.Street, action.ActionNumber);
                        var deadAction = new HandAction(action.PlayerName, HandActionType.POSTS_DEAD, dead_amount, action.Street, action.ActionNumber + 1);

                        PushActionNumbers(actions, i + 1);

                        actions[i] = postAction;
                        actions.Insert(i + 1, deadAction);
                    }
                }
            }
            
            return actions;
        }

        static void PushActionNumbers(List<HandAction> actions, int startIndex)
        {
            for (int i = startIndex; i < actions.Count; i++)
            {
                var action = actions[i];
                action.ActionNumber += 1;
            }
        }

        private List<HandAction> GetWinningAndShowCardActions(string[] handLines, List<HandAction> actions, List<WinningsAction> winners)
        {
            int actionNumber = Int32.MaxValue - 100;

            PlayerList playerList = ParsePlayers(handLines);

            List<HandAction> winningAndShowCardActions = new List<HandAction>();

            foreach (Player player in playerList)
            {
                if (player.hasHoleCards)
                {
                    var folded = actions.Any(p => p.HandActionType == HandActionType.FOLD);
                    if (folded)
                    {
                        continue;
                    }

                    HandAction showCardsAction = new HandAction(player.PlayerName, HandActionType.SHOW, 0, Street.Showdown, actionNumber++);    
                    winningAndShowCardActions.Add(showCardsAction);
                }                
            }

            List<string> playerLines = GetPlayerLinesFromHandLines(handLines);
            for (int i = 0; i < playerLines.Count; i++)
            {
                string playerLine = playerLines[i];
                decimal winnings = GetWinningsFromPlayerLine(playerLine);                
                if (winnings > 0)
                {
                    string playerName = GetNameFromPlayerLine(playerLine);
                    WinningsAction winningsAction = new WinningsAction(playerName, WinningsActionType.WINS, winnings, 0);
                    winners.Add(winningsAction);
                }
            }

            return winningAndShowCardActions;
        }

        public static HandAction ParseHandAction(string line, Street street)
        {
            int actionTypeNumber = GetActionTypeFromActionLine(line);
            string actionPlayerName = GetPlayerFromActionLine(line);
            decimal value = GetValueFromActionLine(line);
            int actionNumber = GetActionNumberFromActionLine(line);
            HandActionType actionType;
            switch (actionTypeNumber)
            {
                case 0:                
                    actionType = HandActionType.FOLD;
                    break;
                case 1:                 
                    actionType = HandActionType.SMALL_BLIND;
                    break;
                case 2:
                    actionType = HandActionType.BIG_BLIND;
                    break;
                case 3:                 
                    actionType = HandActionType.CALL;
                    break;
                case 4:                 
                    actionType = HandActionType.CHECK;
                    break;
                case 5:
                    actionType = HandActionType.BET;
                    break;
                case 6://Both are all-ins but we don't know the difference between them
                case 7://we let the AllInDetection take care of this
                    actionType = HandActionType.ALL_IN;
                    break;
                case 8: //Case 8 is when a player sits out at the beginning of a hand 
                case 9: //Case 9 is when a blind isn't posted - can be treated as sitting out
                    actionType = HandActionType.UNKNOWN;
                    break;
                case 15:
                    actionType = HandActionType.ANTE;
                    break;
                case 23:                 
                    actionType = HandActionType.RAISE;
                    break;
                default:
                    throw new Exception(string.Format("Encountered unknown Action Type: {0} w/ line \r\n{1}", actionTypeNumber, line));
            }

            if (actionType == HandActionType.UNKNOWN)
            {
                return null;
            }
            return new HandAction(actionPlayerName, actionType, value, street, actionNumber);
        }

        static int GetRoundNumberFromLine(string handLine)
        {
            int startPos = handLine.IndexOfFast(" n") + 5;
            int endPos = handLine.IndexOf('"', startPos) - 1;
            string numString = handLine.Substring(startPos, endPos - startPos + 1);
            return Int32.Parse(numString);            
        }

        static int GetActionNumberFromActionLine(string actionLine)
        {
            int actionStartPos = actionLine.IndexOfFast(" n") + 5;
            return FastInt.Parse(actionLine, actionStartPos);
        }

        static string GetPlayerFromActionLine(string actionLine)
        {
            int nameStartPos = actionLine.IndexOfFast(" p") + 9;
            int nameEndPos = actionLine.IndexOf('"', nameStartPos) - 1;
            string name = actionLine.Substring(nameStartPos, nameEndPos - nameStartPos + 1);
            return name;
        }

        static decimal GetValueFromActionLine(string actionLine)
        {
            int startPos = actionLine.IndexOfFast(" s") + 6;
            int endPos = actionLine.IndexOf('"', startPos) - 1;
            string value = actionLine.Substring(startPos, endPos - startPos + 1);
            return value.ParseAmount();
        }

        static int GetActionTypeFromActionLine(string actionLine)
        {
            int actionStartPos = actionLine.IndexOfFast(" t") + 7;
            return FastInt.Parse(actionLine, actionStartPos);
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            /*
                <player seat="3" name="RodriDiaz3" chips="$2.25" dealer="0" win="$0" bet="$0.08" rebuy="0" addon="0" />
                <player seat="8" name="Kristi48ru" chips="$6.43" dealer="1" win="$0.23" bet="$0.16" rebuy="0" addon="0" />
                or
                <player seat="5" name="player5" chips="$100000" dealer="0" win="$0" bet="$0" />
             */

            List<string> playerLines = GetPlayerLinesFromHandLines(handLines);

            PlayerList playerList = new PlayerList();

            for (int i = 0; i < playerLines.Count; i++)
            {
                string playerName = GetNameFromPlayerLine(playerLines[i]);
                decimal stack = GetStackFromPlayerLine(playerLines[i]);
                int seat = GetSeatNumberFromPlayerLine(playerLines[i]);
                playerList.Add(new Player(playerName, stack, seat)
                                   {
                                       IsSittingOut = true
                                   });
            }

            var actionLines = handLines.Where(p => p.StartsWithFast("<act"));
            foreach (var line in actionLines)
            {
                string player = GetPlayerFromActionLine(line);
                int type = GetActionTypeFromActionLine(line);

                //action line may have an empty name and then we skip it
                //<action no="3" sum="€0" cards="" type="9" player=""/>
                if (type != 8 && player != "")
                {
                    playerList[player].IsSittingOut = false;
                }
            }

            /* 
             * Grab known hole cards for players and add them to the player
             * <cards type="Pocket" player="pepealas5">CA CK</cards>
             */

            List<string> cardLines = GetCardLinesFromHandLines(handLines);

            for (int i = 0; i < cardLines.Count; i++)
            {
                string line = cardLines[i];

                //Getting the cards Type
                int typeIndex = line.IndexOfFast("e=\"", 10) + 3;
                char typeChar = line[typeIndex];

                //We only care about Pocket Cards
                if (typeChar != 'P')
                {
                    continue;
                }

                //When players fold, we see a line: 
                //<cards type="Pocket" player="pepealas5">X X</cards>
                //or:
                //<cards type="Pocket" player="playername"></cards>
                //We skip these lines
                if (line[line.Length - 9] == 'X' || line[line.Length - 9] == '>')
                {
                    continue;
                }

                int playerNameStartIndex = line.IndexOfFast("r=\"", 10) + 3;
                int playerNameEndIndex = line.IndexOf('"', playerNameStartIndex) - 1;
                string playerName = line.Substring(playerNameStartIndex,
                                                       playerNameEndIndex - playerNameStartIndex + 1);
                Player player = playerList.First(p => p.PlayerName.Equals(playerName));


                int playerCardsStartIndex = line.LastIndexOf('>', line.Length - 11) + 1;
                int playerCardsEndIndex = line.Length - 9;
                string playerCardString = line.Substring(playerCardsStartIndex,
                                                        playerCardsEndIndex - playerCardsStartIndex + 1);

                ////To make sure we know the exact character location of each card, turn 10s into Ts (these are recognized by our parser)
                ////Had to change this to specific cases so we didn't accidentally change player names
                playerCardString = playerCardString.Replace("10", "T");

                if (playerCardString.Length > 1)
                {
                    player.HoleCards = HoleCards.NoHolecards();

                    for (int j = 0; j < playerCardString.Length; j += 3)
                    {
                        char suit = playerCardString[j];
                        char rank = playerCardString[j + 1];

                        player.HoleCards.AddCard(new Card(rank, suit));
                    }
                }
            }

            return playerList;
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            List<Card> boardCards = new List<Card>();
            /* 
             * <cards type="Flop" player="">D6 S9 S7</cards>
             * <cards type="Turn" player="">H8</cards>
             * <cards type="River" player="">D5</cards>
             * <cards type="Pocket" player="pepealas5">CA CK</cards>
             */

            List<string> cardLines = GetCardLinesFromHandLines(handLines);

            for (int i = 0; i < cardLines.Count; i++)
            {
                string handLine = cardLines[i];
                handLine = handLine.TrimStart();

                //To make sure we know the exact character location of each card, turn 10s into Ts (these are recognized by our parser)
                handLine = handLine.Replace("10", "T");

                //The suit/ranks are reversed, so we need to reverse them when adding them to our board card string
                int typeIndex = handLine.IndexOfFast("e=\"");
                char streetChar = handLine[typeIndex + 3];

                int cardsStartIndex = handLine.LastIndexOf('>', handLine.Length - 9) + 1;
                //Flop
                if (streetChar == 'F')
                {
                    boardCards.Add(new Card(handLine[cardsStartIndex + 1], handLine[cardsStartIndex]));
                    boardCards.Add(new Card(handLine[cardsStartIndex + 4], handLine[cardsStartIndex + 3]));
                    boardCards.Add(new Card(handLine[cardsStartIndex + 7], handLine[cardsStartIndex + 6]));
                }
                //Turn
                if (streetChar == 'T')
                {
                    boardCards.Add(new Card(handLine[cardsStartIndex + 1], handLine[cardsStartIndex]));
                }
                //River
                if (streetChar == 'R')
                {
                    boardCards.Add(new Card(handLine[cardsStartIndex + 1], handLine[cardsStartIndex]));
                    break;
                }
            }

            return BoardCards.FromCards(boardCards.ToArray());
        }

        protected override string ParseHeroName(string[] handlines)
        {
            const string tag = "<nickname>";
            for (int i = 0; i < handlines.Length; i++)
            {
                if (handlines[i][1] == 'n' && handlines[i].StartsWith(tag))
                {
                    string line = handlines[i];
                    int startindex = tag.Length;
                    int endindex = line.IndexOf('<', startindex);
                    return line.Substring(startindex, endindex - startindex);
                }
            }
            return null;
        }

        protected override void FinalizeHandHistory(HandHistory Hand)
        {
            if (Hand.GameDescription.Limit == null)
            {
                var SB = Hand.HandActions.FirstOrDefault(p => p.HandActionType == HandActionType.SMALL_BLIND);
                var BB = Hand.HandActions.FirstOrDefault(p => p.HandActionType == HandActionType.BIG_BLIND);
                var Ante = Hand.HandActions.FirstOrDefault(p => p.HandActionType == HandActionType.ANTE);
                bool haveAnte = Ante != null;

                Limit limit;
                if (SB == null && BB == null)
                {
                    limit = Limit.FromSmallBlindBigBlind(Ante.Absolute, Ante.Absolute, Currency.CHIPS);
                }
                else if (SB == null)
                {
                    limit = Limit.FromSmallBlindBigBlind(BB.Absolute, BB.Absolute, Currency.CHIPS, haveAnte, (haveAnte ? Ante.Absolute : 0));
                }
                else
                {
                    limit = Limit.FromSmallBlindBigBlind(SB.Absolute, BB.Absolute, Currency.CHIPS, haveAnte, (haveAnte ? Ante.Absolute : 0));
                }

                Hand.GameDescription.Limit = limit;
            }
        }
    }
}
