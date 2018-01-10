using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using HandHistories.Objects.Cards;

namespace HandHistories.Objects.Players
{
    [DataContract]
    public sealed class Player
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
            return PlayerName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Player;
            if (other != null)
            {
                return this == other;
            }
            return false;
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

        public static bool operator ==(Player p1, Player p2)
        {
            if (ReferenceEquals(p1, p2))
            {
                return true;
            }
            else if (ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
            {
                return false;
            }
            else
            {
                return p1.PlayerName == p2.PlayerName &&
                    p1.SeatNumber == p2.SeatNumber &&
                    p1.StartingStack == p2.StartingStack &&
                    p1.HoleCards == p2.HoleCards &&
                    p1.IsSittingOut == p2.IsSittingOut;
            }
        }

        public static bool operator !=(Player p1, Player p2)
        {
            return !(p1 == p2);
        }
    }
}
