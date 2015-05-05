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
                .Where(p => p.IsGameAction || 
                    p.IsBlinds || 
                    p.HandActionType == HandActionType.ANTE || 
                    p.HandActionType == HandActionType.POSTS)
                .ToList();

            Dictionary<string, decimal> amounts = new Dictionary<string, decimal>();

            foreach (var action in gameActions)
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

            var Pot = amounts
                .Sum(p => p.Value);

            if (maxCount == 1)
            {
                var values = amounts
                    .Select(p => p.Value)
                    .OrderBy(p => p)
                    .ToList();

                var uncalledBet = values[0] - values[1];

                Pot -= uncalledBet;
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
