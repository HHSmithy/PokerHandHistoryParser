using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace HandHistories.Objects.Cards
{
    //When Card is a struct it only allocates 2 byte on the stack instead of 4 Reference bytes and two strings on the heap
    //Combined with lookup tables and using enums we get a 20x speedup of parsing cards
    /// <summary>
    /// Represents a card.
    /// Uses 2 bytes of memory.
    /// </summary>
    [DataContract]
    public partial struct Card
    {
        #region Properties
        public int RankNumericValue
        {
            get
            {
                return (int)Rank + 2;
            }
        }
        public int SuitNumericValue
        {
            get
            {
                return (int)Suit;
            }
        }

        /// <summary>
        /// 2c = 0, 3c = 1, ..., Ac = 12, ..., As = 51. Returns -1 if there was an error with the rank or suit values.
        /// </summary>
        public int CardIntValue
        {
            get
            {
                return (int)Suit * 13 + (int)Rank;
            }
        }
        #endregion

        #region Members
        [DataMember]
        public readonly SuitEnum Suit;

        [DataMember]
        public readonly CardValueEnum Rank; 
        #endregion

        #region Constructors
        public Card(char rank, char suit)
        {
            Rank = CardParser.ParseRank(rank);
            Suit = CardParser.ParseSuit(suit);

            if (Suit == SuitEnum.Unknown || Rank == CardValueEnum.Unknown)
            {
                throw new ArgumentException("Hand is not correctly formatted. Value: " + rank + " Suit: " + suit);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="rank">Rank should be 2-9,T,J,Q,K,A.</param>
        /// <param name="suit">Suit should be c,d,h,s.</param>
        public Card(string rank, string suit) : this(rank[0], suit[0])
        {
        }

        private Card(CardValueEnum rank, SuitEnum suit)
        {
            Rank = rank;
            Suit = suit;
        }
        #endregion

        #region Operators
        public static bool operator ==(Card c1, Card c2)
        {
            return c1.Rank == c2.Rank && c1.Suit == c2.Suit;
        }

        public static bool operator !=(Card c1, Card c2)
        {
            return c1.Rank != c2.Rank || c1.Suit != c2.Suit;
        }

        public static explicit operator Card(string card)
        {
            return Card.Parse(card);
        }
        #endregion

        #region Functions
        public override string ToString()
        {
            return AllCardStrings[CardIntValue];
        }

        public override bool Equals(object obj)
        {
            if (obj is Card)
            {
                Card other = (Card)obj;
                return Rank == other.Rank && Suit == other.Suit;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return CardIntValue;
        } 
        #endregion
    }
}
