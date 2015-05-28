using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System.Globalization;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.Limits
{
    [TestFixture("PartyPoker", "$0.05-$0.10", "$0.50-$1", "$5-$10")]
    [TestFixture("OnGame", "$0.05-$0.10", "$0.50-$1", "$5-$10", "$0.25-$0.25", "$5-$5")]
    [TestFixture("OnGameIt")]
    [TestFixture("PokerStars", "$0.05-$0.10", "$0.50-$1", "$5-$10", "$100-$200", "$200-$400", "$1-$2")]
    [TestFixture("IPoker", "e0.05-e0.10", "£0.50-£1", "$5-$10")]
    [TestFixture("Pacific", "$0.05-$0.10", "$0.50-$1", "$100-$200", "$5-$10")]
    [TestFixture("Merge", "$0.05-$0.10", "$0.50-$1", "$1-$2", "$5-$10", "$10-$20")]
    // Note: Have to use e instead of € otherwise the test runner reports inconclusive. Have reported this bug.
    [TestFixture("Entraction", "e0.02-e0.04", "e2-e4", "e25-e50", "e0.50-e1", "e15-e30")]
    [TestFixture("FullTilt", "$0.05-$0.10", "$0.50-$1", "$5-$10", "$300-$600", "$2,000-$4,000")]
    [TestFixture("MicroGaming", "e0.01-e0.02", "e0.50-e1", "e1-e2")]
    [TestFixture("Winamax", "e0.05-e0.10", "e0.50-e1", "e5-e10")]
    [TestFixture("WinningPoker", "$2-$4", "$2-$4", "$0.10-$0.25")]
    [TestFixture("BossMedia", "$100-$200", "$5-$10", "$0.02-$0.04")]
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
            
            Assert.AreEqual(expectedLimitString.Replace("e", "€"), GetSummmaryParser().ParseLimit(handText).ToString(CultureInfo.InvariantCulture), "IHandHistorySummaryParser: ParseLimit");
            Assert.AreEqual(expectedLimitString.Replace("e", "€"), GetParser().ParseLimit(handText).ToString(CultureInfo.InvariantCulture), "IHandHistoryParser: ParseLimit");
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
                case SiteName.MicroGaming:
                case SiteName.IPoker:
                case SiteName.PartyPoker:
                case SiteName.OnGame:
                case SiteName.Pacific:
                case SiteName.Merge:
                case SiteName.FullTilt:
                case SiteName.Entraction:
                case SiteName.Winamax:
                case SiteName.WinningPoker:
                case SiteName.BossMedia:
                case SiteName.OnGameIt:
                    Assert.Ignore(Site.ToString() + " doesn't have ante tables.");
                    break;               
                
            }

            // Stars does not contain ante information in the limit so we actually add it once we have parsed all the actions
            string handText = SampleHandHistoryRepository.GetLimitExampleHandHistoryText(PokerFormat.CashGame, Site, "AnteTable");
            string expectedLimitString = "$0.10-$0.25-Ante-$0.05";
            Assert.AreEqual(expectedLimitString, GetParser().ParseFullHandHistory(handText).GameDescription.Limit.ToString().Replace(',', '.'), "IHandHistoryParser: ParseLimit");
        }

        [Test]
        public void ParseLimit_EuroTable_Correct()
        {
            switch (Site)
            {
                case SiteName.MicroGaming:
                case SiteName.PartyPoker:
                case SiteName.Pacific:
                case SiteName.Merge:
                case SiteName.FullTilt:
                case SiteName.Winamax:
                case SiteName.WinningPoker:
                case SiteName.BossMedia:
                case SiteName.OnGame:
                    Assert.Ignore("Site doesn't have euro tables ( example ).");
                    break;
                case SiteName.Entraction:
                    TestTLimit("e2-e4", "EuroTable");
                    break;
                default:
                    TestTLimit("e0.50-e1", "EuroTable");
                    break;
            }
        }

        [Test]
        public void ParseLimit_GbpTable_Correct()
        {
            switch (Site)
            {
                case SiteName.MicroGaming:
                case SiteName.PartyPoker:
                case SiteName.OnGame:
                case SiteName.Pacific:
                case SiteName.Merge:
                case SiteName.Entraction:
                case SiteName.FullTilt:
                case SiteName.Winamax:
                case SiteName.PokerStars:
                case SiteName.WinningPoker:
                case SiteName.BossMedia:
                case SiteName.OnGameIt:
                    Assert.Ignore("Site doesn't have euro tables.");
                    break;
                default:
                    TestTLimit("£0.05-£0.10", "GbpTable");
                    break;

            }
        }
    }
}
