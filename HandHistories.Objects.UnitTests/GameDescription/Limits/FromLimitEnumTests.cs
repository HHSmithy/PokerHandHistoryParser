using HandHistories.Objects.GameDescription;
using NUnit.Framework;

namespace HandHistories.Objects.UnitTests.GameDescription.Limits
{
    [TestFixture]
    class FromLimitEnumTests
    {
        [Test]
        public void FromLimitEnum_ParsesCorrectly()
        {
            Limit limit = Limit.FromLimitEnum(LimitEnum.Limit_125c_250c, Currency.GBP, true, 0.50m);

            Assert.AreEqual(2.50m, limit.BigBlind, "Big Blind");
            Assert.AreEqual(1.25m, limit.SmallBlind, "Small Blind");
            Assert.AreEqual(Currency.GBP, limit.Currency, "Currency");
            Assert.IsTrue(limit.IsAnteTable, "Is Ante Table");
            Assert.AreEqual(0.50m, limit.Ante, "Ante");
        }
    }
}
