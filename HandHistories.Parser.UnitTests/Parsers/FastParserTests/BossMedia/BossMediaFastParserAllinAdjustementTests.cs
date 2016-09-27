using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Parser.Parsers.FastParser.BossMedia;
using HandHistories.Parser.UnitTests.Parsers.Base;
using HandHistories.Parser.Utils.AllInAction;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.BossMedia
{
    /// <summary>
    /// [VARNING] The bossmedia allin adjustments operates on unadjusted amounts
    /// 
    /// Bossmedia have a strange way of representing ALLINs: 
    /// the ALLIN action amount in the handtext = the stack in the beginning of the street
    /// For example:
    /// -Player1's stack is 100 before the hand starts
    /// -Player1 contributes 20 to the pot preflop
    /// -Player1 bets flop 40
    /// -Player1 ALLINs flop 80
    /// </summary>
    [TestFixture]
    class BossMediaFastParserAllinAdjustementTests
    {
        void TestAllInAdjustment(List<HandAction> actions, HandAction allinAction, HandActionType expectedType, decimal expectedAmount)
        {
            var allinType = BossMediaAllInAdjuster.GetAllInActionType(allinAction.PlayerName, allinAction.Absolute, allinAction.Street, actions);
            var actualAmount = BossMediaAllInAdjuster.GetAdjustedAllInAmount(allinAction.PlayerName, allinAction.Absolute, allinAction.Street, actions);

            Assert.AreEqual(expectedType, allinType);
            Assert.AreEqual(expectedAmount, actualAmount);
        }

        [Test]
        public void Boss_CallAllInPreflop()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 10, Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 20, Street.Preflop),
            };

            var action = new HandAction("P3", HandActionType.ALL_IN, 100, Street.Preflop);
            TestAllInAdjustment(actions, action, HandActionType.RAISE, 100);
        }

        [Test]
        public void Boss_CallAllInPreflop_NoPreviousAction()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 10, Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 20, Street.Preflop),
                new HandAction("P3", HandActionType.RAISE, 100, Street.Preflop),
            };

            var action = new HandAction("P2", HandActionType.ALL_IN, 40, Street.Preflop);
            TestAllInAdjustment(actions, action, HandActionType.CALL, 20);
        }

        [TestCase]
        public void Boss_RaiseAllinAdjustement_Flop_0()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 1, Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 2, Street.Preflop),
                new HandAction("P3", HandActionType.CALL, 2, Street.Preflop),
                new HandAction("P1", HandActionType.RAISE, 12, Street.Preflop),
                new HandAction("P2", HandActionType.CALL, 10, Street.Preflop),
                new HandAction("P3", HandActionType.FOLD, 0, Street.Preflop),

                new HandAction("P1", HandActionType.BET, 24, Street.Flop),
            };

            var allinAction = new HandAction("P2", HandActionType.ALL_IN, 30, Street.Flop);

            TestAllInAdjustment(actions, allinAction, HandActionType.RAISE, 30);
        }

        [TestCase]
        public void Boss_RaiseAllinAdjustement_Turn_0()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 1, Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 2, Street.Preflop),
                new HandAction("P1", HandActionType.CALL, 1, Street.Preflop),

                new HandAction("P2", HandActionType.CHECK, 0, Street.Flop),
                new HandAction("P1", HandActionType.CHECK, 0, Street.Flop),

                new HandAction("P2", HandActionType.BET, 4, Street.Turn),
                new HandAction("P1", HandActionType.RAISE, 12, Street.Turn),
            };

            var allinAction = new HandAction("P2", HandActionType.ALL_IN, 36, Street.Preflop);

            TestAllInAdjustment(actions, allinAction, HandActionType.RAISE, 34);
        }
    }
}
