using System;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.PokerStars
{
    [TestFixture]
    class IGTJSONParserExtraTests : HandHistoryParserBaseTests 
    {
        public IGTJSONParserExtraTests() : base("IGT")
        {
        }

        [Test]
        public void CanParse2019Hand()
        {
            var handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "ExtraHands", "IGT2019Hand");
            var hand = GetParser().ParseFullHandHistory(handText, true);

            var limit = hand.GameDescription.Limit;
            Assert.AreEqual(0.25m, limit.SmallBlind);
            Assert.AreEqual(0.50m, limit.BigBlind);
            Assert.AreEqual(Currency.SEK, limit.Currency);
            Assert.AreEqual(0m, limit.Ante);
        }
    }
}
