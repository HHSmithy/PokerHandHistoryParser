using System.Collections.Generic;
using System.Text.RegularExpressions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.RegexParser.PartyPoker;

namespace HandHistories.Parser.Parsers.Base
{
    public abstract class HandHistoryRegexActionParserImplBase : IHandHistoryActionParser
    {
        public abstract string StartOfActionsRegex { get; }

        public abstract string BoardRegexFlop { get; }
        public abstract string BoardRegexTurn { get; }
        public abstract string BoardRegexRiver { get; }

        public abstract SiteActionRegexesBase SiteActionRegexes { get; }

        public List<HandAction> ParseActions(string handText, PlayerList players, Street currentStreet)
        {
            // Mantis Bug 88 all these \r\n garbage
            handText = handText.Replace("\r\n", "\r");
            handText = handText.Replace("\n", "\r");

            Match match = Regex.Match(handText, StartOfActionsRegex, RegexOptions.Multiline);
            string actionTextBlock = (match.Success) ? match.Value : handText;

            var actions = new List<HandAction>();

            string[] actionLines = actionTextBlock.Split(new char[] { '\r', '\n' });
            foreach (var line in actionLines)
            {
                // Mantis Bug 91 generalize this line for other sites (make it abstract)
                if (line.Equals("** Dealing down cards **")) continue;

                if (currentStreet == Street.Preflop && Regex.Match(line, BoardRegexFlop).Success)
                {
                    currentStreet = Street.Flop;
                    continue;
                }
                else if (Regex.Match(line, BoardRegexTurn).Success)
                {
                    if (currentStreet == Street.Flop)
                    {
                        currentStreet = Street.Turn;
                        continue;
                    }
                    else
                    {
                        throw new HandActionException(handText, "ParseActions: Detected turn before flop.");
                    }
                }
                else if (Regex.Match(line, BoardRegexRiver).Success)
                {
                    if (currentStreet == Street.Turn)
                    {
                        currentStreet = Street.River;
                        continue;
                    }
                    else
                    {
                        throw new HandActionException(handText, "ParseActions: Detected river before turn.");
                    }
                }

                // Mantis Bug 92 - optimize this
                string playerName = null;
                foreach (var player in players)
                {
                    if (line.Contains(player.PlayerName))
                    {
                        playerName = player.PlayerName;
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(playerName) == false)
                {
                    HandAction action = HandActionTypeRegexPair.ParseFromActionLine(SiteActionRegexes, currentStreet, line, playerName);

                    if (action.HandActionType != HandActionType.UNKNOWN)
                    {
                        actions.Add(action);
                    }
                }

                //if (!success)
                //    throw new HandParseException("Failed to match action " + line + " with a player");
            }

            return actions;
        }

        public List<HandAction> ParseActions(string handText, PlayerList players)
        {
            return ParseActions(handText, players, Street.Preflop);
        }
    }
}
