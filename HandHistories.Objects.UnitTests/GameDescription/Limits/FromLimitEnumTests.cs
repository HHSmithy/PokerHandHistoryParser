using HandHistories.Objects.GameDescription;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HandHistories.Objects.UnitTests.GameDescription.Limits
{
    [TestFixture]
    class FromLimitEnumTests
    {
        [Test]
        public void FromLimitEnum_ParsesCorrectly()
        {
            Limit limit = Limit.FromLimitEnum(LimitEnum.Limit_125c_250c, Currency.GBP, true, 0.50m);

            ClassicAssert.AreEqual(2.50m, limit.BigBlind, "Big Blind");
            ClassicAssert.AreEqual(1.25m, limit.SmallBlind, "Small Blind");
            ClassicAssert.AreEqual(Currency.GBP, limit.Currency, "Currency");
            ClassicAssert.IsTrue(limit.IsAnteTable, "Is Ante Table");
            ClassicAssert.AreEqual(0.50m, limit.Ante, "Ante");
        }
    }
}
