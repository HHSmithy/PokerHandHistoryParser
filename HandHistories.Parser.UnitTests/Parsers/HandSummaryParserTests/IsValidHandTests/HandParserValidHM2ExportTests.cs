using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.IsValidHandTests
{
    /// <summary>
    /// Holdem manager splits the hands in strange ways, here is tests for this
    /// </summary>
    [TestFixture("IPoker")]
    class HandParserValidHM2ExportTests : HandHistoryParserBaseTests
    {
        int testNumber = 1;

        public HandParserValidHM2ExportTests(string site, int testNumber)
            : base(site)
        {
            this.testNumber = testNumber;
        }

        public HandParserValidHM2ExportTests(string site)
            : base(site)
        {

        }

        [Test]
        public void IsValidHand_HMExport_Works()
        {
            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "ValidHandTests", "HMExport_" + testNumber);

            Assert.AreEqual(true, GetSummmaryParser().IsValidHand(handText), "IHandHistorySummaryParser: IsValidHand");
            Assert.AreEqual(true, GetParser().IsValidHand(handText), "IHandHistoryParser: IsValidHand");
        }
    }
}
