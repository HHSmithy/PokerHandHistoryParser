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

        /// <summary>
        /// Hole cards will be null when there are no cards,
        /// use "hasHoleCards" to find out if there are any cards
        /// </summary>
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
            HoleCards = null;
        }

        public bool hasHoleCards
        {
            get
            {
                return HoleCards != null && HoleCards.Count > 0;
            }
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

        public static bool operator ==(Player p1, Player p2)
        {
            if (ReferenceEquals(p1, null))
            {
                return ReferenceEquals(p1, p2);
            }
            else
            {
                return p1.Equals(p2);
            }
        }

        public static bool operator !=(Player p1, Player p2)
        {
            if (ReferenceEquals(p1, null))
            {
                return !ReferenceEquals(p1, p2);
            }
            else
            {
                return !p1.Equals(p2);
            }
        }

        public override string ToString()
        {
            string s = string.Format("Seat {0}: {1} [{2}] with '{3}'", SeatNumber, PlayerName, StartingStack.ToString("N2"), (hasHoleCards ? HoleCards.ToString() : ""));

            if (IsSittingOut)
            {
                s = "[Sitting Out] " + s;
            }

            return s;
        }
    }
}
