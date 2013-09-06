using System;
using System.Runtime.Serialization;
using HandHistories.Objects.Cards;

namespace HandHistories.Objects.Actions
{
    [DataContract]
    [KnownType(typeof(WinningsAction))]
    [KnownType(typeof(AllInAction))]
    public class HandAction
    {
        [DataMember]
        public string PlayerName { get; private set; }

        [DataMember]
        public HandActionType HandActionType { get; private set; }

        [DataMember]
        public decimal Amount { get; private set; }

        [DataMember]
        public Street Street { get; private set; }

        [DataMember]
        public int ActionNumber { get; private set; }

        [DataMember]
        public bool isAllIn { get; protected set; }
        
        public HandAction(string playerName, 
                          HandActionType handActionType,                           
                          decimal amount,
                          Street street, 
                          int actionNumber = 0)
        {
            Street = street;
            HandActionType = handActionType;
            PlayerName = playerName;
            Amount = GetAdjustedAmount(amount, handActionType);
            ActionNumber = actionNumber;
            isAllIn = false;
        }

        public sealed override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public sealed override bool Equals(object obj)
        {
            HandAction handAction = obj as HandAction;
            if (handAction == null) return false;

            return handAction.ToString().Equals(ToString());
        }

        public override string ToString()
        {
            return GetType().Name + ": " + PlayerName + " does " + HandActionType + " for " + Amount.ToString("N2") + " on street " + Street + "";
        }

        public void DecreaseAmount(decimal value)
        {
            Amount = Math.Abs(Amount) - Math.Abs(value);
            Amount = GetAdjustedAmount(Amount, HandActionType);
        }

        /// <summary>
        /// Actions like calling, betting, raising should be negative amounts.
        /// Actions such as winning should be positive.
        /// Actions such as chatting should be 0 and can cause false positives if people say certain things.
        /// </summary>
        /// <param name="amount">The amount in the action.</param>
        /// <param name="type">The type of the action.</param>
        /// <returns></returns>
        public static decimal GetAdjustedAmount(decimal amount, HandActionType type)
        {
            if (amount == 0m)
            {
                return 0m;
            }

            amount = Math.Abs(amount);

            switch (type)
            {
                case HandActionType.CALL:
                    return amount*-1;                    
                case HandActionType.WINS:
                    return amount;                   
                case HandActionType.WINS_SIDE_POT:
                    return amount;                   
                case HandActionType.TIES:
                    return amount;
                case HandActionType.RAISE:
                    return amount * -1;
                case HandActionType.ALL_IN:
                    return amount * -1;
                case HandActionType.BET:
                    return amount * -1;
                case HandActionType.SMALL_BLIND:
                    return amount * -1;
                case HandActionType.BIG_BLIND:
                    return amount * -1;
                case HandActionType.UNCALLED_BET:
                    return amount;
                case HandActionType.POSTS:
                    return amount*-1;
                case HandActionType.ANTE:
                    return amount * -1;
                case HandActionType.WINS_THE_LOW:
                    return amount;
                case HandActionType.ADDS:
                    return 0.0M; // when someone adds to their stack it doesnt effect their winnings in the hand
                case HandActionType.CHAT:
                    return 0.0M; // overwrite any $ talk in the chat
            }

            throw new ArgumentException("GetAdjustedAmount: Uknown action " + type + " to have amount " + amount);
        }

        public bool IsRaise
        {
            get
            {
                return HandActionType == HandActionType.RAISE;
            }
        }

        public bool IsPreFlopRaise
        {
            get
            {
                return Street == Street.Preflop && HandActionType == HandActionType.RAISE;
            }
        }

        public bool IsAllInAction
        {
            get { return isAllIn; }
        }

        public bool IsWinningsAction
        {
            get
            {
                return HandActionType == HandActionType.WINS ||
                       HandActionType == HandActionType.WINS_SIDE_POT ||
                       HandActionType == HandActionType.TIES || 
                       HandActionType == HandActionType.TIES_SIDE_POT ||
                       HandActionType == HandActionType.WINS_THE_LOW;
            }
        }

        public bool IsAggressiveAction
        {
            get
            {
                return HandActionType == HandActionType.RAISE ||
                       HandActionType == HandActionType.BET;
            }
        }

        public bool IsBlinds
        {
            get
            {
                return HandActionType == HandActionType.SMALL_BLIND ||
                       HandActionType == HandActionType.BIG_BLIND ||
                       HandActionType == HandActionType.POSTS;
            }
        }
       
        
    }
}
