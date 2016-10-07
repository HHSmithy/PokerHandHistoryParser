using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Utils.Pot;
using HandHistories.Parser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Players;

namespace HandHistories.Parser.Utils
{
    public static class HandIntegrity
    {
        public static decimal MaxRakePercentage = 0.1m;

        public static void Assert(HandHistory hand, ValidationChecks checks = ValidationChecks.ALL)
        {
            string reason = null;

            if (!Check(hand, checks, out reason))
            {
                throw new HandIntegrityException(reason);
            }
        }

        public static bool Check(HandHistory hand, out string reason)
        {
            reason = null;
            return Check(hand, ValidationChecks.ALL, out reason);
        }

        public static bool Check(HandHistory hand, ValidationChecks checks, out string reason)
        {
            reason = null;
            if (checks.HasFlag(ValidationChecks.TOTAL_POT))
            {
                if (!CheckTotalPot(hand, out reason))
                {
                    return false;
                }
            }
            if (checks.HasFlag(ValidationChecks.STREET_ORDER))
            {
                if (!CheckStreetOrder(hand.HandActions, out reason))
                {
                    return false;
                }
            }
            if (checks.HasFlag(ValidationChecks.BLIND_ORDER))
            {
                if (!CheckBlindOrder(hand.HandActions, out reason))
                {
                    return false;
                }
            }
            if (checks.HasFlag(ValidationChecks.ACTION_ORDER))
            {
                if (!CheckActionOrder(hand.HandActions, out reason))
                {
                    return false;
                }
            }
            if (checks.HasFlag(ValidationChecks.ACTION_TOTAL_AMOUNTS))
            {
                if (!CheckActionTotalAmounts(hand.Players, hand.HandActions, out reason))
                {
                    return false;
                }
            }
            if (checks.HasFlag(ValidationChecks.PLAYERLIST_SITOUT_WITH_ACTIONS))
            {
                if (!CheckPlayerListSitoutWithActions(hand.Players, hand.HandActions, out reason))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool CheckPlayerListSitoutWithActions(PlayerList players, List<HandAction> actions, out string reason)
        {
            reason = null;
            foreach (var player in players.Where(p => p.IsSittingOut))
            {
                int actionCount = actions.Player(player).Count();
                if (actionCount > 0)
                {
                    reason = string.Format("Player: \"{0}\" is sitout and have {1} HandActions", player.PlayerName, actions);
                    return false;
                }
            }
            return true;
        }

        private static bool CheckActionTotalAmounts(PlayerList playerList, List<HandAction> actions, out string reason)
        {
            foreach (var player in playerList)
            {
                var amounts = actions.Player(player)
                    .Where(p => !p.IsWinningsAction && p.HandActionType != HandActionType.UNCALLED_BET)
                    .Sum(p => p.Absolute);

                var allin = actions.Player(player).Any(p => p.IsAllIn);

                if (player.StartingStack < amounts)
                {
                    reason = string.Format("Player: \"{0}\" Action amounts ({1}) is more than the players stack({2})", 
                        player.PlayerName,
                        amounts,
                        player.StartingStack);
                    return false;
                }

                if (allin && player.StartingStack != amounts)
                {
                    reason = string.Format("Player: \"{0}\" is Allin but stack ({2}) is bigger then contribution to pot ({1})",
                        player.PlayerName,
                        amounts,
                        player.StartingStack);
                    return false;
                }
            }

            reason = null;
            return true;
        }

        private static bool CheckBlindOrder(List<HandAction> list, out string reason)
        {
            reason = null;
            bool blindsEnded = false;
            foreach (var item in list)
            {
                bool validBlindPhaseAction = item.IsBlinds
                    || item.HandActionType == HandActionType.ANTE
                    || item.HandActionType == HandActionType.POSTS
                    || item.HandActionType == HandActionType.POSTS_DEAD;

                if (blindsEnded && validBlindPhaseAction)
                {
                    reason = "Blind occuring after action: #" + item;
		            return false;
                }
                if (!validBlindPhaseAction && item.HandActionType != HandActionType.FOLD)
                {
                    blindsEnded = true;
                }
            }
            return true;
        }

        private static bool CheckStreetOrder(List<HandAction> actions, out string reason)
        {
            reason = null;
            Street previousStreet = Street.Preflop;
            foreach (var item in actions)
            {
                var nextStreet = item.Street;
                switch (previousStreet)
                {
                    case Street.Preflop:
                        if (nextStreet == Street.Turn || nextStreet == Street.River)
                        {
                            reason = string.Format("Jumping Street {0} -> {1}", previousStreet, nextStreet); 
                            return false;
                        }
                        break;
                    case Street.Flop:
                        if (nextStreet == Street.Preflop || nextStreet == Street.River)
                        {
                            reason = string.Format("Jumping Street {0} -> {1}", previousStreet, nextStreet);
                            return false;
                        }
                        break;
                    case Street.Turn:
                        if (nextStreet == Street.Preflop || nextStreet == Street.Flop)
                        {
                            reason = string.Format("Jumping Street {0} -> {1}", previousStreet, nextStreet);
                            return false;
                        }
                        break;
                    case Street.River:
                        if (nextStreet == Street.Preflop || nextStreet == Street.Flop || nextStreet == Street.Turn)
                        {
                            reason = string.Format("Jumping Street {0} -> {1}", previousStreet, nextStreet);
                            return false;
                        }
                        break;
                    case Street.Showdown:
                        if (nextStreet != Street.Showdown)
                        {
                            reason = string.Format("Jumping Street {0} -> {1}", previousStreet, nextStreet);
                            return false;
                        }
                        break;
                }
                previousStreet = nextStreet;
            }

            return true;
        }

        static bool CheckActionOrder(List<HandAction> list, out string reason)
        {
            reason = null;
            bool BetOccured = true;
            int blindEndIndex = -1;

            for (int i = 0; i < list.Count; i++)
			{
                var item = list[i];
                bool validBlindPhaseAction = item.IsBlinds 
                    || item.HandActionType == HandActionType.ANTE
                    || item.HandActionType == HandActionType.FOLD
                    || item.HandActionType == HandActionType.POSTS
                    || item.HandActionType == HandActionType.POSTS_DEAD;
                if (!validBlindPhaseAction)
	            {
		            blindEndIndex = i;
                    break;
                }
			}

            Street currentStreet = Street.Preflop;
            for (int i = blindEndIndex; i < list.Count; i++)
			{
			    var item = list[i];

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
                        reason = "Cant raise/call without a bet/BB: #" + i;
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
                reason = string.Format("Rake is more than {0:0.0%}: {1:0.00}", 
                    MaxRakePercentage, 
                    hand.Rake);
                return false;
            }

            return true;
        }
    }
}
