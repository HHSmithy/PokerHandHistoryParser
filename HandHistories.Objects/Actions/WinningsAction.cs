using System.Runtime.Serialization;
using HandHistories.Objects.Cards;

namespace HandHistories.Objects.Actions
{
    [DataContract]
    public class WinningsAction
    {
        [DataMember]
        public string PlayerName { get; private set; }

        [DataMember]
        public WinningsActionType ActionType { get; protected set; }

        [DataMember]
        public decimal Amount { get; private set; }

        [DataMember]
        public int PotNumber { get; private set; }

        public WinningsAction(string playerName,
                              WinningsActionType actionType, 
                              decimal amount,
                              int potNumber)
        {
            PlayerName = playerName;
            ActionType = actionType;
            Amount = amount;
            PotNumber = potNumber;
        }

        public override string ToString()
        {
            return base.ToString() + "-Pot" + PotNumber;
        }

        public override int GetHashCode()
        {
            return PlayerName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is WinningsAction)
            {
                var other = (WinningsAction)obj;

                return PlayerName == other.PlayerName
                    && Amount == other.Amount
                    && ActionType == other.ActionType
                    && PotNumber == other.PotNumber;
            }
            return false;
        }

        public void DecreaseAmount(decimal amount)
        {
            Amount -= amount;
        }
    }
}