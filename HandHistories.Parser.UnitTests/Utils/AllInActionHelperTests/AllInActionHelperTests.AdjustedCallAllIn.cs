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
    public partial class AllInActionHelperTests
    {
        void TestAdjustedCallAllInAmount(decimal expectedAmount, List<HandAction> actions, string playerName, decimal amount)
        {
            var result = AllInActionHelper.GetAdjustedCallAllInAmount(amount, actions.Player(playerName).ToList());

            Assert.AreEqual(expectedAmount, result);
        }

        [Test]
        public void AdjustedCall_AllInPreflop()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 10, Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 20, Street.Preflop),
            };

            TestAdjustedCallAllInAmount(1000, actions, "P3", 1000);
        }

        [Test]
        public void AdjustedCall_AllInPreflop_NoPreviousAction()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 10, Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 20, Street.Preflop),
                new HandAction("P3", HandActionType.RAISE, 100, Street.Preflop),
            };

            TestAdjustedCallAllInAmount(20, actions, "P2", 40);
        }
    }
}
