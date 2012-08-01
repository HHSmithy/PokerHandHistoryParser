using HandHistories.Objects.Cards;
using NUnit.Framework;

namespace HandHistories.Objects.UnitTests.Cards
{
    [TestFixture]
    class CardConstructorTests
    {
        [Test]
        public void NewCard_Th_HasCorrectValue()
        {
            Card card = new Card("T", "h");

            Assert.AreEqual("Th", card.CardStringValue);
            Assert.AreEqual(34, card.CardIntValue);
        }

        [Test]
        public void NewCard_Td_HasCorrectValue()
        {
            Card card = new Card("T", "d");

            Assert.AreEqual("Td", card.CardStringValue);
            Assert.AreEqual(21, card.CardIntValue);
        }

        [Test]
        public void NewCard_2c_HasCorrectValue()
        {
            Card card = new Card("2", "c");

            Assert.AreEqual("2c", card.CardStringValue);
            Assert.AreEqual(0, card.CardIntValue);
        }

        [Test]
        public void NewCard_As_HasCorrectValue()
        {
            Card card = new Card("A", "s");

            Assert.AreEqual("As", card.CardStringValue);
            Assert.AreEqual(51, card.CardIntValue);
        }

        [Test]
        public void NewCard_aS_CorrectsCapitalization()
        {
            Card card = new Card("a", "S");

            Assert.AreEqual("As", card.CardStringValue);
            Assert.AreEqual(51, card.CardIntValue);
        }

        [Test]
        public void NewCard_InvalidRank_ThrowsCardException()
        {
            Assert.Throws<System.ArgumentException>(() => new Card("1", "d"), "Rank is not correctly formatted. Should be 2-9, T, J, Q, K or A.");            
        }

        [Test]
        public void NewCard_InvalidSuit_ThrowsCardException()
        {
            Assert.Throws<System.ArgumentException>(() => new Card("T", "x"), "Suit is not correctly formatted. Should be c, d, h or s.");
        }
    }
}
