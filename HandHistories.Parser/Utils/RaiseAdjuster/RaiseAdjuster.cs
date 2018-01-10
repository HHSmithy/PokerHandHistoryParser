using HandHistories.Objects.Actions;
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
                        if (actions[j].PlayerName.Equals(currentAction.PlayerName))
                        {
                            // Ante's don't get counted in the raise action lines
                            if (actions[j].HandActionType == HandActionType.ANTE)
                            {
                                continue;
                            }

                            // a POSTS SB is always dead money
                            // a POSTS BB needs to be deducted completely
                            // a POSTS SB+BB only the BB needs to be deducted
                            if (actions[j].HandActionType == HandActionType.POSTS)
                            {
                                // we use <= due to the negative numbers
                                if (actions[j].Amount <= actions.First(a => a.HandActionType == HandActionType.BIG_BLIND).Amount)
                                {
                                    currentAction.DecreaseAmount(actions.First(a => a.HandActionType == HandActionType.BIG_BLIND).Amount);
                                }
                                continue;
                            }

                            // If the player previously called any future raise will be the entire amount
                            if (actions[j].HandActionType == HandActionType.CALL)
                            {
                                currentAction.DecreaseAmount(actions[j].Amount);
                                continue;
                            }


                            // Player who posted SB/BB/SB+BB can check on their first action
                            if (actions[j].HandActionType == HandActionType.CHECK)
                            {
                                continue;
                            }

                            currentAction.DecreaseAmount(actions[j].Amount);
                            break;
                        }
                    }
                }
            }

            return handActions;
        }
    }
}
