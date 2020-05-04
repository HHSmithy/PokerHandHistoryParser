using System;
using System.Runtime.Serialization;

namespace HandHistories.Objects.Cards
{
    [DataContract]
    public class BoardCards : CardGroup
    {
        public Street Street
        {
           get
           {
               switch (Cards.Count)
               {
                   case 0:
                       return Street.Preflop;
                   case 3:
                       return Street.Flop;
                   case 4:
                       return Street.Turn;
                   case 5:
                       return Street.River;
                   default:
                       throw new ArgumentException("Unknown number of board cards " + Cards.Count);
               }
           }
        }

        private BoardCards(params Card[] cards)
            : base(cards)
        {
            
        }

        public static BoardCards ForPreflop()
        {
            return new BoardCards();
        }

        public static BoardCards ForFlop(Card card1, Card card2, Card card3)
        {
            return new BoardCards(card1, card2, card3);
        }

        public static BoardCards ForTurn(Card card1, Card card2, Card card3, Card card4)
        {
            return new BoardCards(card1, card2, card3, card4);
        }

        public static BoardCards ForRiver(Card card1, Card card2, Card card3, Card card4, Card card5)
        {
            return new BoardCards(card1, card2, card3, card4, card5);
        }

        public static BoardCards FromCards(string cards)
        {
            return FromCards(Parse(cards));
        }

        public static BoardCards FromCards(Card[] cards)
        {            
            return new BoardCards(cards);
        }

        public BoardCards GetBoardOnStreet(Street streetAllIn)
        {
            switch (streetAllIn)
            {
                case Street.Preflop:
                    return BoardCards.ForPreflop();
                case Street.Flop:
                    return BoardCards.ForFlop(this[0], this[1], this[2]);
                case Street.Turn:
                    return BoardCards.ForTurn(this[0], this[1], this[2], this[3]);
                case Street.River:
                case Street.Showdown:
                    return BoardCards.ForRiver(this[0], this[1], this[2], this[3], this[4]);
                default:
                    throw new ArgumentException("Can't get board in for null street");
            }
        }
    }
}
