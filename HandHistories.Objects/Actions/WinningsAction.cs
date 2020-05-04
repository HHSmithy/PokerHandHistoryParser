using System.Runtime.Serialization;
using HandHistories.Objects.Cards;
using System.Diagnostics;

namespace HandHistories.Objects.Actions
{
    [DataContract]
    [DebuggerDisplay("{ToString()}")]
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

        public bool SidePot { get { return ActionType == WinningsActionType.TIES_SIDE_POT || ActionType == WinningsActionType.WINS_SIDE_POT; } }

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
            string format = "{0} {1} for {2} in pot: {3}";

            return string.Format(format,
                PlayerName,
                ActionType,
                Amount.ToString("N2"),
                PotNumber);
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