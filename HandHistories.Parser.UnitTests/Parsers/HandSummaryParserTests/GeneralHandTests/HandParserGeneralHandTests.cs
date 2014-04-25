using System;
using System.Globalization;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.GeneralHandTests
{
    [TestFixture("PartyPoker", 13550319286l, "1/6/2014 09:15:12", 3, 9, null, null, "GeneralHand")]
    [TestFixture("PokerStars", 109681313810l, "1/6/2014 12:21:05", 4, 6, 0.06, 1.27, "GeneralHand")]
    [TestFixture("PokerStars", 109664415396l, "1/6/2014 00:22:42", 1, 6, 0.00, 2.50, "ZoomHand")]
    [TestFixture("PokerStars", 109690806574l, "1/6/2014 16:19:47", 5, 6, 1.50, 39.78, "SidePot")]
    [TestFixture("Merge", 533636922070l, "4/17/2012 01:58:48", 8, 9, null, null, "GeneralHand")]
    [TestFixture("IPoker", 5383708755l, "1/6/2014 14:44:37", 6, 3, null, null, "GeneralHand")]
    [TestFixture("OnGame", 5361850810464l, "1/6/2014 00:00:00", 8, 6, null, null, "GeneralHand")]
    [TestFixture("Pacific", 349736402, "1/6/2014 22:40:28", 9, 2, null, null, "GeneralHand")]
    [TestFixture("Entraction", 2645975604, "5/30/2012 13:49:35", 4, 2, null, null, "GeneralHand")]
    [TestFixture("FullTilt", 33728803548, "1/6/2014 10:11:48", 6, 4, 0.00, 0.10, "GeneralHand")]
    [TestFixture("MicroGaming", 5049092010, "9/23/2013 14:27:42",4,6, null, null, "GeneralHand")]
    [TestFixture("Winamax",5281577471, "10/24/2013 03:51:47",2,2, null, null, "GeneralHand")]
    [TestFixture("WinningPoker", 261641541, "3/9/2014 16:53:43", 5, 6, null, null, "GeneralHand")]
    [TestFixture("BossMedia", 2076693331L, "2/18/2014 0:15:38", 5, 5, null, null, "GeneralHand")]
    class HandParserGeneralHandTests : HandHistoryParserBaseTests 
    {
        private readonly long _expectedHandId;
        private readonly int _expectedDealerButtonPosition;
        private readonly int _expectedNumberOfPlayers;
        private readonly string _handFile;
        private readonly decimal? _expectedRake;
        private readonly decimal? _expectedPotSize;
        private readonly DateTime _expectedDateTime;
        private readonly string _handText;

        public HandParserGeneralHandTests(string site, 
                                          long expectedHandId,
                                          string expectedDateOfHand,
                                          int expectedDealerButtonPosition,
                                          int expectedNumberOfPlayers,
                                          double? expectedRake,
                                          double? expectedPotSize,
                                          string handFile)
            : base(site)
        {
            _expectedHandId = expectedHandId;
            _expectedDealerButtonPosition = expectedDealerButtonPosition;
            _expectedNumberOfPlayers = expectedNumberOfPlayers;
            _handFile = handFile;
            if (expectedRake != null) _expectedRake = (decimal)expectedRake;
            if (expectedPotSize != null) _expectedPotSize = (decimal)expectedPotSize;

            try
            {
                _expectedDateTime = DateTime.Parse(expectedDateOfHand, new CultureInfo("en-US"));

                _handText = SampleHandHistoryRepository.GetGeneralHandHistoryText(PokerFormat.CashGame, Site, _handFile);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ParseHandId_Works()
        {            
            Assert.AreEqual(_expectedHandId, GetSummmaryParser().ParseHandId(_handText), "IHandHistorySummaryParser: ParseHandId");
            Assert.AreEqual(_expectedHandId, GetParser().ParseHandId(_handText), "IHandHistoryParser: ParseHandId");
        }

        [Test]
        public void ParseExtraDetails_Works()
        {
            var summary = GetSummmaryParser().ParseFullHandSummary(_handText);

            Assert.AreEqual(_expectedRake, summary.Rake, "Rake");
            Assert.AreEqual(_expectedPotSize, summary.TotalPot, "TotalPot");
        }

        [Test]
        public void ParseDateOfHand_ConvertsToUtcDateTime()
        {
            switch (Site)
            {
                case SiteName.Unknown:
                    Assert.Ignore("Not implemented for site " + Site);
                    break;
            }

            Assert.AreEqual(_expectedDateTime, GetSummmaryParser().ParseDateUtc(_handText), "IHandHistorySummaryParser: ParseDateUtc");
            Assert.AreEqual(_expectedDateTime, GetParser().ParseDateUtc(_handText), "IHandHistoryParser: ParseDateUtc");
        }


        [Test]
        public void ParseDealerButtonPosition_Works()
        {
            switch (Site)
            {
                case SiteName.Unknown:
                    Assert.Ignore("Not implemented for site " + Site);
                    break;
            }

            Assert.AreEqual(_expectedDealerButtonPosition, GetSummmaryParser().ParseDealerPosition(_handText), "IHandHistorySummaryParser: ParseDealerPosition");
            Assert.AreEqual(_expectedDealerButtonPosition, GetParser().ParseDealerPosition(_handText), "IHandHistoryParser: ParseDealerPosition");
        }

        [Test]
        public void ParseNumPlayers_Works()
        {
            switch (Site)
            {
                case SiteName.Unknown:
                    Assert.Ignore("Not implemented for site " + Site);
                    break;
            }

            Assert.AreEqual(_expectedNumberOfPlayers, GetSummmaryParser().ParseNumPlayers(_handText), "IHandHistorySummaryParser: ParseNumPlayers");
            Assert.AreEqual(_expectedNumberOfPlayers, GetParser().ParseNumPlayers(_handText), "IHandHistoryParser: ParseNumPlayers");
        }
    }
}
