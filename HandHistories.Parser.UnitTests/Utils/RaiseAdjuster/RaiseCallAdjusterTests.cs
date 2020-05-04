using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Parser.Utils.RaiseAdjuster;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Utils
{
    [TestFixture]
    class RaiseCallAdjusterTests
    {
        const Street PF = Street.Preflop;

        void TestRaiseCallAdjuster(List<HandAction> expected, List<HandAction> actions)
        {
            var result = RaiseAdjuster.AdjustRaiseSizesAndCalls(actions);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void RaiseCallAdjuster_1()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("SB", HandActionType.SMALL_BLIND, 200, PF),
                new HandAction("BB", HandActionType.BIG_BLIND, 400, PF),

                new HandAction("UTG", HandActionType.RAISE, 1400, PF),
                new HandAction("CO", HandActionType.CALL, 1400, PF),
                new HandAction("BTN", HandActionType.RAISE, 6200, PF),
                new HandAction("SB", HandActionType.FOLD, 0, PF),
                new HandAction("BB", HandActionType.FOLD, 0, PF),
                new HandAction("UTG", HandActionType.CALL, 6200, PF),
                new HandAction("CO", HandActionType.RAISE, 25400, PF),
                new HandAction("BTN", HandActionType.CALL, 10070, PF),
                new HandAction("UTG", HandActionType.CALL, 17984, PF),
            };

            List<HandAction> expected = new List<HandAction>()
            {
                new HandAction("SB", HandActionType.SMALL_BLIND, 200, PF),
                new HandAction("BB", HandActionType.BIG_BLIND, 400, PF),

                new HandAction("UTG", HandActionType.RAISE, 1400, PF),
                new HandAction("CO", HandActionType.CALL, 1400, PF),
                new HandAction("BTN", HandActionType.RAISE, 6200, PF),
                new HandAction("SB", HandActionType.FOLD, 0, PF),
                new HandAction("BB", HandActionType.FOLD, 0, PF),
                new HandAction("UTG", HandActionType.CALL, 4800, PF),
                new HandAction("CO", HandActionType.RAISE, 24000, PF),
                new HandAction("BTN", HandActionType.CALL, 3870, PF),
                new HandAction("UTG", HandActionType.CALL, 11784, PF),
            };

            TestRaiseCallAdjuster(expected, actions);
        }
    }
}
