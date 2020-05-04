using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils.RaiseAdjuster
{
    public class RaiseAdjuster
    {
        /// <summary>
        /// Re-raise amounts are handled differently on different sites. Consider the
        /// situation where:
        ///          Player1 Bets $10
        ///          Player2 Raises to $30 total (call of $10, raise $20)
        ///          Player1 Raises to $100 total (call of $20, raise $70)
        /// 
        /// Party will display this as: Bet 10, Raise 30, Raise 70
        /// Stars will display this as: Bet 10, Raise to 30, Raise to 100. 
        /// 
        /// In the case for Stars we will need to deduct previous action amounts from the raise to figure out how
        /// much that raise actuall is i.e Player 1 only wagered $90 more.
        /// </summary>
        /// <param name="handActions"></param>
        public static List<HandAction> AdjustRaiseSizes(List<HandAction> handActions)
        {
            var actionsByStreets = handActions.GroupBy(h => h.Street);

            foreach (var actionsByStreet in actionsByStreets)
            {
                List<HandAction> actions = actionsByStreet.ToList();

                // loop backward through the actions and subtracting the action prior to each raise
                // from that raise amount
                for (int i = actions.Count - 1; i >= 0; i--)
                {
                    HandAction currentAction = actions[i];

                    if (currentAction.HandActionType != HandActionType.RAISE)
                    {
                        continue;
                    }

                    for (int j = i - 1; j >= 0; j--)
                    {
                        var action_j = actions[j];
                        if (action_j.PlayerName.Equals(currentAction.PlayerName))
                        {
                            // Ante's don't get counted in the raise action lines
                            // POSTS_DEAD don't get counted in the raise action lines
                            if (action_j.HandActionType == HandActionType.ANTE || action_j.HandActionType == HandActionType.POSTS_DEAD)
                            {
                                continue;
                            }

                            // a POSTS SB is always dead money
                            // a POSTS BB needs to be deducted completely
                            // a POSTS SB+BB only the BB needs to be deducted
                            if (action_j.HandActionType == HandActionType.POSTS)
                            {
                                // we use <= due to the negative numbers
                                if (action_j.Amount <= actions.First(a => a.HandActionType == HandActionType.BIG_BLIND).Amount)
                                {
                                    currentAction.DecreaseAmount(actions.First(a => a.HandActionType == HandActionType.BIG_BLIND).Amount);
                                }
                                continue;
                            }

                            // If the player previously called any future raise will be the entire amount
                            if (action_j.HandActionType == HandActionType.CALL)
                            {
                                currentAction.DecreaseAmount(action_j.Amount);
                                continue;
                            }


                            // Player who posted SB/BB/SB+BB can check on their first action
                            if (action_j.HandActionType == HandActionType.CHECK)
                            {
                                continue;
                            }

                            currentAction.DecreaseAmount(action_j.Amount);
                            break;
                        }
                    }
                }
            }

            return handActions;
        }

        /// <summary>
        /// This is for handhistory that only print the total put in on the current street
        /// Primarily used for IGT Parser
        /// 
        /// IGT require both adjustments for calls and raises
        /// </summary>
        /// <param name="handActions"></param>
        /// <returns></returns>
        public static List<HandAction> AdjustRaiseSizesAndCalls(List<HandAction> handActions)
        {
            Dictionary<string, decimal> putinpot = new Dictionary<string, decimal>(10);
            Street currentStreet = Street.Null;

            foreach (var action in handActions)
            {
                if (currentStreet != action.Street)
                {
                    putinpot.Clear();
                    currentStreet = action.Street;
                }

                if (!putinpot.ContainsKey(action.PlayerName))
                {
                    putinpot.Add(action.PlayerName, 0);
                }

                var playerPutInPot = putinpot[action.PlayerName];
                if (action.IsRaise || action.HandActionType == HandActionType.CALL)
                {
                    action.DecreaseAmount(playerPutInPot);
                }
                playerPutInPot += action.Absolute;

                putinpot[action.PlayerName] = playerPutInPot;
            }

            return handActions;
        }
    }
}
