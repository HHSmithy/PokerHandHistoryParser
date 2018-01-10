using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.Cards
{
    partial struct Card
    {
        #region Lookups
        public static readonly Card[] AllCards = GetAllCardStrings().Select(p => Card.Parse(p)).ToArray();

        static readonly string[] AllCardStrings = GetAllCardStrings();

        private static string[] GetAllCardStrings()
        {
            List<string> cards = new List<string>(52);
            foreach (var rank in (CardValueEnum[])Enum.GetValues(typeof(CardValueEnum)))
            {
                if (rank == CardValueEnum.Unknown)
                {
                    continue;
                }
                foreach (var suit in (SuitEnum[])Enum.GetValues(typeof(SuitEnum)))
                {
                    if (suit == SuitEnum.Unknown)
                    {
                        continue;
                    }

                    cards.Add(string.Concat(rank.ToString()[1], char.ToLower(suit.ToString()[0])));
                }
            }
            return cards.OrderBy(p => Card.Parse(p).CardIntValue).ToArray();
        }
        #endregion

        public static Card Unknown
        {
            get
            {
                return new Card(CardValueEnum.Unknown, 0);
            }
        }

        public static Card GetCardFromIntValue(int value)
        {
            //Sanity check
            if (value < 0 || 51 < value)
            {
                //Because card is a struct we cant return null, 
                //however there is a property isEmpty that is true when this method fails
                throw new ArgumentOutOfRangeException(string.Format("Value: {0} is not valid supply a value between 0 and 51", value));
            }

            var suit = value / 13;
            var rank = value % 13;

            return new Card((CardValueEnum)rank, (SuitEnum)suit);
        }

        public static Card Parse(string card)
        {
            if (card.Length != 2)
            {
                throw new ArgumentException("Cards must be length 2. Format Rs where R is rank and s is suit.");
            }

            return new Card(card[0], card[1]);
        }
    }
}
