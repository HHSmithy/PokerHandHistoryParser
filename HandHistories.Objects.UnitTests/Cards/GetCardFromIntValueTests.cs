using HandHistories.Objects.Cards;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HandHistories.Objects.UnitTests.Cards
{
    [TestFixture]
    public class GetCardFromIntValueTests
    {
        [Test]
        public void TestCard_FromIntValue_2cAs9h()
        {
            var card = Card.GetCardFromIntValue(0);
            ClassicAssert.AreEqual("2c", card.ToString());

            card = Card.GetCardFromIntValue(51);
            ClassicAssert.AreEqual("As", card.ToString());

            card = new Card("9","h");
            ClassicAssert.AreEqual("9h", Card.GetCardFromIntValue(card.CardIntValue).ToString());
        }

        [Test]
        public void TestCard_FromIntValue_Invalid()
        {
            var card = Card.GetCardFromIntValue(-1);
            ClassicAssert.IsTrue(card.isEmpty);
            card = Card.GetCardFromIntValue(52);
            ClassicAssert.IsTrue(card.isEmpty);
        }
    }
}
