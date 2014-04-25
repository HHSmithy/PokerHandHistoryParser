using System;
using System.Runtime.Serialization;

namespace HandHistories.Objects.Cards
{
    //When Card is a struct it only allocates 1 byte on the stack instead of 4 Reference bytes and two strings on the heap
    //Combined with lookup tables and using enums we get a 20x speedup of parsing cards
    [DataContract]
    public partial struct Card
    {
        const int SuitCardMask = 0xF0;
        const int RankCardMask = 0x0F;

        #region Properties
        private SuitEnum suit
        {
            get
            {
                return (SuitEnum)((int)_card & SuitCardMask);
            }
        }

        private CardValueEnum rank
        {
            get
            {
                return (CardValueEnum)((int)_card & RankCardMask);
            }
        }

        public string Rank
        {
            get
            {
                int rank = (int)_card & RankCardMask;
                return ((CardValueEnum)rank).ToString().Substring(1);
            }
        }

        public int RankNumericValue
        {
            get
            {
                return (int)_card & RankCardMask;
            }
        }

        public string Suit
        {
            get
            {
                int suit = (int)_card & SuitCardMask;
                return ((SuitEnum)suit).ToString().Remove(1).ToLower();
            }
        }

        public string CardStringValue
        {
            get { return _card.ToString().Substring(1); }
        }

        public bool isEmpty
        {
            get
            {
                return _card == CardEnum.Unknown;
            }
        }
        #endregion

        [DataMember]
        private CardEnum _card;

        #region Constructors
        public Card(char rank, char suit)
        {
            CardValueEnum _rank = rankParseLookup[rank];

            SuitEnum _suit = suitParseLookup[suit];

            _card = (CardEnum)((int)_suit + (int)_rank);
            if (_suit == SuitEnum.Unknown || _rank == CardValueEnum.Unknown)
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

        private Card(SuitEnum suit, CardValueEnum rank)
        {
            _card = (CardEnum)((int)suit + (int)rank);
        }
        #endregion

        #region Operators
        public static bool operator ==(Card c1, Card c2)
        {
            return c1._card == c2._card;
        }

        public static bool operator !=(Card c1, Card c2)
        {
            return c1._card != c2._card;
        }

        public static explicit operator Card(string card)
        {
            return Card.Parse(card);
        }
        #endregion

        public static string [] PossibleRanksHighCardFirst
        {
            get
            {
                return new string[]
                       {
                           "A",
                           "K",
                           "Q",
                           "J",
                           "T",
                           "9",
                           "8",
                           "7",
                           "6",
                           "5",
                           "4",
                           "3",
                           "2"
                       };
            }            
        }

        public static Card GetCardFromIntValue(int value)
        {
            //Sanity check
            if (value >= 52 || value <= -1)
            {
                //Because card is a struct we cant return null, 
                //however there is a property isEmpty that is true when this method fails
                return new Card();
            }

            var suit = (int) (value/13);
            var rank = value % 13;

            //suit starts at zero and SuitEnum starts at 1
            SuitEnum suitValue = (SuitEnum)((suit + 1) << 4);

            //rank starts at zero and CardValueEnum starts at 2
            CardValueEnum rankValue = (CardValueEnum)rank + 2;
            return new Card(suitValue, rankValue);                
        }

        public static Card Parse(string card)
        {
            if (card.Length != 2)
                throw new ArgumentException("Cards must be length 2. Format Rs where R is rank and s is suit.");

            return new Card(card[0], card[1]);
        }
       
        /// <summary>
        /// 2c = 0, 3c = 1, ..., Ac = 12, ..., As = 51. Returns -1 if there was an error with the rank or suit values.
        /// </summary>
        public int CardIntValue
        {
            get
            {
                int rankValue = (int)rank;
                int suitValue = ((int)suit >> 4) - 1;

                // note minus 1 is so we can index by 0
                return (suitValue*13) + rankValue - 2;
            }
        }

        public static string GetMaximumRank(string rank1, string rank2)
        {
            return GetRankNumericValue(rank1) > GetRankNumericValue(rank2) ? rank1 : rank2;
        }

        public static string GetMinimumRank(string rank1, string rank2)
        {
            return GetRankNumericValue(rank1) < GetRankNumericValue(rank2) ? rank1 : rank2;
        }

        public static int GetRankNumericValue(string rank1)
        {
            return (int)rankParseLookup[rank1[0]];
        }

        public override string ToString()
        {
            return CardStringValue;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj.ToString().Equals(ToString());
        }

        public override int GetHashCode()
        {
            return _card.GetHashCode();
        }
    }
}
