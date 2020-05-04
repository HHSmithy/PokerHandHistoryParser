using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.FastParser.PokerStars;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
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

        //protected void TestParseActions(string fileName, List<HandAction> expectedActions)
        //{
        //    string handText = SampleHandHistoryRepository.GetHandExample(format, Site, "HandActionTests", fileName);

        //    List<HandAction> actionList = GetParser().ParseHandActions(handText);

        //    Assert.AreEqual(expectedActions.Count, actionList.Count, "Action List Count");
        //    Assert.AreEqual(expectedActions, actionList);
        //}

        protected void TestParseActions(string fileName, List<HandAction> expectedActions, List<WinningsAction> expectedWinners)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(format, Site, "HandActionTests", fileName);

            List<WinningsAction> realWinners;
            List<HandAction> actionList = GetParser().ParseHandActions(handText, out realWinners);

            Assert.AreEqual(expectedActions.Count, actionList.Count, "Action List Count");
            Assert.AreEqual(expectedActions, actionList);
            Assert.AreEqual(expectedWinners, realWinners);
        }

        protected abstract List<HandAction> ExpectedHandActionsBasicHand { get; }
        protected abstract List<HandAction> ExpectedHandActionsFoldedPreflop { get; }
        protected abstract List<HandAction> ExpectedHandActions3BetHand { get; }
        protected abstract List<HandAction> ExpectedHandActionsAllInHand { get; }
        protected abstract List<HandAction> ExpectedHandActionsUncalledBetHand { get; }
        protected abstract List<HandAction> ExpectedOmahaHiLoHand { get; }
        protected virtual List<HandAction> ExpectedHandActionsSplitPot { get { Assert.Ignore(); throw new NotImplementedException(); } }

        protected abstract List<WinningsAction> ExpectedWinnersHandActionsBasicHand { get; }
        protected abstract List<WinningsAction> ExpectedWinnersHandActionsFoldedPreflop { get; }
        protected abstract List<WinningsAction> ExpectedWinnersHandActions3BetHand { get; }
        protected abstract List<WinningsAction> ExpectedWinnersHandActionsAllInHand { get; }
        protected abstract List<WinningsAction> ExpectedWinnersHandActionsUncalledBetHand { get; }
        protected abstract List<WinningsAction> ExpectedWinnersOmahaHiLoHand { get; }
        protected virtual List<WinningsAction> ExpectedWinnersSplitPot { get { Assert.Ignore(); throw new NotImplementedException(); } }

        [Test]
        public void ParseHandActions_BasicHand()
        {
            TestParseActions("BasicHand", ExpectedHandActionsBasicHand, ExpectedWinnersHandActionsBasicHand);
        }

        [Test]
        public void ParseHandActions_FoldedPreflop()
        {
            TestParseActions("FoldedPreflop", ExpectedHandActionsFoldedPreflop, ExpectedWinnersHandActionsFoldedPreflop);
        }

        [Test]
        public void ParseHandActions_3BetHand()
        {
            TestParseActions("3BetHand", ExpectedHandActions3BetHand, ExpectedWinnersHandActions3BetHand);
        }

        [Test]
        public void ParseHandActions_AllInHand()
        {
            TestParseActions("AllInHandWithShowdown", ExpectedHandActionsAllInHand, ExpectedWinnersHandActionsAllInHand);
        }

        [Test]
        public void ParseHandActions_UncalledBetHand()
        {
            switch (Site)
            {
                case SiteName.BossMedia:
                case SiteName.PokerStars:
                case SiteName.Winamax:
                    TestParseActions("UncalledBet", ExpectedHandActionsUncalledBetHand, ExpectedWinnersHandActionsUncalledBetHand);
                    return;
            }
            Assert.Ignore();
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

            TestParseActions("OmahaHiLo", ExpectedOmahaHiLoHand, ExpectedWinnersOmahaHiLoHand);
        }

        [Test]
        public void ParseHandActions_SplitPot()
        {
            TestParseActions("SplitPot", ExpectedHandActionsSplitPot, ExpectedWinnersSplitPot);
        }
    }
}
