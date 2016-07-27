using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.GameTypes
{
    [TestFixture("PartyPoker")]
    [TestFixture("MicroGaming")]
    class HandParserGameTypeTests_MTT : HandParserGameTypeTests
    {
        public HandParserGameTypeTests_MTT(string site)
            : base(PokerFormat.MultiTableTournament, site)
        {
        }

        [Test]
        public void ParseGameType_ParsesNoLimitHoldem()
        {
            TestGameType(GameType.NoLimitHoldem);
        }

        //[Test]
        //public void ParseGameType_ParsesFixedLimitHoldem()
        //{
        //    switch (Site)
        //    {
        //        case SiteName.Winamax:
        //            Assert.Ignore(Site + " currently doesn't have FL example.");
        //            return;
        //        case SiteName.WinningPoker:
        //            Assert.Ignore(Site + " does not make a diffrence for Fixed/No Limit/Limit");
        //            return;
        //    }
        //    TestGameType(GameType.FixedLimitHoldem);
        //}

        //[Test]
        //public void ParseGameType_ParsesPotLimitOmaha()
        //{
        //    switch (Site)
        //    {
        //        case SiteName.FullTilt:
        //            Assert.Ignore(Site + " currently doesn't have Pot Limit Omaha example.");
        //            return;
        //    }

        //    TestGameType(GameType.PotLimitOmaha);
        //}

        //[Test]
        //public void ParseGameType_ParsesFixedLimitOmahaHiLo()
        //{
        //    if (Site != SiteName.Entraction)
        //    {
        //        Assert.Ignore(Site + " currently doesn't have Fixed Limit Omaha HiLo.");
        //        return;
        //    }

        //    TestGameType(GameType.FixedLimitOmahaHiLo);
        //}

        //[Test]
        //public void ParseGameType_ParsesPotLimitHoldem()
        //{
        //    switch (Site)
        //    {
        //        case SiteName.MicroGaming:
        //        case SiteName.Merge:
        //        case SiteName.IPoker:
        //        case SiteName.FullTilt:
        //        case SiteName.OnGame:
        //        case SiteName.Pacific:
        //        case SiteName.Winamax:
        //        case SiteName.PartyPoker:
        //        case SiteName.BossMedia:
        //            Assert.Ignore(Site + " currently doesn't have pot limit holdem.");
        //            break;
        //        case SiteName.WinningPoker:
        //            Assert.Ignore(Site + " does not make a diffrence for Fixed/No Limit/Limit");
        //            return;
        //    }

        //    TestGameType(GameType.PotLimitHoldem);
        //}

        //[Test]
        //public void ParseGameType_ParsesNoLimitOmaha()
        //{
        //    switch (Site)
        //    {
        //        case SiteName.MicroGaming:
        //        case SiteName.IPoker:
        //        case SiteName.PartyPoker:
        //        case SiteName.Merge:
        //        case SiteName.OnGame:
        //        case SiteName.PokerStars:
        //        case SiteName.PokerStarsFr:
        //        case SiteName.PokerStarsIt:
        //        case SiteName.FullTilt:
        //        case SiteName.Entraction:
        //        case SiteName.Winamax:
        //        case SiteName.Pacific:
        //        case SiteName.BossMedia:
        //            Assert.Ignore(Site + " currently doesn't have No Limit Omaha example.");
        //            break;
        //        case SiteName.WinningPoker:
        //            Assert.Ignore(Site + " does not make a diffrence for Fixed/No Limit/Limit");
        //            return;
        //    }

        //    TestGameType(GameType.NoLimitOmaha);
        //}

        //[Test]
        //public void ParseGameType_ParsesNoLimitOmahaHiLo()
        //{
        //    switch (Site)
        //    {
        //        case SiteName.MicroGaming:
        //        case SiteName.IPoker:
        //        case SiteName.PartyPoker:
        //        case SiteName.Merge:
        //        case SiteName.OnGame:
        //        case SiteName.Pacific:
        //        case SiteName.PokerStarsFr:
        //        case SiteName.FullTilt:
        //        case SiteName.PokerStarsIt:
        //        case SiteName.Entraction:
        //        case SiteName.Winamax:
        //        case SiteName.BossMedia:
        //            Assert.Ignore(Site + " currently doesn't have No Limit Omaha HiLo example.");
        //            break;
        //        case SiteName.WinningPoker:
        //            Assert.Ignore(Site + " does not make a diffrence for Fixed/No Limit/Limit");
        //            return;
        //    }

        //    TestGameType(GameType.NoLimitOmahaHiLo);
        //}

        //[Test]
        //public void ParseGameType_ParsesPotLimitOmahaHiLo()
        //{
        //    switch (Site)
        //    {
        //        case SiteName.Merge:
        //        case SiteName.IPoker:
        //        case SiteName.FullTilt:
        //        case SiteName.Entraction:
        //        case SiteName.Winamax:
        //            Assert.Ignore(Site + " currently doesn't have Pot Limit Omaha HiLo example.");
        //            break;
        //        case SiteName.WinningPoker:
        //            Assert.Ignore(Site + " does not make a diffrence for Fixed/No Limit/Limit");
        //            return;
        //    }

        //    TestGameType(GameType.PotLimitOmahaHiLo);
        //}

        //[Test]
        //public void ParseGameType_ParsesFiveCardPotLimitOmaha()
        //{
        //    if (Site != SiteName.Entraction)
        //    {
        //        Assert.Ignore(Site + " currently doesn't have sample for " + GameType.FiveCardPotLimitOmaha);
        //    }

        //    TestGameType(GameType.FiveCardPotLimitOmaha);
        //}
    }
}
