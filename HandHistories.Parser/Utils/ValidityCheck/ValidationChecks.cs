using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils
{
    [Flags]
    public enum ValidationChecks : int
    {
        TOTAL_POT = 1 << 0,
        ACTION_ORDER = 1 << 1,
        STREET_ORDER = 1 << 2,
        BLIND_ORDER = 1 << 3,
        /// <summary>
        /// Checks if all actions by a player is less then or equal the players stack
        /// </summary>
        ACTION_TOTAL_AMOUNTS = 1 << 4,
        /// <summary>
        /// Checks that no players who are sitout also has actions in the hand
        /// </summary>
        PLAYERLIST_SITOUT_WITH_ACTIONS = 1 << 5,
        /// <summary>
        /// Checks that players who are sitin also has actions in the hand
        /// </summary>
        PLAYERLIST_SITIN_WITHOUT_ACTIONS = 1 << 6,
        ALL = -1
    }
}
