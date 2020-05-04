using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.GameTypes
{
    [TestFixture("PartyPoker", "PP_Hero")]
    [TestFixture("PokerStars", "PS_Hero")]
    [TestFixture("OnGame", "ONG_Hero")]
    [TestFixture("IPoker", "IPK_Hero")]
    [TestFixture("Pacific", "PAC_Hero")]
    //[TestFixture("Merge", "")]
    //[TestFixture("Entraction", "")]
    [TestFixture("FullTilt", "FT_Hero")]
    [TestFixture("MicroGaming", "MGN_Hero")]
    [TestFixture("Winamax", "WM_Hero")]
    [TestFixture("WinningPoker", "WP_Hero")]
    [TestFixture("WinningPokerV2", "WP_Hero")]
    [TestFixture("BossMedia", "BO_Hero")]
    class HandParserHeroNameTests : HandHistoryParserBaseTests
    {
        private readonly string _expectedHeroName;

        public HandParserHeroNameTests(string site, string expectedHero)
            : base(site)
        {
            _expectedHeroName = expectedHero;
        }

        [Test]
        public void ParseHero()
        {
            string hand = SampleHandHistoryRepository.GetGeneralHandHistoryText(PokerFormat.CashGame, Site, "HeroName");

            var handhistory = GetParser().ParseFullHandHistory(hand);
            Assert.AreEqual(_expectedHeroName, handhistory.Hero.PlayerName, "IHandHistoryParser: ParseHeroName");
        }
    }
}
