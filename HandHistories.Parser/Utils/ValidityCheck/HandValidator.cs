using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Utils.Pot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils
{
    public static class HandIntegrity
    {
        public static decimal MaxRakePercentage = 0.1m;

        public static bool Check(HandHistory hand, out string reason)
        {
            reason = null;

            return CheckTotalPot(hand, out reason) && CheckActionOrder(hand.HandActions, out reason);
        }

        static bool CheckActionOrder(List<HandAction> list, out string reason)
        {
            reason = null;
            bool BetOccured = true;
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
                    reason = "Blind occuring after action: #" + i;
		            return false;
                }

                if (item.Street != currentStreet)
                {
                    BetOccured = false;
                    currentStreet = item.Street;
                }

                if (item.HandActionType == HandActionType.BET)
                {
                    if (BetOccured)
                    {
                        reason = "Cant bet twice on same street: #" + i;
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
                        reason = "Cant raise without a bet/BB: #" + i;
                        return false;
                    }
                }
			}

            return true;
        }

        static bool CheckTotalPot(HandHistory hand, out string reason)
        {
            decimal expectedPot = PotCalculator.CalculateTotalPot(hand);

            bool PotValid = Math.Abs(expectedPot) == hand.TotalPot;

            if (PotValid)
            {
                reason = null;
            }
            else
            {
                reason = string.Format("Total Pot not correct: {0} actions: {1}", hand.TotalPot, expectedPot);
                return false;
            }

            if (hand.Rake < 0)
            {
                reason = "Rake can not be smaller then zero: " + hand.Rake;
                return false;
            }
            else if (hand.Rake > MaxRakePercentage * hand.TotalPot)
            {
                reason = string.Format("Rake is more than {0}%: {1}", 
                    MaxRakePercentage * 100, 
                    hand.Rake);
                return false;
            }

            return true;
        }
    }
}
