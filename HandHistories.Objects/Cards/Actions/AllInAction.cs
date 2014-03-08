using System.Runtime.Serialization;
using HandHistories.Objects.Cards;

namespace HandHistories.Objects.Actions
{
    [DataContract]
    public sealed class AllInAction : HandAction
    {        
        public AllInAction(string playerName,
                           decimal amount,
                           Street street,
                           bool isRaiseAllIn,
                           int actionNumber = 0)
            : base(playerName, (isRaiseAllIn ? HandActionType.RAISE : HandActionType.CALL), amount, street, actionNumber)
        {
            isAllIn = true;
        }

        public AllInAction(string playerName,
                           decimal amount,
                           Street street,
                           HandActionType HType,
                           int actionNumber = 0)
            : base(playerName, HType, amount, street, actionNumber)
        {
            isAllIn = true;
        }
    }
}