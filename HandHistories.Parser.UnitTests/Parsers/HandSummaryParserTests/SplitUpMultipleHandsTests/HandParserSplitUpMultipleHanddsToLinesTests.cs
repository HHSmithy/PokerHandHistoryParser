using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    internal class HandParserSplitUpMultipleHandsToLinesTests : HandHistoryParserBaseTests
    {
        public HandParserSplitUpMultipleHandsToLinesTests(string site)
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
            var parser = (GetParser() as HandHistories.Parser.Parsers.FastParser.Base.HandHistoryParserFastImpl);

            if (parser == null)
            {
                Assert.Ignore("No FastParser Available");
                return;
            }

            IEnumerable<string[]> actualSplitHands = parser.SplitUpMultipleHandsToLines(handText);

            Assert.AreEqual(expectedHandCount, actualSplitHands.Count(), "IHandHistorySummaryParser: SplitUpMultipleHands");

            Assert.IsTrue(actualSplitHands.All(s => s.Length > 0 ));
        }

        [TestCase(10)]
        public void SplitUpMultipleHandsToLines_ReturnsCorrectNumberOfHands(int expectedCount)
        {
            TestSplittingUpHands(expectedCount);
        }

        [TestCase(5)]
        public void SplitUpMultipleHandsToLines_ExtraLines_ReturnsCorrectNumberOfHands(int expectedCount)
        {
            if (Site != SiteName.PartyPoker)
            {
                Assert.Ignore("No example for site " + Site);
            }
            TestSplittingUpHands(expectedCount);
        }
    }
}
