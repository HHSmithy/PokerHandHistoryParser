using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.GameTypes
{
    [TestFixture("PartyPoker", "PP_Hero")]
    [TestFixture("PokerStars", "PS_Hero")]
    [TestFixture("OnGame", "ONG_Hero")]
    [TestFixture("Pacific", "PAC_Hero")]
    [TestFixture("MicroGaming", "MGN_Hero")]
    [TestFixture("BossMedia", "BO_Hero")]
    [TestFixture("IGT", "HERO")]
    class DealtToHeroTests : HandHistoryParserBaseTests
    {
        private readonly string _expectedHero;

        public DealtToHeroTests(string site, string expectedName)
            : base(site)
        {
            _expectedHero = expectedName;
        }

        [Test]
        public void ParseHero()
        {
            string hand = SampleHandHistoryRepository.GetGeneralHandHistoryText(PokerFormat.CashGame, Site, "HeroName");

            var heroName = GetParser().ParseHeroName(hand);

            Assert.AreEqual(_expectedHero, heroName, "IHandHistoryParser: ParseHeroName");
        }
    }
}
