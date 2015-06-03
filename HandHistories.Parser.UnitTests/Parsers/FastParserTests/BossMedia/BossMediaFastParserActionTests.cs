using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using NUnit.Framework;
using HandHistories.Parser.UnitTests.Parsers.Base;
using HandHistories.Parser.Parsers.FastParser.BossMedia;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.BossMedia
{
    [TestFixture]
    class BossMediaFastParserActionTests : HandHistoryParserBaseTests
    {
        BossMediaFastParserImpl parser = new BossMediaFastParserImpl();

        public BossMediaFastParserActionTests()
            : base("BossMedia")
        {
        }

        void TestBlindAction(HandAction expected, string line)
        {
            var action = BossMediaFastParserImpl.ParseBlinds(line);

            Assert.AreEqual(expected, action);
        }

        [Test]
        public void ParseBlindActionLine_DeadSmallBlind()
        {
            TestBlindAction(new HandAction("Player1", HandActionType.POSTS, 0.25m, Street.Preflop), "<ACTION TYPE=\"HAND_BLINDS\" PLAYER=\"Player1\" KIND=\"HAND_DSB\" VALUE=\"0.25\"></ACTION>");
        }
    }
}
