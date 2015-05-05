using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils.AllInAction
{
    public class AllInActionHelper
    {
        public static HandActionType GetAllInActionType(string playerName, decimal amount, Street street, List<HandAction> actions)
        {
            var streetActions = actions.Street(street);

            if (street != Street.Preflop && streetActions.FirstOrDefault(p => p.HandActionType == HandActionType.BET) == null)
            {
                return HandActionType.BET;
            }

            Dictionary<string, decimal> playerWagers = new Dictionary<string, decimal>();

            foreach (var action in streetActions)
            {
                if (!playerWagers.ContainsKey(action.PlayerName))
                {
                    playerWagers.Add(action.PlayerName, 0);   
                }
                playerWagers[action.PlayerName] += action.Amount;
            }

            decimal biggestWager = Math.Abs(playerWagers.Min(p => p.Value));

            decimal playerWager = amount;

            if (playerWagers.ContainsKey(playerName))
            {
                playerWager += Math.Abs(playerWagers[playerName]);
            }

            if (playerWager <= biggestWager)
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
