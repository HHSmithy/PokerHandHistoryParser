using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.PokerFormatTests
{
    abstract class HandParserPokerFormatTests : HandHistoryParserBaseTests 
    {
        private readonly PokerFormat _format;
        private readonly long _expectedHandId;
        private readonly string _handFile;
        private readonly string _handText;

        public HandParserPokerFormatTests(PokerFormat format,
                                          string site, 
                                          long expectedHandId,
                                          string handFile)
            : base(site)
        {
            _format = format;
            _expectedHandId = expectedHandId;
            _handFile = handFile;

            try
            {
                _handText = SampleHandHistoryRepository.GetGeneralHandHistoryText(_format, Site, _handFile);
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
        public void ParsePokerFormat_Works()
        {
            Assert.AreEqual(_format, GetSummmaryParser().ParseFullHandSummary(_handText).GameDescription.PokerFormat, "IHandHistorySummaryParser: PokerFormat");
            Assert.AreEqual(_format, GetParser().ParseFullHandHistory(_handText).GameDescription.PokerFormat, "IHandHistoryParser: PokerFormat");
        }
    }
}
