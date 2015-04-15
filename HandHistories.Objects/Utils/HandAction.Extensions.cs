using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.Actions
{
    public static class HandActionExtension
    {
        public static IEnumerable<HandAction> Street(this IEnumerable<HandAction> actions, Street street)
        {
            return actions.Where(p => p.Street == street);
        }

        public static IEnumerable<HandAction> Street(this IEnumerable<HandAction> actions, HandAction action)
        {
            return actions.Where(p => p.Street == action.Street);
        }

        public static IEnumerable<HandAction> Player(this IEnumerable<HandAction> actions, string PlayerName)
        {
            return actions.Where(p => p.PlayerName == PlayerName);
        }

        public static IEnumerable<HandAction> Player(this IEnumerable<HandAction> actions, HandAction action)
        {
            return actions.Where(p => p.PlayerName == action.PlayerName);
        }

        public static IEnumerable<HandAction> Player(this IEnumerable<HandAction> actions, Player action)
        {
            return actions.Where(p => p.PlayerName == action.PlayerName);
        }
    }
}
