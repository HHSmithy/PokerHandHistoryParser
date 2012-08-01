using System;
using System.Runtime.Serialization;

namespace HandHistories.Objects.Cards
{
    [DataContract]
    public class Card
    {
        [DataMember]
        private readonly string _rank;
        
        [DataMember]
        private readonly string _suit;

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
            var suit = (int) (value/13);
            var rank = value % 13;
            string suitString = null, rankString = null;
            switch (suit)
            {
                case 0:
                    suitString = "c";
                    break;
                case 1:
                    suitString = "d";
                    break;
                case 2:
                    suitString = "h";
                    break;
                case 3:
                    suitString = "s";
                    break;
                default:
                    break;
            }

            switch (rank)
            {
                case 0:
                    rankString = "2";
                    break;
                case 1:
                    rankString = "3";
                    break;
                case 2:
                    rankString = "4";
                    break;
                case 3:
                    rankString = "5";
                    break;
                case 4:
                    rankString = "6";
                    break;
                case 5:
                    rankString = "7";
                    break;
                case 6:
                    rankString = "8";
                    break;
                case 7:
                    rankString = "9";
                    break;
                case 8:
                    rankString = "T";
                    break;
                case 9:
                    rankString = "J";
                    break;
                case 10:
                    rankString = "Q";
                    break;
                case 11:
                    rankString = "K";
                    break;
                case 12:
                    rankString = "A";
                    break;
                default:
                    break;
            }

            if (rankString != null && suitString != null)
            {
                return new Card(rankString, suitString);                
            }
            return null;
        }

        public Card(char rank, char suit) : this(rank.ToString(), suit.ToString()) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rank">Rank should be 2-9,T,J,Q,K,A.</param>
        /// <param name="suit">Suit should be c,d,h,s.</param>
        public Card(string rank, string suit)
        {
            suit = suit.ToLower();
            rank = rank.ToUpper();

            if (!IsValidSuit(suit))
                throw new ArgumentException("Suit is not correctly formatted. Should be c, d, h or s.");

            if (!IsValidRank(rank))
                throw new ArgumentException("Rank is not correctly formatted. Should be 2-9, T, J, Q, K or A.");

            _rank = rank;
            _suit = suit;            
        }

        public String Rank
        {
            get { return _rank; }
        }

        public int RankNumericValue
        {
            get
            {
                return GetRankNumericValue(_rank);
            }
        }

        public static int GetRankNumericValue(string rank)
        {
            switch (rank)
            {
                case "A":
                    return 14;
                case "K":
                    return 13;
                case "Q":
                    return 12;
                case "J":
                    return 11;
                case "T":
                    return 10;
                default:
                    return Int32.Parse(rank);
            }
        }

        public String Suit
        {
            get { return _suit; }
        }

        public String CardStringValue
        {
            get  { return _rank + _suit; }
        }

        public override string ToString()
        {
            return CardStringValue;
        }

        public override bool Equals(object obj)
        {
            return obj.ToString().Equals(ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
       
        /// <summary>
        /// 2c = 0, 3c = 1, ..., Ac = 12, ..., As = 51. Returns -1 if there was an error with the rank or suit values.
        /// </summary>
        public int CardIntValue
        {
            get
            {
                int rankValue;

                switch (_rank)
                {
                    case "2":
                        rankValue = 1;
                        break;
                    case "3":
                        rankValue = 2;
                        break;
                    case "4":
                        rankValue = 3;
                        break;
                    case "5":
                        rankValue = 4;
                        break;
                    case "6":
                        rankValue = 5;
                        break;
                    case "7":
                        rankValue = 6;
                        break;
                    case "8":
                        rankValue = 7;
                        break;
                    case "9":
                        rankValue = 8;
                        break;
                    case "T":
                        rankValue = 9;
                        break;
                    case "J":
                        rankValue = 10;
                        break;
                    case "Q":
                        rankValue = 11;
                        break;
                    case "K":
                        rankValue = 12;
                        break;
                    case "A":
                        rankValue = 13;
                        break;
                    default:
                        return -1;
                }

                int suitValue;

                switch (_suit)
                {
                    case "c":
                        suitValue = 0;
                        break;
                    case "d":
                        suitValue = 1;
                        break;
                    case "h":
                        suitValue = 2;
                        break;
                    case "s":
                        suitValue = 3;
                        break;
                    default:
                        return -1;
                }

                // note minus 1 is so we can index by 0
                return (suitValue*13) + rankValue - 1;
            }
        }

        public static Card Parse(string card)
        {
            if (card.Length != 2)
                throw new ArgumentException("Cards must be length 2. Format Rs where R is rank and s is suit.");

            return new Card(card[0], card[1]);
        }
        
        private static bool IsValidRank(string rank)
        {
            rank = rank.ToUpper();

            return (rank.Equals("2") ||
                    rank.Equals("3") ||
                    rank.Equals("4") ||
                    rank.Equals("5") ||
                    rank.Equals("6") ||
                    rank.Equals("7") ||
                    rank.Equals("8") ||
                    rank.Equals("9") ||
                    rank.Equals("T") ||
                    rank.Equals("J") ||
                    rank.Equals("Q") ||
                    rank.Equals("K") ||
                    rank.Equals("A"));
        }

        private static bool IsValidSuit(string suit)
        {
            suit = suit.ToLower();

            return (suit.Equals("c") || 
                    suit.Equals("d") || 
                    suit.Equals("h") || 
                    suit.Equals("s"));
        }

        public static string GetMaximumRank(string rank1, string rank2)
        {
            return GetRankNumericValue(rank1) > GetRankNumericValue(rank2) ? rank1 : rank2;
        }

        public static string GetMinimumRank(string rank1, string rank2)
        {
            return GetRankNumericValue(rank1) < GetRankNumericValue(rank2) ? rank1 : rank2;
        }
    }
}
