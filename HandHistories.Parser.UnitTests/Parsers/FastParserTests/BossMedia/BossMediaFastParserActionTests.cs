using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.FastParser.BossMedia;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.BossMedia
{
    using Parser = BossMediaFastParserImpl;
    [TestFixture]
    class BossMediaFastParserActionTests : HandHistoryParserBaseTests
    {
        BossMediaFastParserImpl parser = new BossMediaFastParserImpl();

        public BossMediaFastParserActionTests()
            : base("BossMedia")
        {
        }

        void TestBlindAction(HandAction expected, string line, bool BBPOsted = false)
        {
            var action = Parser.ParseBlindAction(line, ref BBPOsted);

            Assert.AreEqual(expected, action);
        }

        void TestRegularAction(HandAction expected, string line)
        {
            List<HandAction> actions = new List<HandAction>();
            var action = Parser.ParseRegularAction(line, Street.Flop, actions);

            Assert.AreEqual(expected, action);
        }

        [Test]
        public void ParseBlindActionLine_DeadSmallBlind()
        {
            TestBlindAction(new HandAction("Player1", HandActionType.POSTS_DEAD, 0.25m, Street.Preflop), "<ACTION TYPE=\"HAND_BLINDS\" PLAYER=\"Player1\" KIND=\"HAND_DSB\" VALUE=\"0.25\"></ACTION>");
        }

        [Test]
        public void ParseBlindActionLine_Posts()
        {
            TestBlindAction(new HandAction("AllinAnna", HandActionType.POSTS, 200m, Street.Preflop), "<ACTION TYPE=\"HAND_BLINDS\" PLAYER=\"AllinAnna\" KIND=\"HAND_BB\" VALUE=\"200.00\"></ACTION>", true);
        }

        [Test]
        public void ParseBlindActionLine_SmallBlind()
        {
            TestBlindAction(new HandAction("Phyre", HandActionType.SMALL_BLIND, 100m, Street.Preflop), "<ACTION TYPE=\"HAND_BLINDS\" PLAYER=\"Phyre\" KIND=\"HAND_SB\" VALUE=\"100.00\"></ACTION>");
        }

        [Test]
        public void ParseBlindActionLine_BigBlind()
        {
            TestBlindAction(new HandAction("AllinAnna", HandActionType.BIG_BLIND, 200m, Street.Preflop), "<ACTION TYPE=\"HAND_BLINDS\" PLAYER=\"AllinAnna\" KIND=\"HAND_BB\" VALUE=\"200.00\"></ACTION>");
        }
        
        [Test]
        public void ParseRegularActionLine_Fold()
        {
            TestRegularAction(new HandAction("phallos", HandActionType.FOLD, 0m, Street.Flop), "<ACTION TYPE=\"ACTION_FOLD\" PLAYER=\"phallos\"></ACTION>");
        }

        [Test]
        public void ParseRegularActionLine_Call()
        {
            TestRegularAction(new HandAction("1Mentalist", HandActionType.CALL, 5m, Street.Flop), "<ACTION TYPE=\"ACTION_CALL\" PLAYER=\"1Mentalist\" VALUE=\"5.00\"></ACTION>");
        }

        [Test]
        public void ParseRegularActionLine_Check()
        {
            TestRegularAction(new HandAction("ItalyToast", HandActionType.CHECK, 0m, Street.Flop), "<ACTION TYPE=\"ACTION_CHECK\" PLAYER=\"ItalyToast\"></ACTION>");
        }
        


    }
}
