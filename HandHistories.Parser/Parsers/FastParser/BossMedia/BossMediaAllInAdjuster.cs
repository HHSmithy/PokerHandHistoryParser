using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Parsers.FastParser.BossMedia
{
    /// <summary>
    /// [VARNING] The bossmedia allin adjustments operates on unadjusted amounts
    /// 
    /// Bossmedia have a strange way of representing ALLINs: 
    /// the ALLIN action amount in the handtext = the stack in the beginning of the street
    /// For example:
    /// -Player1's stack is 100 before the hand starts
    /// -Player1 contributes 20 to the pot preflop
    /// -Player1 bets flop 40
    /// -Player1 ALLINs flop 80
    /// </summary>
#if DEBUG
    public class BossMediaAllInAdjuster
#else
    class BossMediaAllInAdjuster
#endif
    {
        /// <summary>
        /// Gets the adjusted amount for a AllIn action before raise adjustments
        /// </summary>
        /// <param name="amount">The Call Action AMount</param>
        /// <param name="playerActions">The calling players previous actions</param>
        /// <returns>the adjusted call size</returns>
        public static decimal GetAdjustedAllInAmount(string player, decimal amount, Street street, IEnumerable<HandAction> handActions)
        {
            var playerActions = handActions
                .Player(player)
                .Street(street);

            if (playerActions.Count() == 0)
            {
                return amount;
            }

            return amount - Math.Abs(playerActions.Min(p => p.Amount));
        }

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

            Dictionary<string, decimal> PutInPot = new Dictionary<string, decimal>();

            foreach (var action in streetActions)
            {
                if (!PutInPot.ContainsKey(action.PlayerName))
                {
                    PutInPot.Add(action.PlayerName, action.Amount);
                }
                else
                {
                    PutInPot[action.PlayerName] += action.Amount;
                }
            }

            var contributed = Math.Abs(amount);
            if (PutInPot.ContainsKey(playerName))
            {
                contributed += Math.Abs(PutInPot[playerName]);
            }
            if (contributed <= Math.Abs(PutInPot.Min(p => p.Value)))
            {
                return HandActionType.CALL;
            }
            else
            {
                return HandActionType.RAISE;
            }
        }
    }
}
