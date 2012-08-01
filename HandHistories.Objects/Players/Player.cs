using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using HandHistories.Objects.Cards;

namespace HandHistories.Objects.Players
{
    [DataContract]
    public class Player
    {
        [DataMember]
        public string PlayerName { get; private set; }

        [DataMember]
        public decimal StartingStack { get; private set; }

        [DataMember]
        public int SeatNumber { get; private set; }

        [DataMember]
        public HoleCards HoleCards { get; set; }

        [DataMember]
        public bool IsSittingOut { get; set; }

        public Player(string playerName, 
                      decimal startingStack,
                      int seatNumber)
        {
            PlayerName = playerName;
            StartingStack = startingStack;
            SeatNumber = seatNumber;
            HoleCards = HoleCards.NoHolecards(playerName);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj.ToString().Equals(ToString());
        }

        public override string ToString()
        {
            string s = 
                string.Format("Seat {0}: {1} [{2}] with '{3}'", SeatNumber, PlayerName, StartingStack.ToString("N2"), HoleCards.ToString());

            if (IsSittingOut)
            {
                s = "[Sitting Out] " + s;
            }

            return s;
        }
    }
}
