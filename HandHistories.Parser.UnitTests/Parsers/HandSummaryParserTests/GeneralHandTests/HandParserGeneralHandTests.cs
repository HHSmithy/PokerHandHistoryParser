using System;
using System.Globalization;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Utils;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.GeneralHandTests
{
    abstract class HandParserGeneralHandTests : HandHistoryParserBaseTests 
    {
        private readonly PokerFormat _format;
        private readonly long _expectedHandId;
        private readonly int _expectedDealerButtonPosition;
        private readonly int _expectedNumberOfPlayers;
        private readonly string _handFile;
        private readonly decimal? _expectedRake;
        private readonly decimal? _expectedPotSize;
        private readonly DateTime _expectedDateTime;
        private readonly string _handText;

        protected HandParserGeneralHandTests(PokerFormat format,
                                          string site, 
                                          long expectedHandId,
                                          string expectedDateOfHand,
                                          int expectedDealerButtonPosition,
                                          int expectedNumberOfPlayers,
                                          double? expectedRake,
                                          double? expectedPotSize,
                                          string handFile)
            : base(site)
        {
            _format = format;
            _expectedHandId = expectedHandId;
            _expectedDealerButtonPosition = expectedDealerButtonPosition;
            _expectedNumberOfPlayers = expectedNumberOfPlayers;
            _handFile = handFile;
            if (expectedRake != null) _expectedRake = (decimal)expectedRake;
            if (expectedPotSize != null) _expectedPotSize = (decimal)expectedPotSize;

            try
            {
                _expectedDateTime = DateTime.Parse(expectedDateOfHand, new CultureInfo("en-US"));

                _handText = SampleHandHistoryRepository.GetGeneralHandHistoryText(format, Site, _handFile);
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

        [Test]
        public void ParsePokerFormat_Works()
        {
            Assert.AreEqual(_format, GetSummmaryParser().ParseFullHandSummary(_handText).GameDescription.PokerFormat, "IHandHistorySummaryParser: PokerFormat");
            Assert.AreEqual(_format, GetParser().ParseFullHandHistory(_handText).GameDescription.PokerFormat, "IHandHistoryParser: PokerFormat");
        }

        [Test]
        public void HandIntegrity_Works()
        {
            HandHistory hand = GetParser().ParseFullHandHistory(_handText);
            HandIntegrity.Assert(hand);
        }
    }
}
