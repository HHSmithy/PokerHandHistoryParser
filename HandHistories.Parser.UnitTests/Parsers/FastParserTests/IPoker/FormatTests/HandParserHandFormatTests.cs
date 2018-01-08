using System;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.FormatTests
{
    [TestFixture("IPoker", "FormatVersion17", 7558166000, 12, 1)]
    [TestFixture("IPoker", "UnformattedXmlHand", 3305969126, 8, 1)]
    class HandParserHandFormatTests : HandHistoryParserBaseTests 
    {
        private readonly string _unformattedXmlHand;
        private readonly long _expectedHandId;
        private readonly int _expectedNumActions;
        private readonly int _expectedNumWinners;

        public HandParserHandFormatTests(string site, string handname, long handId, int expectedNumActions, int expectedWinners)
            : base(site)
        {
            _expectedHandId = handId;
            _expectedNumActions = expectedNumActions;
            _expectedNumWinners = expectedWinners;

            try
            {
                _unformattedXmlHand = SampleHandHistoryRepository.GetFormatHandHistoryText(PokerFormat.CashGame, Site, handname);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void HandleUnformattedHand_Works()
        {
            Assert.IsTrue(GetSummmaryParser().IsValidHand(_unformattedXmlHand));

            HandHistorySummary summary = GetSummmaryParser().ParseFullHandSummary(_unformattedXmlHand);
            HandHistory fullHandParse = GetParser().ParseFullHandHistory(_unformattedXmlHand);
            
            Assert.AreEqual(_expectedHandId, summary.HandId, "IHandHistorySummaryParser: ParseHandId");
            Assert.AreEqual(_expectedHandId, fullHandParse.HandId, "IHandHistoryParser: ParseHandId");
            Assert.AreEqual(_expectedNumActions, fullHandParse.HandActions.Count, "IHandHistoryParser: HandActionCount");
            Assert.AreEqual(_expectedNumWinners, fullHandParse.Winners.Count, "IHandHistoryParser: WinnerCount");

        }

    }
}
