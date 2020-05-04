using HandHistories.Objects.GameDescription;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.Limits
{
    [TestFixture("PartyPoker", new string[] { "$0.05-$0.10", "$0.50-$1", "$5-$10" })]
    [TestFixture("OnGame", new string[] { "$0.05-$0.10", "$0.50-$1", "$5-$10", "$0.25-$0.25", "$5-$5", "€5-€10" })]
    //[TestFixture("OnGameIt")]
    [TestFixture("PokerStars", new string[] { "$0.05-$0.10", "$0.50-$1", "$5-$10", "$100-$200", "$200-$400", "$1-$2" })]
    [TestFixture("IPoker", new string[] { "e0.05-e0.10", "£0.50-£1", "$5-$10" })]
    [TestFixture("Pacific", new string[] { "$0.05-$0.10", "$0.50-$1", "$100-$200", "$5-$10", "$25-$50" })]
    [TestFixture("Merge", new string[] { "$0.05-$0.10", "$0.50-$1", "$1-$2", "$5-$10", "$10-$20" })]
    // Note: Have to use e instead of € otherwise the test runner reports inconclusive. Have reported this bug.
    [TestFixture("Entraction", new string[] { "e0.02-e0.04", "e2-e4", "e25-e50", "e0.50-e1", "e15-e30" })]
    [TestFixture("FullTilt", new string[] { "$0.05-$0.10", "$0.50-$1", "$5-$10", "$300-$600", "$2,000-$4,000" })]
    [TestFixture("MicroGaming", new string[] { "e0.01-e0.02", "e0.50-e1", "e1-e2" })]
    [TestFixture("Winamax", new string[] { "e0.05-e0.10", "e0.50-e1", "e5-e10" })]
    [TestFixture("WinningPoker", new string[] { "$2-$4", "$2-$4", "$0.10-$0.25" })]
    [TestFixture("WinningPokerV2", new string[] { "$0.50-$1" })]
    [TestFixture("BossMedia", new string[] { "$100-$200", "$5-$10", "$0.02-$0.04" })]
    [TestFixture("IGT", new string[] { "SEK0.25-SEK0.50", "SEK10-SEK20" })]
    class HandParserLimitTests_CashGame : HandParserLimitTests
    {
        public HandParserLimitTests_CashGame(string site, params string[] expectedLimits) 
            : base(PokerFormat.CashGame, site, expectedLimits)
        {
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
                case SiteName.WinningPokerV2:
                case SiteName.BossMedia:
                case SiteName.OnGameIt:
                case SiteName.IGT:
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
                case SiteName.WinningPokerV2:
                case SiteName.BossMedia:
                case SiteName.OnGame:
                case SiteName.IGT:
                    Assert.Ignore("Site doesn't have euro tables ( example ).");
                    break;
                case SiteName.Entraction:
                    TestLimit("e2-e4", "EuroTable");
                    break;
                default:
                    TestLimit("e0.50-e1", "EuroTable");
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
                case SiteName.WinningPokerV2:
                case SiteName.BossMedia:
                case SiteName.OnGameIt:
                case SiteName.IGT:
                    Assert.Ignore("Site doesn't have euro tables.");
                    break;
                default:
                    TestLimit("£0.05-£0.10", "GbpTable");
                    break;

            }
        }
        [Test]
        public void ParseLimit_YuanTable_Correct()
        {
            switch (Site)
            {
                case SiteName.IPoker:
                case SiteName.MicroGaming:
                case SiteName.PartyPoker:
                case SiteName.OnGame:
                case SiteName.Pacific:
                case SiteName.Merge:
                case SiteName.Entraction:
                case SiteName.FullTilt:
                case SiteName.Winamax:
                case SiteName.WinningPoker:
                case SiteName.WinningPokerV2:
                case SiteName.BossMedia:
                case SiteName.OnGameIt:
                case SiteName.IGT:
                    Assert.Ignore("Site doesn't have yuan tables.");
                    break;
                default:
                    TestLimit("¥25-¥50", "YuanTable");
                    break;

            }
        }

    }
}
