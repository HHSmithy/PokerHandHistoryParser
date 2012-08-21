using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.GameTypes
{    
    [TestFixture("PartyPoker")]
    [TestFixture("PokerStars")]
    [TestFixture("OnGame")]
    [TestFixture("IPoker")]
    [TestFixture("Pacific")]
    [TestFixture("Merge")]
    [TestFixture("Entraction")]
    class HandParserGameTypeTests : HandHistoryParserBaseTests 
    {
        public HandParserGameTypeTests(string site) : base(site)
        {
           
        }
        
        private void TestGameType(GameType expected)
        {
            string handText = SampleHandHistoryRepository.GetGameTypeHandHistoryText(PokerFormat.CashGame, Site, expected);

            Assert.AreEqual(expected, GetSummmaryParser().ParseGameType(handText), "IHandHistorySummaryParser: ParseGameType");
            Assert.AreEqual(expected, GetParser().ParseGameType(handText), "IHandHistoryParser: ParseGameType");
        }
        
        [Test]
        public void ParseGameType_ParsesFixedLimitHoldem()
        {
            TestGameType(GameType.FixedLimitHoldem);
        }

        [Test]
        public void ParseGameType_ParsesNoLimitHoldem()
        {
            TestGameType(GameType.NoLimitHoldem);
        }

        [Test]
        public void ParseGameType_ParsesPotLimitOmaha()
        {
            TestGameType(GameType.PotLimitOmaha);
        }


        [Test]
        public void ParseGameType_ParsesFixedLimitOmahaHiLo()
        {
            if (Site != SiteName.Entraction)
            {
                Assert.Ignore(Site + " currently doesn't have pot limit holdem.");                
                return;
            }

            TestGameType(GameType.FixedLimitOmahaHiLo);
        }

        [Test]
        public void ParseGameType_ParsesPotLimitHoldem()
        {
            switch (Site)
            {
                case SiteName.Merge:
				case SiteName.IPoker:
                case SiteName.OnGame:
                case SiteName.Pacific:
                    Assert.Ignore(Site + " currently doesn't have pot limit holdem.");
                    break;
            }

            TestGameType(GameType.PotLimitHoldem);
        }

        [Test]
        public void ParseGameType_ParsesCapNoLimitHoldem()
        {
            switch (Site)
            {
                    case SiteName.IPoker:
					case SiteName.PartyPoker:
                    case SiteName.Merge:
                    case SiteName.OnGame:
                    case SiteName.Pacific:
                    case SiteName.Entraction:
                        Assert.Ignore(Site + " currently doesn't have cap games.");
                        break;
            }            

            TestGameType(GameType.CapNoLimitHoldem);
        }

        [Test]
        public void ParseGameType_ParsesNoLimitOmaha()
        {
            switch (Site)
            {
                case SiteName.IPoker:
                case SiteName.PartyPoker:
                case SiteName.Merge:
                case SiteName.OnGame:
                case SiteName.PokerStars:
                case SiteName.PokerStarsFr:
                case SiteName.PokerStarsIt:
                case SiteName.Entraction:
                    Assert.Ignore(Site + " currently doesn't have cap games.");
                    break;
            }

            TestGameType(GameType.NoLimitOmaha);
        }

        [Test]
        public void ParseGameType_ParsesNoLimitOmahaHiLo()
        {
            switch (Site)
            {
                case SiteName.IPoker:
                case SiteName.PartyPoker:
                case SiteName.Merge:
                case SiteName.OnGame:
                case SiteName.Pacific:
                case SiteName.PokerStarsFr:
                case SiteName.PokerStarsIt:
                case SiteName.Entraction:
                    Assert.Ignore(Site + " currently doesn't have cap games.");
                    break;
            }

            TestGameType(GameType.NoLimitOmahaHiLo);
        }
        
        [Test]
        public void ParseGameType_ParsesPotLimitOmahaHiLo()
        {
            switch (Site)
            {
                case SiteName.Merge:
				case SiteName.IPoker:
                case SiteName.Pacific:
                case SiteName.Entraction:
                    Assert.Ignore(Site + " currently doesn't have hi-lo.");
                    break;
            }

            TestGameType(GameType.PotLimitOmahaHiLo);
        }

        [Test]
        public void ParseGameType_ParsesFiveCardPotLimitOmaha()
        {
            if (Site != SiteName.Entraction)
            {
                Assert.Ignore(Site + " currently doesn't have sample for " + GameType.FiveCardPotLimitOmaha);
            }

            TestGameType(GameType.FiveCardPotLimitOmaha);
        }
    }
}
