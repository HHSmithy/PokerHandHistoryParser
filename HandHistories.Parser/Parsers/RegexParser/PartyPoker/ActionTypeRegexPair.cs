using System;
using System.Text.RegularExpressions;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;

namespace HandHistories.Parser.Parsers.RegexParser.PartyPoker
{
    public class HandActionTypeRegexPair
    {
        public string ActionRegex { get; private set; }
        public HandActionType HandActionType { get; private set; }

        public HandActionTypeRegexPair(HandActionType handHandActionType, string actionRegex)
        {
            ActionRegex = actionRegex;
            HandActionType = handHandActionType;
        }

        public static HandAction ParseFromActionLine(SiteActionRegexesBase siteActionRegexes, Street street, string actionText, string playerName)
        {
            return GetAction(siteActionRegexes, street, actionText, playerName);
        }

        private static HandAction GetAction(SiteActionRegexesBase siteActions, Street street, string actionText, string playerName)
        {
            HandActionType ActionType = ParseActionType(siteActions, street, actionText);

            decimal amount;

            if (ActionType == HandActionType.CHAT)
            {
                amount = 0m;
            }
            else if (ActionType == HandActionType.POSTS)
            {
                amount = ParseAmountPosts(siteActions, actionText);                
            }
            else
            {
                amount = ParseAmount(siteActions, actionText);
            }

            if (ActionType == HandActionType.ALL_IN)
            {
                return new AllInAction(playerName, amount, street, true);
            }

            HandAction handAction = new HandAction(playerName, ActionType, amount, street);
            if (handAction.IsWinningsAction)
            {
                int potNumber;
                if (actionText.Contains(" main pot ")) potNumber = 0;
                else if (actionText.Contains(" side pot 1 ")) potNumber = 1;
                else if (actionText.Contains(" side pot ") == false) potNumber = 0;
                else throw new NotImplementedException("Can't do side pots for " + actionText);

                return new WinningsAction(playerName, ActionType, amount, potNumber);
            }
            return handAction;
        }

        private static decimal ParseAmountPosts(SiteActionRegexesBase siteActionRegexes, string actionText)
        {
            var amountMatch = Regex.Match(actionText, siteActionRegexes.PostAmountRegex);

            if (amountMatch.Success)
                return Decimal.Parse(amountMatch.Value);

            return 0.0M;
        }

        private static Decimal ParseAmount(SiteActionRegexesBase siteActionRegexes, string actionText)
        {
            //Mantis Bug 91 - Generalize parsing for multiple sites

            var amountMatch = Regex.Match(actionText, siteActionRegexes.AmountRegex);

            if (amountMatch.Success)
                return Decimal.Parse(amountMatch.Value);

            return 0.0M;
        }

        private static HandActionType ParseActionType(SiteActionRegexesBase siteActionRegexes, Street street, string actionText)
        {
            foreach (var actionRegex in siteActionRegexes.GetPossibleActions(street))
            {
                if (string.IsNullOrWhiteSpace(actionRegex.ActionRegex)) continue;

                var match = Regex.Match(actionText, actionRegex.ActionRegex);

                if (match.Success)
                {
                    return actionRegex.HandActionType;
                }
            }

            return HandActionType.UNKNOWN;
        }
    }
}