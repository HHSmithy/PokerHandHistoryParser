using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.GameTypes
{
    [TestFixture("PartyPoker", "JcKd")]
    [TestFixture("PokerStars", "4s7h")]
    [TestFixture("OnGame", "8h3c")]
    [TestFixture("Pacific", "3dJc")]
    [TestFixture("MicroGaming", "AhQsKs8c")]
    [TestFixture("BossMedia", "3dKd")]
    class HeroDealtToTests : HandHistoryParserBaseTests
    {
        private readonly string _expectedHeroHand;

        public HeroDealtToTests(string site, string expectedHand)
            : base(site)
        {
            _expectedHeroHand = expectedHand;
        }

        [Test]
        public void ParseHero()
        {
            string hand = SampleHandHistoryRepository.GetGeneralHandHistoryText(PokerFormat.CashGame, Site, "HeroName");

            var handhistory = GetParser().ParseFullHandHistory(hand);

            var heroHand = handhistory.Hero.HoleCards.ToString();
            Assert.AreEqual(_expectedHeroHand, heroHand, "IHandHistoryParser: ParseHeroHand");
        }
    }
}
