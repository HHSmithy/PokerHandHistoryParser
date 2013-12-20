using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandHistories.Utilities
{
    public static class StreetUtility
    {
        public static List<HandAction> GetStreetActions(List<HandAction> HAs, Street street)
        {
            List<HandAction> actions = new List<HandAction>();
            foreach (var item in HAs)
            {
                if (item.Street == street)
                {
                    actions.Add(item);
                }
            }
            return actions;
        }

        public static HandAction GetNextHandAction(List<HandAction> list, string PlayerName, int StartIndex = 0)
        {
            for (int i = StartIndex + 1; i < list.Count; i++)
            {
                if ((list[i].IsGameAction || list[i].IsBlinds) && list[i].PlayerName == PlayerName)
                {
                    return list[i];
                }
            }
            return null;
        }

        public static HandAction GetNextHandAction(List<HandAction> list, int StartIndex = 0)
        {
            for (int i = StartIndex + 1; i < list.Count; i++)
            {
                if (list[i].IsGameAction || list[i].IsBlinds)
                {
                    return list[i];
                }
            }
            return null;
        }

        public static HandAction GetNextHandActionNumber(List<HandAction> list, string PlayerName, int StartIndex = 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ActionNumber > StartIndex && (list[i].IsGameAction || list[i].IsBlinds) && list[i].PlayerName == PlayerName)
                {
                    return list[i];
                }
            }
            return null;
        }

        public static HandAction GetNextHandActionNumber(List<HandAction> list, int StartIndex = 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ActionNumber > StartIndex && (list[i].IsGameAction || list[i].IsBlinds))
                {
                    return list[i];
                }
            }
            return null;
        }

        public static HandAction GetFirstVPIPAction(List<HandAction> HAs)
        {
            foreach (var Action in HAs)
            {
                if (Action.VPIP)
                {
                    return Action;
                }
            } 
            return null;
        }

        public static HandAction GetNextVPIPAction(List<HandAction> HAs, int ActionNumber)
        {
            for (int i = 0; i < HAs.Count; i++)
            {
                if (HAs[i].ActionNumber > ActionNumber && HAs[i].VPIP)
                {
                    return HAs[i];
                }
            }
            return null;
        }

        public static bool HandWentToShowdown(HandHistory HH)
        {
            return HH.HandActions[HH.HandActions.Count - 1].Street == Street.Showdown;
        }

        public static HandAction LastAction(List<HandAction> HAs, Street street)
        {
            List<HandAction> HAStreet = GetStreetActions(HAs, street);
            return (HAStreet.Count != 0 ? HAStreet[HAStreet.Count - 1] : null);
        }

        public static HandAction FirstAction(List<HandAction> HAs, Street street, string PlayerName)
        {
            List<HandAction> HAStreet = GetStreetActions(HAs, street);
            for (int i = 0; i < HAStreet.Count; i++)
            {
                if (HAStreet[i].PlayerName == PlayerName)
                {
                    return HAStreet[i];
                }
            }
            return null;
        }

        class PlayerNameEquality : IEqualityComparer<HandAction>
        {
            public static PlayerNameEquality Default = new PlayerNameEquality();

            public bool Equals(HandAction x, HandAction y)
            {
                return x.PlayerName == y.PlayerName;
            }

            public int GetHashCode(HandAction obj)
            {
                return obj.PlayerName.GetHashCode();
            }
        }
        public static int PlayersOnStreet(List<HandAction> HAs, Street street)
        {
            return HAs.Where(p => p.Street == street).Distinct(PlayerNameEquality.Default).Count();
        }
    }
}
