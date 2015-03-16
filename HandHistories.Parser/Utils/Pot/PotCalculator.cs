using HandHistories.Objects.Actions;
using HandHistories.Objects.Hand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils.Pot
{
    public static class PotCalculator
    {
        public static decimal CalculateTotalPot(HandHistory hand)
        {
            var gameActions = hand.HandActions
                .Where(p => p.IsGameAction)
                .ToList();

            var lastAction = gameActions[gameActions.Count - 1];

            var playerPutInPotOnLastStreet = gameActions
                .Street(lastAction.Street)
                .Player(lastAction.PlayerName)
                .Sum(p => p.Amount);

            Dictionary<string, decimal> amounts = hand.Players
                .ToDictionary(p => p.PlayerName, p => 0m);

            decimal amountToCall = 0;

            foreach (var action in gameActions)
            {
                if (action.HandActionType == HandActionType.BIG_BLIND)
                {
                    amountToCall = action.Amount;
                }
                if (action.IsBlinds || 
                    action.HandActionType == HandActionType.POSTS)
                {
                    amounts[action.PlayerName] += action.Amount;
                }
                if (action.IsAggressiveAction)
                {
                    amountToCall += action.Amount;
                    amounts[action.PlayerName] = amountToCall;
                }
                else if (action.HandActionType == HandActionType.CALL)
                {
                    amounts[action.PlayerName] += action.Amount;
                }
            }

            var maxValue = amounts
                .Min(p => p.Value);

            var maxCount = amounts
                .Count(p => p.Value == maxValue);

            var Pot = amounts
                .Sum(p => p.Value);

            if (maxCount == 1)
            {
                var nextValue = amounts.OrderBy(p => p.Value)
                    .Skip(1)
                    .First()
                    .Value;

                Pot -= maxValue - nextValue;
            }

            return Math.Abs(Pot);
        }

        public static decimal CalculateRake(HandHistory hand)
        {
            var TotalCollected = hand.HandActions
                .Where(p => p.IsWinningsAction)
                .Sum(p => p.Amount);

            return hand.TotalPot.Value - TotalCollected;
        }
    }
}
