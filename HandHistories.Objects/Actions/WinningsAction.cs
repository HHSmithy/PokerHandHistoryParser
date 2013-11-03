using System.Runtime.Serialization;
using HandHistories.Objects.Cards;

namespace HandHistories.Objects.Actions
{
    [DataContract]
    public sealed class WinningsAction : HandAction
    {
        [DataMember]
        public int PotNumber { get; private set; }

        public WinningsAction(string playerName, 
                              HandActionType handActionType, 
                              decimal amount,                               
                              int potNumber,
                              int actionNumber = 0) : base(playerName, handActionType, amount, Street.Showdown, actionNumber)
        {
            PotNumber = potNumber;
        }

        public override string ToString()
        {
            return base.ToString() + "-Pot" + PotNumber;
        }
    }
}