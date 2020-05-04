using System;
using System.Runtime.Serialization;

namespace HandHistories.Objects.Cards
{
    [DataContract]
    public class HoleCards : CardGroup
    {
        [DataMember]
        public string PlayerName { get; private set; }

        private HoleCards(string playerName, params Card [] cards) : base(cards)
        {
            PlayerName = playerName;
        }

        public static HoleCards ForHoldem(Card card1, Card card2)
        {
            return new HoleCards(string.Empty, card1, card2);
        }

        public static HoleCards ForHoldem(string playerName, Card card1, Card card2)
        {
            return new HoleCards(playerName, card1, card2);
        }

        public static HoleCards ForOmaha(Card card1, Card card2, Card card3, Card card4)
        {
            return new HoleCards(string.Empty, card1, card2, card3, card4);
        }

        public static HoleCards ForOmaha(string playerName, Card card1, Card card2, Card card3, Card card4)
        {            
            return new HoleCards(playerName, card1, card2, card3, card4);
        }

        public static HoleCards ForOmaha5(Card card1, Card card2, Card card3, Card card4, Card card5)
        {
            return new HoleCards(string.Empty, card1, card2, card3, card4, card5);
        }

        public static HoleCards ForOmaha5(string playerName, Card card1, Card card2, Card card3, Card card4, Card card5)
        {
            return new HoleCards(playerName, card1, card2, card3, card4, card5);
        }

        public static HoleCards NoHolecards()
        {
            return new HoleCards(string.Empty);
        }

        public static HoleCards NoHolecards(string playerName)
        {
            return new HoleCards(playerName);
        }

        public static HoleCards FromCards(string playerName, string cards)
        {
            Card[] cardsArray = Parse(cards);

            return FromCards(playerName, cardsArray);
        }

        public static HoleCards FromCards(string cards)
        {
            Card[] cardsArray = Parse(cards);

            return FromCards(string.Empty, cardsArray);
        }

        public static HoleCards FromCards(string playerName, Card[] cards)
        {
            if (cards.Length == 0)
            {
                return NoHolecards();
            }
            if (cards.Length > 5)
            {
                throw new ArgumentException("Hole cards cant contain more than 5 cards.");
            }
            return new HoleCards(playerName, cards);
        }       
    }
}
