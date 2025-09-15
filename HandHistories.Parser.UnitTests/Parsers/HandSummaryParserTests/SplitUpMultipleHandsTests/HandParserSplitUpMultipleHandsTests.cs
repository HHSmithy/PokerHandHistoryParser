using System.Collections.Generic;
using System.Linq;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.SplitUpMultipleHandsTests
{
    [TestFixture("PartyPoker")]
    [TestFixture("PokerStars")]
    [TestFixture("OnGame")]
    [TestFixture("Entraction")]
    [TestFixture("IPoker")]
    [TestFixture("Merge")]
    [TestFixture("FullTilt")]
    [TestFixture("MicroGaming")]
    [TestFixture("Winamax")]
    [TestFixture("WinningPoker")]
    [TestFixture("BossMedia")]
    internal class HandParserSplitUpMultipleHandsTests : HandHistoryParserBaseTests
    {
        public HandParserSplitUpMultipleHandsTests(string site)
            : base(site)
        {

        }

        private void TestSplittingUpHands(int expectedHandCount)
        {
            string handText = SampleHandHistoryRepository.GetMultipleHandExampleText(PokerFormat.CashGame, Site, expectedHandCount);

            TestSplittingUpHands(handText, expectedHandCount);
        }

        private void TestSplittingUpHands(string handText, int expectedHandCount)
        {           
            IEnumerable<string> actualSplitHands = GetSummmaryParser().SplitUpMultipleHands(handText);

            ClassicAssert.AreEqual(expectedHandCount, actualSplitHands.Count(), "IHandHistorySummaryParser: SplitUpMultipleHands");

            ClassicAssert.IsTrue(actualSplitHands.All(s => string.IsNullOrWhiteSpace(s) == false));
        }

        [TestCase(10)]
        public void SplitUpMultipleHands_ReturnsCorrectNumberOfHands(int expectedCount)
        {
            TestSplittingUpHands(expectedCount);
        }

        [TestCase(5)]
        public void SplitUpMultipleHands_ExtraLines_ReturnsCorrectNumberOfHands(int expectedCount)
        {
            if (Site != SiteName.PartyPoker)
            {
                ClassicAssert.Ignore("No example for site " + Site);
            }
            TestSplittingUpHands(expectedCount);
        }

        [TestCase(58)]
        public void SplitUpMultipleHands_NoSpace_ReturnsCorrectNumberOfHands(int expectedCount)
        {
            if (Site != SiteName.PokerStars)
            {
                ClassicAssert.Ignore("No example for site " + Site);    
            }

            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "MultipleHandsTests", "NoNewLinesMultipleHands");

            TestSplittingUpHands(handText, expectedCount);
        }

        [Test]
        public void SplitUpMultipleHands_MinerFormat_ReturnsCorrectNumberOfHands()
        {
            if (Site != SiteName.IPoker)
            {
                ClassicAssert.Ignore("No example for site " + Site);
            }

            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "MultipleHandsTests", "MinerFormat");

            TestSplittingUpHands(handText, 4);
        }
    }
}
