using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Parser.Utils.AllInAction;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Utils.AllInActionHelperTests
{
    [TestFixture]
    public class AllInActionHelperTests
    {
        void TestAllInActionHelper(string playerName, decimal amount, Street street, List<HandAction> actions, HandActionType expectedAllInActionType)
        {
            var result = AllInActionHelper.GetAllInActionType(playerName, amount, street, actions);

            Assert.AreEqual(expectedAllInActionType, result);
        }

        [Test]
        public void AllInPreflop()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 10, Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 20, Street.Preflop),
            };

            TestAllInActionHelper("P1", 100, Street.Preflop, actions, HandActionType.RAISE);
        }
    }
}
