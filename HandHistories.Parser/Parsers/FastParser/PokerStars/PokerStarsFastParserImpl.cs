﻿using System;
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
using HandHistories.Parser.Utils.FastParsing;

namespace HandHistories.Parser.Parsers.FastParser.PokerStars
{
    public class PokerStarsFastParserImpl : HandHistoryParserFastImpl
    {
        const int gameIDStartIndex = 17;

        public override bool RequresAdjustedRaiseSizes
        {
            get { return true; }
        }

        private SiteName _siteName;

        public override SiteName SiteName
        {
            get { return _siteName; }
        }

        // So the same parser can be used for It and Fr variations
        public PokerStarsFastParserImpl(SiteName siteName = SiteName.PokerStars)
        {
            _siteName = siteName;
        }

        private static readonly Regex HandSplitRegex = new Regex("(PokerStars Game #)|(PokerStars Hand #)|(PokerStars Zoom Hand #)", RegexOptions.Compiled);

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            return HandSplitRegex.Split(rawHandHistories)
                            .Where(s => string.IsNullOrWhiteSpace(s) == false && s.Length > 30)
                            .Select(s => "PokerStars Game #" + s.Trim('\r', 'n'));
        }

        protected override int ParseDealerPosition(string[] handLines)
        {
            // Expect the 2nd line to look like this:
            // Table 'Alemannia IV' 6-max Seat #2 is the button

            int startIndex = handLines[1].LastIndexOf('#') + 1;

            return FastInt.Parse(handLines[1], startIndex);
        }

        protected override DateTime ParseDateUtc(string[] handLines)
        {
            // Expect the first line to look like this: 
            // PokerStars Hand #78453197174:  Hold'em No Limit ($0.08/$0.16 USD) - 2012/04/06 20:56:40 ET

            // or

            // PokerStars Game #61777648755:  Hold'em No Limit ($0.50/$1.00 USD) - 2011/05/06 20:51:38 PT [2011/05/06 23:51:38 ET]

            handLines[0] = handLines[0].TrimEnd(']');

            int startIndex = handLines[0].Length - 22;
            string dateString = handLines[0].Substring(startIndex, 20);

            dateString = dateString.TrimStart('[');

            dateString = dateString.Trim(' ');

            CultureInfo provider = CultureInfo.InvariantCulture;
            // DateString is one of:
            // 2012/04/07 2:58:27
            // 2012/04/07 18:58:27

            DateTime dateTime = DateTime.ParseExact(dateString, "yyyy/MM/dd H:mm:ss", provider);//new DateTime(year, month, day, hour, minute, second);

            DateTime converted = TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            return DateTime.SpecifyKind(converted, DateTimeKind.Utc);
        }

        protected override void ParseExtraHandInformation(string[] handLines, Objects.Hand.HandHistorySummary handHistorySummary)
        {
            if (handHistorySummary.Cancelled)
            {
                return;                
            }

            for (int i = handLines.Length - 1; i >= 0; i--)
            {
                string handLine = handLines[i];

                // Check for summary line:
                //  *** SUMMARY ***
                if (handLine[0] == '*' && handLine[4] == 'S')
                {
                    // Line after summary line is:
                    //  Total pot $13.12 | Rake $0.59 
                    // or
                    //  Total pot $62.63 Main pot $54.75. Side pot $5.38. | Rake $2.50 
                    string totalLine = handLines[i + 1];

                    int lastSpaceIndex = totalLine.LastIndexOf(" ");
                    int spaceAfterFirstNumber = totalLine.IndexOf(" ", 11);

                    handHistorySummary.Rake =
                        decimal.Parse(totalLine.Substring(lastSpaceIndex + 2, totalLine.Length - lastSpaceIndex - 2), System.Globalization.CultureInfo.InvariantCulture);

                    handHistorySummary.TotalPot =
                        decimal.Parse(totalLine.Substring(11, spaceAfterFirstNumber - 11), System.Globalization.CultureInfo.InvariantCulture);

                    break;
                }
            }
        }

        protected override long ParseHandId(string[] handLines)
        {
            // Expect the first line to look like this: 
            //   PokerStars Hand #78453197174:  Hold'em No Limit ($0.08/$0.16 USD) - 2012/04/06 20:56:40 ET
            // or
            //   PokerStars Game #PokerStars Zoom Hand #84414134468:  Omaha Pot Limit ($0.05/$0.10 USD) - 2012/08/07 14:40:01 ET            

            // Zoom format           
            int firstDigitIndex = handLines[0][38] == '#' ? 39 : 17;
            int lastDigitIndex = handLines[0].IndexOf(':') - 1;

            string handId = handLines[0].Substring(firstDigitIndex, lastDigitIndex - firstDigitIndex + 1);
            return long.Parse(handId);
        }

        protected override string ParseTableName(string[] handLines)
        {
            // Line two is in form:
            // Table 'Centaurus VIII' 6-max Seat #2 is the button
            const int firstDashIndex = 7;
            int secondDash = handLines[1].LastIndexOf('\'');

            return handLines[1].Substring(firstDashIndex, secondDash - firstDashIndex);
        }

        protected override SeatType ParseSeatType(string[] handLines)
        {
            // line two looks like :
            // Table 'Alcor V' 6-max Seat #4 is the button
            int secondDash = handLines[1].LastIndexOf('\'');

            // 2-max, 6-max or 9-max
            int maxPlayers = FastInt.Parse(handLines[1][secondDash + 2]);

            // can't have 1max so must be 10max
            if (maxPlayers == 1)
            {
                maxPlayers = 10;
            }

            return SeatType.FromMaxPlayers(maxPlayers);
        }

        protected override GameType ParseGameType(string[] handLines)
        {
            // Expect the first line to look like this: 
            // PokerStars Hand #78453197174:  GAME-TYPE-INFO ($0.08/$0.16 USD) - 2012/04/06 20:56:40 ET   
            int colonIndex = handLines[0].IndexOf(':', gameIDStartIndex);

            // can be either 1 or 2 spaces after the colon
            int startIndex = (handLines[0][colonIndex + 2] == ' ') ? colonIndex + 3 : colonIndex + 2;
            int endIndex = handLines[0].IndexOf('(') - 2;

            int gameTypeLength = endIndex - startIndex + 1;
            

            // Faster to check the length vs doing an equality comparison
            switch (gameTypeLength)
            {
                case 16:
                    return GameType.NoLimitHoldem;
                case 15:
                    return GameType.PotLimitOmaha;
                case 13:
                    return GameType.FixedLimitHoldem;
                case 17:
                    string gameTypeString = handLines[0].Substring(startIndex, gameTypeLength);
                    if (gameTypeString[0] == 'O')
                    {
                        return GameType.FixedLimitOmahaHiLo;
                    }
                    return GameType.PotLimitHoldem;
                case 21:
                    return GameType.PotLimitOmahaHiLo;
                case 20:
                    return GameType.NoLimitOmahaHiLo;
                case 11:
                    return GameType.FixedLimitOmaha;
                default:
                    string errorGameTypeString = handLines[0].Substring(startIndex, gameTypeLength);
                    throw new UnrecognizedGameTypeException(handLines[0], "Unrecognized game-type: " + errorGameTypeString);
            }
        }

        protected override TableType ParseTableType(string[] handLines)
        {
            // Stars does not right out things such as speed/shallow/fast to hands right now.

            if (handLines[1].Contains(" Zoom "))
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Zoom);
            }

            // older hand history files have the cap mark in the first line
            if (handLines[1].LastIndexOf(" CAP", StringComparison.Ordinal) != -1 ||
               handLines[0].LastIndexOf(" Cap ", StringComparison.Ordinal) != -1)
            {
                return TableType.FromTableTypeDescriptions(TableTypeDescription.Cap);
            }

            return TableType.FromTableTypeDescriptions(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(string[] handLines)
        {
            // Expect the first line to look like:
            // PokerStars Hand #78441538809:  Hold'em Limit ($30/$60 USD) - 2012/04/06 16:45:19 ET

            int startIndex = handLines[0].IndexOf('(', gameIDStartIndex) + 1;
            int lastIndex = handLines[0].IndexOf(')', startIndex) - 1;

            string limitSubstring = handLines[0].Substring(startIndex, lastIndex - startIndex + 1);

            char currencySymbol = limitSubstring[0];
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
                    throw new LimitException(handLines[0], "Unrecognized currency symbol " + currencySymbol);
            }

            int slashIndex = limitSubstring.IndexOf('/');
            int firstSpace = limitSubstring.IndexOf(' ');

            decimal small = decimal.Parse(limitSubstring.Substring(1, slashIndex - 1), System.Globalization.CultureInfo.InvariantCulture);
            decimal big = decimal.Parse(limitSubstring.Substring(slashIndex + 2, firstSpace - (slashIndex + 2) + 1), System.Globalization.CultureInfo.InvariantCulture);


            // If it is an ante table we expect to see an ante line after the big blind
            decimal ante = 0;
            bool isAnte = false;

            return Limit.FromSmallBlindBigBlind(small, big, currency, isAnte, ante);
        }

        public override bool IsValidHand(string[] handLines)
        {
            bool isCancelled; // in this case eat it
            return IsValidOrCancelledHand(handLines, out isCancelled);
        }

        public override bool IsValidOrCancelledHand(string[] handLines, out bool isCancelled)
        {
            for (int i = handLines.Length - 1; i >= 0; i--)
            {
                string handLine = handLines[i];

                if (handLine.StartsWith("*** SU")) // if its the summary line
                {
                    // check if the previous line is 
                    // Hand cancelled
                    string previousLine = handLines[i - 1];

                    // lines before summary are collection lines so shouldn't be able to start w/ a H and end w/ a d
                    bool cancelled = (previousLine[0] == 'H' && previousLine[previousLine.Length - 1] == 'd');
                    //bool completeHand = previousLine.EndsWith("doesn't show hand");

                    isCancelled = cancelled;

                    return true;// || completeHand;
                }
            }

            // doesn't contain a summary line
            isCancelled = false;
            return false;
        }

        protected override List<HandAction> ParseHandActions(string[] handLines, GameType gameType = GameType.Unknown)
        {
            // actions take place from the last seat info until the *** SUMMARY *** line            

            int actionIndex = GetFirstActionIndex(handLines);

            List<HandAction> handActions = new List<HandAction>(handLines.Length - actionIndex);

            actionIndex = ParseBlindActions(handLines, actionIndex, handActions);

            Street currentStreet = Street.Preflop;

            for (int lineNumber = actionIndex; lineNumber < handLines.Length; lineNumber++)
            {
                string handLine = handLines[lineNumber];

                try
                {
                    bool isFinished = ParseLine(handLine, gameType, ref currentStreet, ref handActions);

                    if (isFinished)
                    {
                        actionIndex = lineNumber + 1;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    throw new HandActionException(handLine, "Couldn't parse line '" + handLine + " with ex: " + ex.Message);
                }
            }

            if (currentStreet == Street.Showdown)
            {
                ParseShowDown(handLines, actionIndex, ref handActions, gameType);
            }
            
            return handActions;
        }

        private int ParseBlindActions(string[] handLines, int firstActionIndex, List<HandAction> handActions)
        {
            for (int i = firstActionIndex; i < handLines.Length; i++)
            {
                var line = handLines[i];

                var lastChar = line[line.Length - 1];

                switch (lastChar)
                {
                    //*** FLOP *** [6d 7c 6h]
                    //*** TURN *** [6d 7c 6h] [2s]
                    //*** RIVER *** [6d 7c 6h 2s] [Qc]
                    case ']':
                        throw new HandActionException(string.Join(Environment.NewLine, handLines), "Unexpected Line: " + line);
                    //*** HOLE CARDS ***
                    case'*':
                        return i + 1;
                    //JLlama: sits out
                    //Craftyspirit: is sitting out
                    case 't':
                    //2As88 will be allowed to play after the button
                    case 'n':
                        continue;
                }

                int colonIndex = line.LastIndexOf(':');

                var action = ParsePostingActionLine(line, colonIndex);
                handActions.Add(action);
            }
            throw new HandActionException(string.Join(Environment.NewLine, handLines), "No end of posting actions");
        }

        private void ParseShowDown(string[] handLines, int actionIndex, ref List<HandAction> handActions, GameType gameType)
        {
            for (int i = actionIndex; i < handLines.Length; i++)
            {
                var line = handLines[i];

                var lastChar = line[line.Length - 1];

                switch (lastChar)
                {
                    // woezelenpip collected $7.50 from pot
                    case 't':
                    // templargio collected €6.08 from side pot-2
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
                        handActions.Add(ParseCollectedLine(line, Street.Showdown));
                        continue;

                    //*** SUMMARY ***
                    case '*':
                        return;

                    //No low hand qualified
                    //EASSA: mucks hand
                    case 'd':
                        if (line.EndsWith("hand"))
                        {
                            break;
                        }
                        continue;
                }

                int colonIndex = line.LastIndexOf(':'); // do backwards as players can have : in their name

                var action = ParseMiscShowdownLine(line, colonIndex, gameType);
                handActions.Add(action);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="line"></param>
        /// <param name="currentStreet"></param>
        /// <param name="handActions"></param>
        /// <returns>True if we have reached the end of the action block.</returns>
        private bool ParseLine(string line, GameType gameType, ref Street currentStreet, ref List<HandAction> handActions)
        {
            //Chat lines start with the PlayerName and
            //PlayerNames can contain characters that disturb parsing
            //So we must check for chatlines first
            char lastChar = line[line.Length - 1];
            if (lastChar == '\"')
            {
#if DEBUG
                if (!line.Contains(" said, "))
                {
                    throw new ArgumentException("Did not find \" said, \"");
                }
#endif
                return false;
            }

            char firstChar = line[0];

            if (firstChar == '*') // lines with a * are either board cards, hole cards or summary info
            {
                char typeOfEventChar = line[7];

                switch (typeOfEventChar)
                {
                    case 'P':
                        currentStreet = Street.Flop;
                        return false;
                    case 'N':
                        currentStreet = Street.Turn;
                        return false;
                    case 'E':
                        currentStreet = Street.River;
                        return false;
                    case 'W':
                        currentStreet = Street.Showdown;
                        return true;
                    case 'M':
                        return true;
                    default:
                        throw new HandActionException(line, "Unrecognized line w/ a *:" + line);
                }
            }

            // Uncalled bet lines look like:
            // Uncalled bet ($6) returned to woezelenpip
            if (line.Length > 29 && line[13] == '(')
            {
                handActions.Add(ParseUncalledBetLine(line, currentStreet));
                currentStreet = Street.Showdown;
                return true;
            }

            switch (lastChar)
            {
                //golfiarzp has timed out
                case 't':
                //MS13ZEN leaves the table
                case 'e':
                //Mr Sturmer is disconnected
                case 'd':
                    return false;

                //2As88 will be allowed to play after the button
                //matze1987: raises $8.94 to $10.94 and is all-in
                case 'n':
                    if (line.EndsWith("on"))
                    {
                        return false;
                    }
                    break;

                //Dealt to PS_Hero [4s 7h]
                case ']':
                    return false;
            }

            //zeranex88 joins the table at seat #5 
            if (lastChar == '#' || line[line.Length - 2] == '#')
            {
                // joins action
                // don't bother parsing it or adding it
                return false;
            }

            int colonIndex = line.LastIndexOf(':'); // do backwards as players can have : in their name

            HandAction handAction;
            switch (currentStreet)
            {
                case Street.Showdown:
                case Street.Null:
                    throw new HandActionException("", "Invalid State: Street");

                default:
                    handAction = ParseRegularActionLine(line, colonIndex, currentStreet);
                    break;
            }

            if (handAction != null)
            {
                handActions.Add(handAction);
            }

            return false;
        }

        public HandAction ParseMiscShowdownLine(string actionLine, int colonIndex, GameType gameType = GameType.Unknown)
        {
            // if the game type is Omaha HiLo can get colons like this after the Hi
            //  DOT19: shows [As 8h Ac Kd] (HI: two pair, Aces and Sixes)            
            while ((gameType == GameType.PotLimitOmahaHiLo || gameType == GameType.FixedLimitOmahaHiLo || gameType == GameType.NoLimitOmahaHiLo) &&
                actionLine.Count(c => c == ':') > 1 &&
                actionLine.Contains("(HI:") || actionLine.Contains("; LO:"))
            {
                int lastColon = actionLine.LastIndexOf(':');
                actionLine = actionLine.Remove(lastColon - 1);
                colonIndex = actionLine.LastIndexOf(':');
            }

            string playerName = actionLine.Substring(0, colonIndex);

            char actionIdentifier = actionLine[colonIndex + 2];

            switch (actionIdentifier)
            {
                case 's': // RECHUK: shows [Ac Qh] (a full house, Aces full of Queens)
                    return new HandAction(playerName, HandActionType.SHOW, 0, Street.Showdown);
                case 'd': // woezelenpip: doesn't show hand 
                case 'm': // Fjell_konge: mucks hand
                    return new HandAction(playerName, HandActionType.MUCKS, 0, Street.Showdown);
                default:
                    throw new HandActionException(actionLine, "ParseMiscShowdownLine: Unrecognized line '" + actionLine + "'");
            }
        }

        public HandAction ParsePostingActionLine(string actionLine, int colonIndex)
        {
            string playerName = actionLine.Substring(0, colonIndex);

            // Expect lines to look like one of these:

            // bingo185: posts small blind $0.50
            // bingo185: posts big blind $0.50
            // bingo185: posts the ante $0.05
            // bingo185: posts small & big blinds $0.75

            // the column w/ the & is a unique identifier
            char identifierChar = actionLine[colonIndex + 14];

            int firstDigitIndex;
            HandActionType handActionType;

            switch (identifierChar)
            {
                case 'b':
                    firstDigitIndex = colonIndex + 21;
                    handActionType = HandActionType.SMALL_BLIND;
                    break;
                case 'i':
                    firstDigitIndex = colonIndex + 19;
                    handActionType = HandActionType.BIG_BLIND;
                    break;
                case 't':
                    firstDigitIndex = colonIndex + 18;
                    handActionType = HandActionType.ANTE;
                    break;
                case '&':
                    firstDigitIndex = colonIndex + 28;
                    handActionType = HandActionType.POSTS;
                    break;
                default:
                    throw new HandActionException(actionLine, "ParsePostingActionLine: Unregonized lined " + actionLine);
            }

            decimal amount = decimal.Parse(actionLine.Substring(firstDigitIndex, actionLine.Length - firstDigitIndex), System.Globalization.CultureInfo.InvariantCulture);
            return new HandAction(playerName, handActionType, amount, Street.Preflop);
        }

        public HandAction ParseRegularActionLine(string actionLine, int colonIndex, Street currentStreet)
        {
            string playerName = actionLine.Substring(0, colonIndex);

            // all-in likes look like: 'Piotr280688: raises $8.32 to $12.88 and is all-in'
            bool isAllIn = actionLine[actionLine.Length - 1] == 'n';
            if (isAllIn)// Remove the  ' and is all in' and just proceed like normal
            {
                actionLine = actionLine.Remove(actionLine.Length - 14);
            }

            // lines that reach the cap look like tzuiop23: calls $62 and has reached the $80 cap
            bool hasReachedCap = actionLine[actionLine.Length - 1] == 'p';
            if (hasReachedCap)// Remove the  ' and has reached the $80 cap' and just proceed like normal
            {
                int lastNonCapCharacter = actionLine.LastIndexOf('n') - 2;  // find the n in the and
                actionLine = actionLine.Remove(lastNonCapCharacter);
            }

            char actionIdentifier = actionLine[colonIndex + 2];

            HandActionType actionType;
            decimal amount;
            int firstDigitIndex;

            switch (actionIdentifier)
            {
                //gaydaddy: folds
                case 'f':
                    return new HandAction(playerName, HandActionType.FOLD, 0, currentStreet);

                case 'c':
                    //Piotr280688: checks
                    if (actionLine[colonIndex + 3] == 'h')
                    {
                        return new HandAction(playerName, HandActionType.CHECK, 0, currentStreet);
                    }
                    //MECO-LEO: calls $1.23
                    firstDigitIndex = actionLine.LastIndexOf(' ') + 2;
                    amount = decimal.Parse(actionLine.Substring(firstDigitIndex, actionLine.Length - firstDigitIndex), System.Globalization.CultureInfo.InvariantCulture);
                    actionType = HandActionType.CALL;
                    break;

                //MS13ZEN: bets $1.76
                case 'b':
                    firstDigitIndex = actionLine.LastIndexOf(' ') + 2;
                    amount = decimal.Parse(actionLine.Substring(firstDigitIndex, actionLine.Length - firstDigitIndex), System.Globalization.CultureInfo.InvariantCulture);
                    actionType = HandActionType.BET;
                    break;

                //Zypherin: raises $6400 to $8300              
                case 'r':
                    firstDigitIndex = actionLine.LastIndexOf(' ') + 2;
                    amount = decimal.Parse(actionLine.Substring(firstDigitIndex, actionLine.Length - firstDigitIndex), System.Globalization.CultureInfo.InvariantCulture);
                    actionType = HandActionType.RAISE;
                    break;
                default:
                    throw new HandActionException(actionLine, "ParseRegularActionLine: Unrecognized line:" + actionLine);
            }

            return new HandAction(playerName, actionType, amount, currentStreet, isAllIn);
        }

        public HandAction ParseCollectedLine(string actionLine, Street currentStreet)
        {
            // 0 = main pot
            int potNumber = 0;
            HandActionType handActionType = HandActionType.WINS;

            // check for side pot lines like
            //  CinderellaBD collected $7 from side pot-2
            if (actionLine[actionLine.Length - 2] == '-')
            {
                handActionType = HandActionType.WINS_SIDE_POT;
                potNumber = Int32.Parse(actionLine[actionLine.Length - 1].ToString());
                // This removes the ' from side pot-2' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 16);
            }
            // check for a side pot line like
            // bozzoTHEclow collected $136.80 from side pot
            else if (actionLine[actionLine.Length - 8] == 's')
            {
                potNumber = 1;
                handActionType = HandActionType.WINS_SIDE_POT;
                // This removes the ' from side pot' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 14);
            }
            // check for main pot line like 
            //bozzoTHEclow collected $245.20 from main pot
            else if (actionLine[actionLine.Length - 8] == 'm')
            {
                // This removes the ' from main pot' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 14);
            }
            // otherwise is basic line like
            // alecc frost collected $1.25 from pot
            else
            {
                // This removes the ' from pot' from the line
                actionLine = actionLine.Substring(0, actionLine.Length - 9);
            }

            // Collected bet lines look like:
            // alecc frost collected $1.25 from pot

            int firstAmountDigit = actionLine.LastIndexOf(' ') + 2;
            decimal amount = decimal.Parse(actionLine.Substring(firstAmountDigit, actionLine.Length - firstAmountDigit), System.Globalization.CultureInfo.InvariantCulture);

            // 12 characters from first digit to the space infront of collected
            string playerName = actionLine.Substring(0, firstAmountDigit - 12);


            return new WinningsAction(playerName, handActionType, amount, potNumber);
        }

        public HandAction ParseUncalledBetLine(string actionLine, Street currentStreet)
        {
            // Uncalled bet lines look like:
            // Uncalled bet ($6) returned to woezelenpip

            // position 15 is after the currency symbol
            int closeParenIndex = actionLine.IndexOf(')', 16);
            decimal amount = decimal.Parse(actionLine.Substring(15, closeParenIndex - 15), System.Globalization.CultureInfo.InvariantCulture);

            int firstLetterOfName = closeParenIndex + 14; // ' returned to ' is length 14

            string playerName = actionLine.Substring(firstLetterOfName, actionLine.Length - firstLetterOfName);

            return new HandAction(playerName, HandActionType.UNCALLED_BET, amount, currentStreet);
        }

        private int GetFirstActionIndex(string[] handLines)
        {
            for (int lineNumber = 2; lineNumber < handLines.Length; lineNumber++)
            {
                //Seat 8: Zockermicha ($1613.51 in chips) 
                //BoomDoon: posts small blind $5
                string line = handLines[lineNumber];
                if (line[0] != 'S' || line[line.Length - 1] != ')')
                {
                    return lineNumber;
                }
            }

            throw new HandActionException(string.Empty, "GetFirstActionIndex: Couldn't find it.");
        }

        protected override PlayerList ParsePlayers(string[] handLines)
        {
            PlayerList playerList = new PlayerList();

            int lastLineRead = -1;

            // We start on line index 2 as first 2 lines are table and limit info.
            for (int lineNumber = 2; lineNumber < handLines.Length - 1; lineNumber++)
            {
                string line = handLines[lineNumber];

                char startChar = line[0];
                char endChar = line[line.Length - 1];

                //Seat 1: thaiJhonny ($16.08 in chips)
                if (endChar != ')')
                {
                    lastLineRead = lineNumber;
                    break;
                }

                // seat info expected in format: 
                //Seat 1: thaiJhonny ($16.08 in chips)
                const int seatNumberStartIndex = 4;
                int spaceIndex = line.IndexOf(' ', seatNumberStartIndex);
                int colonIndex = line.IndexOf(':', spaceIndex + 1);
                int seatNumber = FastInt.Parse(line, spaceIndex + 1);

                // we need to find the ( before the number. players can have ( in their name so we need to go backwards and skip the last one
                int openParenIndex = line.LastIndexOf('(');
                int spaceAfterOpenParen = line.IndexOf(' ', openParenIndex); // add 2 so we skip the $ and first # always                

                string playerName = line.Substring(colonIndex + 2, (openParenIndex - 1) - (colonIndex + 2));

                string stackString = line.Substring(openParenIndex + 2, spaceAfterOpenParen - (openParenIndex + 2));
                decimal stack = decimal.Parse(stackString, System.Globalization.CultureInfo.InvariantCulture);

                playerList.Add(new Player(playerName, stack, seatNumber));
            }

            if (lastLineRead == -1)
            {
                throw new PlayersException(string.Empty, "Didn't break out of the seat reading block.");
            }

            // Looking for the showdown info which looks like this
            //*** SHOW DOWN ***
            //JokerTKD: shows [2s 3s Ah Kh] (HI: a pair of Sixes)
            //DOT19: shows [As 8h Ac Kd] (HI: two pair, Aces and Sixes)
            //DOT19 collected $24.45 from pot
            //No low hand qualified
            //*** SUMMARY ***

            int summaryIndex = GetSummaryStartIndex(handLines, lastLineRead);
            int showDownIndex = GetShowDownStartIndex(handLines, lastLineRead, summaryIndex);
            //Starting from the bottom to parse faster
            if (showDownIndex != -1)
            {
                for (int lineNumber = showDownIndex + 1; lineNumber < summaryIndex; lineNumber++)
                {
                    //jimmyhoo: shows [7h 6h] (a full house, Sevens full of Jacks)
                    //EASSA: mucks hand 
                    //jimmyhoo collected $562 from pot
                    string line = handLines[lineNumber];
                    //Skip when player mucks and collects
                    //EASSA: mucks hand 
                    char lastChar = line[line.Length - 1];
                    if (lastChar == 'd' || lastChar == 't')
                    {
                        continue;
                    }

                    int lastSquareBracket = line.LastIndexLoopsBackward(']', line.Length - 1);

                    if (lastSquareBracket == -1)
                    {
                        continue;
                    }

                    int firstSquareBracket = line.LastIndexOf('[', lastSquareBracket);

                    // can show single cards:
                    // Zaza5573: shows [Qc]
                    if (line[firstSquareBracket + 3] == ']')
                    {
                        continue;
                    }

                    int colonIndex = line.LastIndexOf(':', firstSquareBracket);

                    if (colonIndex == -1)
                    {
                        // players with [ in their name
                        // [PS_UA]Tarik collected $18.57 from pot
                        continue;
                    }

                    string playerName = line.Substring(0, colonIndex);

                    string cards = line.Substring(firstSquareBracket + 1, lastSquareBracket - (firstSquareBracket + 1));

                    playerList[playerName].HoleCards = HoleCards.FromCards(cards);
                }
            }

            return playerList;
        }

        private int GetShowDownStartIndex(string[] handLines, int lastLineRead, int summaryIndex)
        {
            for (int i = lastLineRead; i < summaryIndex; i++)
            {
                if (handLines[i][0] == '*' && handLines[i][4] == 'S' && handLines[i][5] == 'H')//handLines[i].StartsWith("*** SH"))
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetSummaryStartIndex(string[] handLines, int lastLineRead)
        {
            for (int lineNumber = handLines.Length - 3; lineNumber > lastLineRead; lineNumber--)
            {
                if (handLines[lineNumber][0] != 'S' && 
                    handLines[lineNumber][0] != 'T' &&
                    handLines[lineNumber][0] != 'B')
                {
                    return lineNumber;
                }
            }
            //Summary must exist or it is not a valid Pokerstars Hand
            throw new IndexOutOfRangeException("Could not find *** Summary ***");
        }

        protected override BoardCards ParseCommunityCards(string[] handLines)
        {
            // Expect the end of the hand to have something like this:
            //*** SUMMARY ***
            //Total pot $90 | Rake $2.80 
            //Board [4s 7d Ad]
            //Seat 4: TopKat5757 (small blind) folded before Flop
            //Seat 5: Masic.Almir (big blind) folded before Flop

            BoardCards boardCards = BoardCards.ForPreflop();
            for (int lineNumber = handLines.Length - 2; lineNumber >= 0; lineNumber--)
            {
                string line = handLines[lineNumber];
                if (line[0] == '*')
                {
                    return boardCards;
                }

                if (line[0] != 'B')
                {
                    continue;
                }

                int firstSquareBracket = 6;
                int lastSquareBracket = line.Length - 1;

                return
                    BoardCards.FromCards(line.Substring(firstSquareBracket + 1, lastSquareBracket - (firstSquareBracket + 1)));
            }

            throw new CardException(string.Empty, "Read through hand backwards and didn't find a board or summary.");
        }

        protected override string ParseHeroName(string[] handlines)
        {
            for (int i = 0; i < handlines.Length; i++)
            {
                if (handlines[i][0] == 'D' && handlines[i].StartsWith("Dealt to "))
                {
                    string line = handlines[i];
                    int endIndex = line.LastIndexOf('[');
                    return line.Substring(9, endIndex - 9 - 1);
                }
            }
            return null;
        }
    }
}
