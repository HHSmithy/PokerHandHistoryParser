using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Utilities
{
    public static class PositionUtility
    {
        public static bool IsPlayerIP(HandHistory HH, string playerName, Street street)
        {
            int buttonPos = HH.DealerButtonPosition;
            List<HandAction> StreetHAs = StreetUtility.GetStreetActions(HH.HandActions, street);

            string FirstPlayer = StreetHAs[0].PlayerName;
            for (int i = 1; i < StreetHAs.Count; i++)
            {
                if (StreetHAs[i].PlayerName == FirstPlayer)
                {
                    if (StreetHAs[i - 1].PlayerName == playerName)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return StreetHAs[StreetHAs.Count - 1].PlayerName == playerName;
        }
    }
}
