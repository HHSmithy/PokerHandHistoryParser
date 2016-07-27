using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.FastParser.PokerStars;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.HandActionTests
{
    abstract internal class HandParserHandActionTests : HandHistoryParserBaseTests
    {
        PokerFormat format = PokerFormat.CashGame;

        protected HandParserHandActionTests(string site)
            : base(site)
        {
        }

        protected HandParserHandActionTests(PokerFormat format, string site)
            : base(site)
        {
            this.format = format;
        }   

        protected void TestParseActions(string fileName, List<HandAction> expectedActions)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(format, Site, "HandActionTests", fileName);

            List<HandAction> actionList = GetParser().ParseHandActions(handText);

            Assert.AreEqual(expectedActions.Count, actionList.Count, "Action List Count");
            Assert.AreEqual(expectedActions, actionList);
        }

        protected abstract List<HandAction> ExpectedHandActionsBasicHand { get; }
        protected abstract List<HandAction> ExpectedHandActionsFoldedPreflop { get; }
        protected abstract List<HandAction> ExpectedHandActions3BetHand { get; }
        protected abstract List<HandAction> ExpectedHandActionsAllInHand { get; }
        protected abstract List<HandAction> ExpectedOmahaHiLoHand { get; }

        [Test]
        public void ParseHandActions_BasicHand()
        {
            TestParseActions("BasicHand", ExpectedHandActionsBasicHand);
        }

        [Test]
        public void ParseHandActions_FoldedPreflop()
        {
            TestParseActions("FoldedPreflop", ExpectedHandActionsFoldedPreflop);
        }

        [Test]
        public void ParseHandActions_3BetHand()
        {
            TestParseActions("3BetHand", ExpectedHandActions3BetHand);
        }

        [Test]
        public void ParseHandActions_AllInHand()
        {
            TestParseActions("AllInHandWithShowdown", ExpectedHandActionsAllInHand);
        }

        [Test]
        public void ParseHandActions_OmahaHiLoHands()
        {
            switch (Site)
            {              
                case SiteName.Winamax:
                case SiteName.Pacific:
                case SiteName.WinningPoker:
                    Assert.Ignore("No example for omaha hi-lo hands for site " + Site);
                    break;
            }

            TestParseActions("OmahaHiLo", ExpectedOmahaHiLoHand);
        }  
    }
}
