using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils.Uncalled
{
    public static class UncalledBet
    {
        /// <summary>
        /// Adds a Uncalled bet action if there was a RAISE/BET/BB that was not called at all or only called partially
        /// </summary>
        /// <param name="handActions"></param>
        /// <returns></returns>
        public static List<HandAction> Fix(List<HandAction> handActions)
        {
            if (handActions.FirstOrDefault(p => p.HandActionType == HandActionType.UNCALLED_BET) != null)
            {
                return handActions;
            }

            var realActions = handActions.Where(a => a.IsGameAction && !a.IsWinningsAction && a.HandActionType != HandActionType.FOLD).ToList();

            var lastAction = realActions[realActions.Count - 1];

            //uncalled bets need to be inserted before the showdown actions
            int insertIndex = -1;
            for (int i = handActions.Count - 1; i >= 0; i--)
            {
                if (handActions[i].Street != Street.Showdown)
                {
                    insertIndex = i + 1;
                    break;
                }
            }

            switch (lastAction.HandActionType)
            {
                case HandActionType.RAISE:
                    handActions.Insert(insertIndex, GetUncalledRaise(realActions, lastAction));
                    break;

                // when the last action before summary is a bet, we need to return that bet to the according player
                case HandActionType.BET:
                    handActions.Insert(insertIndex, GetUncalledBet(lastAction));
                    break;

                // when the last action before the summary is the big blind, we need to return the difference between BB and SB
                case HandActionType.BIG_BLIND:
                    handActions.Insert(insertIndex, GetUncalledBigBlind(realActions, lastAction));
                    break;

                case HandActionType.CALL:
                    if (lastAction.IsAllIn)
                    {
                        handActions.Insert(insertIndex, GetPartiallyUncalledRaise(realActions));
                    }
                    break;
            }

            return handActions;
        }

        /// <summary>
        /// Adjust the wins for the uncalledBets
        /// </summary>
        /// <param name="handActions"></param>
        /// <returns></returns>
        public static List<HandAction> FixUncalledBetWins(List<HandAction> handActions)
        {
            var uncalled = handActions.FirstOrDefault(p => p.HandActionType == HandActionType.UNCALLED_BET);
            if (uncalled != null)
            {
                var winner = handActions.FirstOrDefault(p => p.IsWinningsAction && p.PlayerName == uncalled.PlayerName);

                if (winner != null)
                {
                    winner.DecreaseAmount(uncalled.Absolute);
                }
            }
            return handActions;
        }

        private static HandAction GetPartiallyUncalledRaise(List<HandAction> realActions)
        {
            var lastRaiser = realActions.Last(p => p.IsAggressiveAction || p.HandActionType == HandActionType.BIG_BLIND);

            var UncalledRaise = GetUncalledRaise(realActions, lastRaiser);

            return UncalledRaise;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realActions">a list of handactions only containing CALL/BET/RAISE</param>
        /// <param name="lastRaiser">the last realAction</param>
        /// <returns>an Uncalled Bet action</returns>
        private static HandAction GetUncalledRaise(List<HandAction> realActions, HandAction lastRaiser)
        {
            // amount to return is the amount the raise player invested - the 2nd largest amount invested by a different player
            var totalInvestedAmount = realActions.Where(a => a.PlayerName.Equals(lastRaiser.PlayerName)).Sum(a => a.Amount);

            // now we need to get the maximum amount invested by a different player involved in the hand
            var totalInvestedAmountOtherPlayer = realActions.Where(a => !a.PlayerName.Equals(lastRaiser.PlayerName)).GroupBy(a => a.PlayerName)
                                                            .Select(p => new
                                                            {
                                                                PlayerName = p.Key,
                                                                Invested = p.Sum(x => x.Amount)
                                                            })
                                                            .Min(x => x.Invested); // money invested is negative, so take the "max" negative value


            return new HandAction(lastRaiser.PlayerName, HandActionType.UNCALLED_BET, totalInvestedAmount - totalInvestedAmountOtherPlayer, lastRaiser.Street);
        }

        /// <summary>
        /// Creates a new handaction with the diffrence between BB and SB
        /// </summary>
        /// <param name="realActions">a list of handactions only containing CALL/BET/RAISE</param>
        /// <param name="lastAction">the last realAction</param>
        /// <returns>an Uncalled Bet action</returns>
        private static HandAction GetUncalledBigBlind(List<HandAction> realActions, HandAction lastAction)
        {
            // it can actually happen that there was no SB involved
            var sbAction = realActions.FirstOrDefault(a => a.HandActionType == HandActionType.SMALL_BLIND);
            var sbAmount = 0m;

            if (sbAction != null)
            {
                sbAmount = sbAction.Amount;
            }

            return new HandAction(lastAction.PlayerName, HandActionType.UNCALLED_BET, lastAction.Amount - sbAmount, lastAction.Street);
        }

        private static HandAction GetUncalledBet(HandAction lastAction)
        {
            return new HandAction(lastAction.PlayerName, HandActionType.UNCALLED_BET, lastAction.Amount, lastAction.Street);
        }
    }
}
