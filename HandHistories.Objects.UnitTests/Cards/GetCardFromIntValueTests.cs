﻿using HandHistories.Objects.Cards;
using NUnit.Framework;
using System;

namespace HandHistories.Objects.UnitTests.Cards
{
    [TestFixture]
    public class GetCardFromIntValueTests
    {
        [Test]
        public void TestCard_FromIntValue_2cAs9h()
        {
            var card = Card.GetCardFromIntValue(0);
            Assert.AreEqual("2c", card.ToString());

            card = Card.GetCardFromIntValue(51);
            Assert.AreEqual("As", card.ToString());

            card = new Card("9","h");
            Assert.AreEqual("9h", Card.GetCardFromIntValue(card.CardIntValue).ToString());
        }

        [Test]
        public void TestCard_FromIntValue_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(delegate { Card.GetCardFromIntValue(-1); });
            Assert.Throws<ArgumentOutOfRangeException>(delegate { Card.GetCardFromIntValue(52); });
        }
    }
}
