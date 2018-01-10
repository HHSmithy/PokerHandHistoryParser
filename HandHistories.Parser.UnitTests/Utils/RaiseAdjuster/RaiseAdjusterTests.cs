﻿using HandHistories.Objects.Actions;
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
    class RaiseAdjusterTests
    {
        const Street PF = Street.Preflop;

        void TestRaiseAdjuster(List<HandAction> expected, List<HandAction> actions)
        {
            var result = RaiseAdjuster.AdjustRaiseSizes(actions);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void RFI_SB()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("SB", HandActionType.SMALL_BLIND, 1, PF),
                new HandAction("BB", HandActionType.BIG_BLIND, 2, PF),
                new HandAction("SB", HandActionType.RAISE, 8, PF),
                new HandAction("BB", HandActionType.RAISE, 30, PF),
                new HandAction("SB", HandActionType.RAISE, 100, PF),
            };

            List<HandAction> expected = new List<HandAction>()
            {
                new HandAction("SB", HandActionType.SMALL_BLIND, 1, PF),
                new HandAction("BB", HandActionType.BIG_BLIND, 2, PF),
                new HandAction("SB", HandActionType.RAISE, 7, PF),
                new HandAction("BB", HandActionType.RAISE, 28, PF),
                new HandAction("SB", HandActionType.RAISE, 92, PF),
            };

            TestRaiseAdjuster(expected, actions);
        }

        [Test]
        public void CC_RAISE_SB()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("SB", HandActionType.SMALL_BLIND, 1, PF),
                new HandAction("BB", HandActionType.BIG_BLIND, 2, PF),
                new HandAction("SB", HandActionType.CALL, 1, PF),
                new HandAction("BB", HandActionType.RAISE, 8, PF),
                new HandAction("SB", HandActionType.RAISE, 30, PF),
            };

            List<HandAction> expected = new List<HandAction>()
            {
                new HandAction("SB", HandActionType.SMALL_BLIND, 1, PF),
                new HandAction("BB", HandActionType.BIG_BLIND, 2, PF),
                new HandAction("SB", HandActionType.CALL, 1, PF),
                new HandAction("BB", HandActionType.RAISE, 6, PF),
                new HandAction("SB", HandActionType.RAISE, 28, PF),
            };

            TestRaiseAdjuster(expected, actions);
        }
    }
}
