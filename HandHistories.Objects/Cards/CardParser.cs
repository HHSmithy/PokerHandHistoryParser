using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.Cards
{
    static class CardParser
    {
        #region EnumLookups
        static CardValueEnum[] rankParseLookup = initRankParseLookup();

        private static CardValueEnum[] initRankParseLookup()
        {
            CardValueEnum[] ranks = new CardValueEnum[68];
            for (int i = 0; i < ranks.Length; i++)
            {
                ranks[i] = CardValueEnum.Unknown;
            }

            ranks['A' - '2'] = CardValueEnum._A;
            ranks['a' - '2'] = CardValueEnum._A;
            ranks['K' - '2'] = CardValueEnum._K;
            ranks['k' - '2'] = CardValueEnum._K;
            ranks['Q' - '2'] = CardValueEnum._Q;
            ranks['q' - '2'] = CardValueEnum._Q;
            ranks['J' - '2'] = CardValueEnum._J;
            ranks['j' - '2'] = CardValueEnum._J;
            ranks['T' - '2'] = CardValueEnum._T;
            ranks['t' - '2'] = CardValueEnum._T;
            for (int i = 2; i <= 9; i++)
            {
                ranks[i.ToString()[0] - '2'] = (CardValueEnum)(i - 2);
            }
            return ranks;
        }
        #endregion

        internal static SuitEnum ParseSuit(char suit)
        {
            switch (suit)
            {
                case 'c':
                case 'C':
                    return SuitEnum.Clubs;
                case 'd':
                case 'D':
                    return SuitEnum.Diamonds;
                case 'h':
                case 'H':
                    return SuitEnum.Hearts;
                case 's':
                case 'S':
                    return SuitEnum.Spades;
                default:
                    return SuitEnum.Unknown;
            }
        }

        internal static CardValueEnum ParseRank(char suit)
        {
            if (suit < '2' ||  'q' < suit)
            {
                return CardValueEnum.Unknown;
            }
            return rankParseLookup[suit - '2'];
        }
    }
}
