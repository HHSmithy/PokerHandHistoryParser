using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System.Globalization;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.Limits
{
    abstract class HandParserLimitTests : HandHistoryParserBaseTests
    {
        private readonly string[] _expectedLimits;
        readonly PokerFormat format;

        public HandParserLimitTests(PokerFormat format, string site, params string[] expectedLimits)
            : base(site)
        {
            _expectedLimits = expectedLimits;
            this.format = format;
        }

        private void TestLimit(int limitTestId, string fileName)
        {
            if (_expectedLimits.Length < limitTestId)
            {
                Assert.Ignore("No matching sample hand for Limit test " + fileName);
            }

            string expectedLimitString = _expectedLimits[limitTestId - 1];

            TestLimit(expectedLimitString, fileName);
        }

        protected void TestLimit(string expectedLimitString, string fileName)
        {
            string handText = SampleHandHistoryRepository.GetLimitExampleHandHistoryText(format, Site, fileName);
            
            Assert.AreEqual(expectedLimitString.Replace("e", "€").Replace("A", "Ante-"), GetSummmaryParser().ParseLimit(handText).ToString(CultureInfo.InvariantCulture), "IHandHistorySummaryParser: ParseLimit");
            Assert.AreEqual(expectedLimitString.Replace("e", "€").Replace("A", "Ante-"), GetParser().ParseLimit(handText).ToString(CultureInfo.InvariantCulture), "IHandHistoryParser: ParseLimit");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void ParseLimit_Correct(int limitTestId)
        {
            TestLimit(limitTestId, "Limit" + limitTestId);
        }
    }
}
