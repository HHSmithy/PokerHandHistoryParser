using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils.AllInAction
{
    public static class AllInActionHelper
    {
        /// <summary>
        /// Gets the ActionType for an unadjusted action amount
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="amount"></param>
        /// <param name="street"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public static HandActionType GetAllInActionType(string playerName, decimal amount, Street street, List<HandAction> actions)
        {
            var streetActions = actions.Street(street);

            if (street != Street.Preflop && streetActions.FirstOrDefault(p => p.HandActionType == HandActionType.BET) == null)
            {
                return HandActionType.BET;
            }


            if (Math.Abs(amount) <= Math.Abs(actions.Min(p => p.Amount)))
            {
                return HandActionType.CALL;
            }
            else
            {
                return HandActionType.RAISE;
            }
        }

        /// <summary>
        /// Gets the adjusted amount for a Call-AllIn action
        /// </summary>
        /// <param name="amount">The Call Action AMount</param>
        /// <param name="playerActions">The calling players previous actions</param>
        /// <returns>the adjusted call size</returns>
        public static decimal GetAdjustedCallAllInAmount(decimal amount, IEnumerable<HandAction> playerActions)
        {
            if (playerActions.Count() == 0)
            {
                return amount;
            }

            return amount - Math.Abs(playerActions.Min(p => p.Amount));
        }
    }
}
