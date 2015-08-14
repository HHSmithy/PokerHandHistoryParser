using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils
{
    public static class HandIntegrity
    {
        public static bool Check(HandHistory hand)
        {
            return CheckTotalPot(hand) && CheckActionOrder(hand.HandActions);
        }

        static bool CheckActionOrder(List<HandAction> list)
        {
            bool BetOccured = false;
            int blindEndIndex = -1;

            for (int i = 0; i < list.Count; i++)
			{
                var item = list[i];
			    if (!item.IsBlinds && item.HandActionType != HandActionType.ANTE)
	            {
		            blindEndIndex = i;
                    break;
                }
			}


            Street currentStreet = Street.Preflop;
            for (int i = blindEndIndex; i < list.Count; i++)
			{
			    var item = list[i];
			    if (item.IsBlinds || item.HandActionType == HandActionType.ANTE)
	            {
		            return false;
                }

                if (item.Street != currentStreet)
                {
                    if ((int)item.Street < (int)currentStreet)
                    {
                        return false;
                    }

                    BetOccured = false;
                    currentStreet = item.Street;
                }

                if (item.HandActionType == HandActionType.BET)
                {
                    if (BetOccured)
                    {
                        return false;
                    }
                    else
                    {
                        BetOccured = true;
                    }
                }

                if (item.HandActionType == HandActionType.RAISE || item.HandActionType == HandActionType.CALL)
                {
                    if (!BetOccured)
                    {
                        return false;
                    }
                }
			}

            return true;
        }

        static bool CheckTotalPot(HandHistory hand)
        {
            var totalPot = hand.HandActions
                .Where(p => p.IsGameAction || p.IsBlinds)
                .Sum(p => p.Amount);

            return Math.Abs(totalPot) == hand.TotalPot;
        }
    }
}
