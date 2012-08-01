using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HandHistories.Objects.Cards
{
    [DataContract]
    public abstract class CardGroup : IEnumerable<Card>
    {
        [DataMember]
        protected readonly List<Card> Cards;

        protected CardGroup(params Card[] cards)
        {
            Cards = cards.ToList();

            if (Cards.Select(c => c.CardIntValue).Distinct().Count() != cards.Length)
            {
                throw new ArgumentException("Hole cards must be unique cards.");
            }
        }

        public static Card[] Parse(string cards)
        {
            if (cards == null)
            {
                return new Card[] {};
            }

            cards = cards.Replace(" ", ""); // strip white space
            cards = cards.Replace(",", ""); // strip commas

            List<Card> cardsList = new List<Card>();
            for (int i = 0; i < cards.Length; i += 2)
            {
                cardsList.Add(Card.Parse(cards.Substring(i, 2)));
            }
            return cardsList.ToArray();
        }        

        public void AddCard(Card card)
        {
            if (Cards.Contains(card))
            {
                throw new ArgumentException("Card " + card.ToString() + " already exists.");
            }

            if (Cards.Count >= 5)
            {
                throw new ArgumentException("Board can't consist of more than 5 cards.");
            }

            Cards.Add(card);
        }

        public void AddCards(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                AddCard(card);
            }
        }

        public Card this[int i]
        {
            get { return Cards[i]; }
        }

        public int[] GetIntValue()
        {
            return Cards.Select(c => c.CardIntValue).ToArray();
        }

        public int Count
        {
            get { return Cards.Count; }
        }

        public override string ToString()
        {
            return string.Join("", Cards.Select(c => c.CardStringValue));
        }

        public override bool Equals(object obj)
        {
            bool stringEquality = obj.ToString().Equals(ToString());
            if (stringEquality) return true;

            CardGroup cardGroup = obj as CardGroup;
            if (cardGroup == null) return false;

            if (cardGroup.Cards.Count != Cards.Count) return false;

            return (cardGroup.Cards.All(c => Cards.Contains(c)));
        }
       
        public override int GetHashCode()
        {
            return (Cards != null ? Cards.GetHashCode() : 0);
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return Cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}