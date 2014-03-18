using HandHistories.Objects.Cards;
using NUnit.Framework;
using System;

namespace HandHistories.Objects.UnitTests.Cards
{
    [TestFixture]
    public class CardEqualityTests
    {
        [Test]
        public void Card_TestEquality()
        {
            if (new Card('2', 'c') != new Card("2", "C"))
            {
                throw new Exception("Card: 2c is not equal 2c.");
            }
        }
    }
}
