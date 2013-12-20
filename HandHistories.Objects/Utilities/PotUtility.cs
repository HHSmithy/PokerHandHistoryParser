using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Objects.GameDescription;

namespace HandHistories.Utilities
{
    public static class PotUtility
    {
        public static decimal GetPot(List<HandAction> HAs, int index, bool IncludeIndex = false)
        {
            if (IncludeIndex)
            {
                index += 1;
            }
            decimal Pot = 0;
            for (int i = 0; i < index; i++)
            {
                if (HAs[i].IsGameAction || HAs[i].IsBlinds)
                {
                    Pot -= HAs[i].Amount;
                }
            }
            return Pot;
        }

        public static decimal GetActionPotSize(List<HandAction> HAs, HandAction HA)
        {
            decimal Pot = GetPot(HAs, HA.ActionNumber);
            return Math.Abs(HA.Amount) / Pot;
        }

        public static double StackSizeInBBs(Limit HH, Player player)
        {
            return (double)(player.StartingStack / HH.BigBlind);
        }
    }
}
