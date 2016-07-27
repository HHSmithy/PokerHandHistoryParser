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

        [Test]
        public void RaiseAllInFlop()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 10, Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 20, Street.Preflop),
                new HandAction("P3", HandActionType.CALL, 20, Street.Preflop),
                new HandAction("P1", HandActionType.CALL, 10, Street.Preflop),
                new HandAction("P2", HandActionType.CHECK, 0, Street.Preflop),

                new HandAction("P1", HandActionType.BET, 40, Street.Flop),
                new HandAction("P2", HandActionType.FOLD, 0, Street.Flop),
                new HandAction("P3", HandActionType.RAISE, 100, Street.Flop),
            };

            TestAllInActionHelper("P1", 100, Street.Flop, actions, HandActionType.RAISE);
        }

        [Test]
        public void CallAllInFlop()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 10, Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 20, Street.Preflop),
                new HandAction("P1", HandActionType.CALL, 10, Street.Preflop),

                new HandAction("P1", HandActionType.BET, 40, Street.Flop),
                new HandAction("P2", HandActionType.RAISE, 100, Street.Flop),
            };

            TestAllInActionHelper("P1", 50, Street.Flop, actions, HandActionType.CALL);
        }
    }
}
