using System.Runtime.Serialization;
using HandHistories.Objects.Cards;

namespace HandHistories.Objects.Actions
{
    [DataContract]
    public class AllInAction : HandAction
    {        
        [DataMember]
        public bool IsRaiseAllIn { get; private set; }

        public AllInAction(string playerName,
                           decimal amount,
                           Street street,
                           bool isRaiseAllIn,
                           int actionNumber = 0)
            : base(playerName, HandActionType.ALL_IN, amount, street, actionNumber)
        {
            IsRaiseAllIn = isRaiseAllIn;
        }

        public override string ToString()
        {
            return base.ToString() + "-RaiseAllIn=" + IsRaiseAllIn;
        }

    }
}