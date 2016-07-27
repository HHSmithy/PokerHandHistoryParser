using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.Actions
{
    partial class HandAction
    {
        #region Operators
        public static bool operator ==(HandAction a, HandAction b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }
            if (a.IsWinningsAction || b.IsWinningsAction)
            {
                return a.ToString() == b.ToString();
            }

            return a.HandActionType == b.HandActionType &&
                a.Amount == b.Amount &&
                a.IsAllIn == b.IsAllIn &&
                a.Street == b.Street &&
                a.PlayerName == b.PlayerName;
        }

        public static bool operator !=(HandAction a, HandAction b)
        {
            return !(a == b);
        }
        #endregion

        public static HandAction AllIn(string playername, HandActionType action, decimal amount, Street street, int actionNumber = 0)
        {
            return new HandAction(playername, action, street, true, actionNumber);
        }
    }
}
