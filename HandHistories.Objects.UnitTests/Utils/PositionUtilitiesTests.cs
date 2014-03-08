using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Utilities;
using HandHistories.Objects.Hand;

namespace HandHistories.Objects.UnitTests.Utils
{
    class PositionUtilitiesTests
    {
        [Test]
        public void Utilities_PlayersOnStreet()
        {
            HandHistory HH = new HandHistory() { DealerButtonPosition = 0, HandActions = PotUtilityTest.TestActions1 };
            Assert.AreEqual(PositionUtility.IsPlayerIP(HH, "yrrrhh33", Objects.Cards.Street.Flop), false);
            Assert.AreEqual(PositionUtility.IsPlayerIP(HH, "yrrrhh33", Objects.Cards.Street.Turn), false);
            Assert.AreEqual(PositionUtility.IsPlayerIP(HH, "yrrrhh33", Objects.Cards.Street.River), false);

            Assert.AreEqual(PositionUtility.IsPlayerIP(HH, "igalo1979", Objects.Cards.Street.Flop), false);
            Assert.AreEqual(PositionUtility.IsPlayerIP(HH, "igalo1979", Objects.Cards.Street.Turn), false);
            Assert.AreEqual(PositionUtility.IsPlayerIP(HH, "igalo1979", Objects.Cards.Street.River), false);

            Assert.AreEqual(PositionUtility.IsPlayerIP(HH, "lillil32", Objects.Cards.Street.Flop), true);
            Assert.AreEqual(PositionUtility.IsPlayerIP(HH, "lillil32", Objects.Cards.Street.Turn), true);
            Assert.AreEqual(PositionUtility.IsPlayerIP(HH, "lillil32", Objects.Cards.Street.River), true);
        }
    }
}
