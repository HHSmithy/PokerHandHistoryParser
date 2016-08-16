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
        /// <summary>
        /// Total Pot = all actions - uncalled bets - winnings.
        /// Dead small blinds are included
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public static decimal CalculateTotalPot(HandHistory hand)
        {
            var sum = hand.HandActions.Where(p => !p.IsWinningsAction).Sum(a => a.Amount);

            return Math.Abs(sum);
        }

        public static decimal CalculateRake(HandHistory hand)
        {
            var totalCollected = hand.HandActions
                .Where(p => p.IsWinningsAction)
                .Sum(p => p.Amount);

            return hand.TotalPot.Value - totalCollected;
        }
    }
}
