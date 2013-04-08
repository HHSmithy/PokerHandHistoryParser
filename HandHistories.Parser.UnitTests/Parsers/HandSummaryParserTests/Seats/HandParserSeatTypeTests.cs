using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.Seats
{
    [TestFixture("PartyPoker")]
    [TestFixture("PokerStars")]
    [TestFixture("OnGame")]
    [TestFixture("IPoker")]
    [TestFixture("Pacific")]
    [TestFixture("Merge")]
    [TestFixture("Entraction")]
    [TestFixture("FullTilt")]
    class HandParserSeatTypeTests : HandHistoryParserBaseTests 
    {
        public HandParserSeatTypeTests(string site) : base(site)
        {
        }

        private void TestSeatType(SeatType expectedSeatType)
        {
            string handText = SampleHandHistoryRepository.GetSeatExampleHandHistoryText(PokerFormat.CashGame, Site, expectedSeatType);

            Assert.AreEqual(expectedSeatType, GetSummmaryParser().ParseSeatType(handText), "IHandHistorySummaryParser: ParseSeatType");
            Assert.AreEqual(expectedSeatType, GetParser().ParseSeatType(handText), "IHandHistoryParser: ParseSeatType");
        }

        [Test]
        public void ParseSeatType_HeadsUp()
        {
            TestSeatType(SeatType.FromMaxPlayers(2));
        }

        [Test]
        public void ParseSeatType_6Max()
        {
            TestSeatType(SeatType.FromMaxPlayers(6));
        }

        [Test]
        public void ParseSeatType_4Max()
        {
            switch (Site)
            {
                case SiteName.IPoker:
                case SiteName.Merge:
                case SiteName.PartyPoker:
                case SiteName.PokerStars:
                case SiteName.OnGame:
                case SiteName.Entraction:
                case SiteName.Pacific:
                case SiteName.FullTilt:
                    Assert.Ignore(Site + " currently doesn't have 4 max games.");
                    break;
            }            

            TestSeatType(SeatType.FromMaxPlayers(4));
        }

        [Test]
        public void ParseSeatType_10Handed()
        {
            switch (Site)
            {
                case SiteName.PartyPoker:
                case SiteName.Merge:
                case SiteName.OnGame:
                case SiteName.Entraction:
                case SiteName.FullTilt:
                case SiteName.Pacific:
                    Assert.Ignore(Site + " currently doesn't have 10 handed games.");
                    break;
            }            

            TestSeatType(SeatType.FromMaxPlayers(10));
        }

        [Test]
        public void ParseSeatType_9Handed()
        {
            TestSeatType(SeatType.FromMaxPlayers(9));
        }

    }
}
