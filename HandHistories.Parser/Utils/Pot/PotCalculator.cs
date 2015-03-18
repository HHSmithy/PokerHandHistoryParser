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

            Dictionary<string, decimal> amounts = new Dictionary<string, decimal>();

            foreach (var action in gameActions
                .Street(lastAction.Street))
            {
                if (!amounts.ContainsKey(action.PlayerName))
                {
                    amounts.Add(action.PlayerName, action.Amount);
                }
                else
                {
                    amounts[action.PlayerName] += action.Amount;
                }
            }

            var maxValue = amounts
                .Min(p => p.Value);

            var maxCount = amounts
                .Count(p => p.Value == maxValue);

            var Pot = gameActions
                .Sum(p => p.Amount);

            if (maxCount == 1)
            {
                var LastBetPotContribution = amounts
                    .Where(p => p.Value != maxValue)
                    .Min(p => p.Value);

                Pot -= maxValue;
                Pot += LastBetPotContribution;
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
