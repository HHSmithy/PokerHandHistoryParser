using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.Limits
{
    [TestFixture("PartyPoker", "$0.10-$0.25", "$2-$4", "$25-$50", "$30-$60", "$10-$20", "$0.02-$0.04")]
    [TestFixture("OnGame", "$0.25-$0.25", "$0.25-$0.50", "$5-$10", "$5-$5", "$50-$100")]
    [TestFixture("PokerStars", "$0.08-$0.16", "$2-$4", "$25-$50", "$30-$60", "$10-$20")]
    [TestFixture("IPoker", "$0.02-$0.04", "$0.10-$0.20", "$0.50-$1", "$3-$6", "$50-$100")]
    [TestFixture("Pacific", "$1-$2", "$0.02-$0.05", "$500-$1,000", "$5-$10", "$0.50-$1")]
    [TestFixture("Merge", "$0.05-$0.10", "$0.50-$1", "$1-$2", "$5-$10", "$10-$20")]
    // Note: Have to use e instead of € otherwise the test runner reports inconclusive. Have reported this bug.
    [TestFixture("Entraction", "e0.02-e0.04", "e2-e4", "e25-e50", "e0.50-e1", "e15-e30")]
    [TestFixture("FullTilt", "$25-$50", "$5-$10", "$100-$200", "$0.50-$1", "$0.25-$0.50")]
    class HandParserLimitTests : HandHistoryParserBaseTests
    {
        private readonly string[] _expectedLimits;

        public HandParserLimitTests(string site, params string[] expectedLimits)
            : base(site)
        {
            _expectedLimits = expectedLimits;            
        }

        private void TestTLimit(int limitTestId, string fileName)
        {
            if (_expectedLimits.Length < limitTestId)
            {
                Assert.Ignore("No matching sample hand for Limit test " + fileName);
            }

            string expectedLimitString = _expectedLimits[limitTestId - 1];

            TestTLimit(expectedLimitString, fileName);
        }

        private void TestTLimit(string expectedLimitString, string fileName)
        {
            string handText = SampleHandHistoryRepository.GetLimitExampleHandHistoryText(PokerFormat.CashGame, Site, fileName);

            Assert.AreEqual(expectedLimitString.Replace("e", "€").Replace(",", "."), GetSummmaryParser().ParseLimit(handText).ToString().Replace(",", "."), "IHandHistorySummaryParser: ParseLimit");
            Assert.AreEqual(expectedLimitString.Replace("e", "€").Replace(",", "."), GetParser().ParseLimit(handText).ToString().Replace(",", "."), "IHandHistoryParser: ParseLimit");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void ParseLimit_Correct(int limitTestId)
        {
            TestTLimit(limitTestId, "Limit" + limitTestId);
        }

        [Test]        
        public void ParseLimit_AnteTable_Correct()
        {
            switch (Site)
            {
                case SiteName.IPoker:
                case SiteName.PartyPoker:
                case SiteName.OnGame:
                case SiteName.Pacific:
                case SiteName.Merge:
                case SiteName.FullTilt:
                case SiteName.Entraction:
                    Assert.Ignore(Site.ToString() + " doesn't have ante tables.");
                    break;               
                
            }

            // Stars does not contain ante information in the limit so we actually add it once we have parsed all the actions
            string handText = SampleHandHistoryRepository.GetLimitExampleHandHistoryText(PokerFormat.CashGame, Site, "AnteTable");
            string expectedLimitString = "$0.10-$0.25-Ante-$0.05";
            Assert.AreEqual(expectedLimitString, GetParser().ParseFullHandHistory(handText).GameDescription.Limit.ToString(), "IHandHistoryParser: ParseLimit");
        }

        [Test]
        public void ParseLimit_EuroTable_Correct()
        {
            switch (Site)
            {
                case SiteName.PartyPoker:
                case SiteName.Pacific:
                case SiteName.Merge:
                case SiteName.FullTilt:
                    Assert.Ignore("Site doesn't have euro tables.");
                    break;
            }

            TestTLimit("€2-€4", "EuroTable");
        }

        [Test]
        public void ParseLimit_GbpTable_Correct()
        {
            switch (Site)
            {
                case SiteName.PartyPoker:
                case SiteName.OnGame:
                case SiteName.Pacific:
                case SiteName.Merge:
                case SiteName.Entraction:
                case SiteName.FullTilt:
                    Assert.Ignore("Site doesn't have euro tables.");
                    break;
            }

            TestTLimit("£0.10-£0.25", "GbpTable");
        }
    }
}
