using HandHistories.Objects.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.Actions
{
    public static class WinningsActionExtensionMethods
    {
        public static IEnumerable<WinningsAction> Player(this IEnumerable<WinningsAction> winners, string PlayerName)
        {
            return winners.Where(p => p.PlayerName == PlayerName);
        }

        public static IEnumerable<WinningsAction> Player(this IEnumerable<WinningsAction> winners, HandAction action)
        {
            return winners.Where(p => p.PlayerName == action.PlayerName);
        }

        public static IEnumerable<WinningsAction> Player(this IEnumerable<WinningsAction> winners, Player action)
        {
            return winners.Where(p => p.PlayerName == action.PlayerName);
        }
    }
}
