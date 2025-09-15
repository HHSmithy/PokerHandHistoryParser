using HandHistories.Objects.Cards;
using NUnit.Framework;
using NUnit.Framework.Legacy;


namespace HandHistories.Objects.UnitTests.Cards
{
    [TestFixture]
    class CardConstructorTests
    {
        [Test]
        public void NewCard_Th_HasCorrectValue()
        {
            Card card = new Card("T", "h");

            ClassicAssert.AreEqual("Th", card.CardStringValue);
            ClassicAssert.AreEqual(34, card.CardIntValue);
        }

        [Test]
        public void NewCard_Td_HasCorrectValue()
        {
            Card card = new Card("T", "d");

            ClassicAssert.AreEqual("Td", card.CardStringValue);
            ClassicAssert.AreEqual(21, card.CardIntValue);
        }

        [Test]
        public void NewCard_2c_HasCorrectValue()
        {
            Card card = new Card("2", "c");

            ClassicAssert.AreEqual("2c", card.CardStringValue);
            ClassicAssert.AreEqual(0, card.CardIntValue);
        }

        [Test]
        public void NewCard_As_HasCorrectValue()
        {
            Card card = new Card("A", "s");

            ClassicAssert.AreEqual("As", card.CardStringValue);
            ClassicAssert.AreEqual(51, card.CardIntValue);
        }

        [Test]
        public void NewCard_As_FromIntValue()
        {
            Card card = Card.GetCardFromIntValue(51);

            ClassicAssert.AreEqual("As", card.CardStringValue);
            ClassicAssert.AreEqual(51, card.CardIntValue);
        }

        [Test]
        public void NewCard_aS_CorrectsCapitalization()
        {
            Card card = new Card("a", "S");

            ClassicAssert.AreEqual("As", card.CardStringValue);
            ClassicAssert.AreEqual(51, card.CardIntValue);
        }

        [Test]
        public void NewCard_InvalidRank_ThrowsCardException()
        {
            ClassicAssert.Throws<System.ArgumentException>(() => new Card("1", "d"), "Rank is not correctly formatted. Should be 2-9, T, J, Q, K or A.");            
        }

        [Test]
        public void NewCard_InvalidSuit_ThrowsCardException()
        {
            ClassicAssert.Throws<System.ArgumentException>(() => new Card("T", "x"), "Suit is not correctly formatted. Should be c, d, h or s.");
        }
    }
}
