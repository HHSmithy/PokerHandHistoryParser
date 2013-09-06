using System.Runtime.Serialization;
using HandHistories.Objects.Cards;

namespace HandHistories.Objects.Actions
{
    [DataContract]
    public class AllInAction : HandAction
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

        public override string ToString()
        {
            return base.ToString() + " and is AllIn";
        }
    }
}