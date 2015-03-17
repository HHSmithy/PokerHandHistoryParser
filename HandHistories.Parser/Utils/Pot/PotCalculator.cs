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
        static bool EndsWithUncalledBet(List<HandAction> actions)
        {
            for (int i = actions.Count - 1; i > 0; i--)
            {
                var action = actions[i];

                switch (action.HandActionType)
                {
                    case HandActionType.CALL:
                    case HandActionType.CHECK:
                    case HandActionType.BIG_BLIND:
                        return false;
                    case HandActionType.RAISE:
                    case HandActionType.BET:
                        return true;

                    //This a cancelled hand
                    case HandActionType.SMALL_BLIND:
                        return false;
                }
            }
            throw new ArgumentException("Incomplete Hand: NO CALL/CHECK/BET/RAISE/BB Found");
        }

        public static decimal CalculateTotalPot(HandHistory hand)
        {
            var gameActions = hand.HandActions
                .Where(p => p.IsGameAction)
                .ToList();

            var playerWagers = hand.Players.ToDictionary(p => p.PlayerName, p => 0m);

            decimal amountToCall = 0;
            decimal lastBetAmount = 0;

            foreach (var action in hand.HandActions)
            {
                switch (action.HandActionType)
                {
                    case HandActionType.CALL:
                        amountToCall = 0;
                        playerWagers[action.PlayerName] += action.Amount;
                        break;
                    case HandActionType.RAISE:
                        playerWagers[action.PlayerName] += action.Amount;
                        lastBetAmount = playerWagers[action.PlayerName] - amountToCall;
                        amountToCall = playerWagers[action.PlayerName];
                        break;
                    case HandActionType.BET:
                        lastBetAmount = action.Amount;
                        playerWagers[action.PlayerName] += action.Amount;
                        amountToCall = playerWagers[action.PlayerName];
                        break;
                    case HandActionType.SMALL_BLIND:
                    case HandActionType.BIG_BLIND:
                    case HandActionType.ANTE:
                    case HandActionType.POSTS:
                        amountToCall = action.Amount;
                        playerWagers[action.PlayerName] += action.Amount;
                        break;
                }
            }

            decimal TotalPot = playerWagers
                .Sum(p => p.Value);

            if (EndsWithUncalledBet(hand.HandActions))
            {
                TotalPot -= lastBetAmount;
            }

            return Math.Abs(TotalPot);
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
