using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System.Globalization;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.TournamentBuyinTests
{
    abstract class TournamentBuyinTests : HandHistoryParserBaseTests
    {
        private readonly string[] _expectedBuyins;
        readonly PokerFormat format;

        public TournamentBuyinTests(PokerFormat format, string site, params string[] expectedBuyins)
            : base(site)
        {
            _expectedBuyins = expectedBuyins;
            this.format = format;
        }

        private void TestLimit(int limitTestId, string fileName)
        {
            if (_expectedBuyins.Length < limitTestId)
            {
                Assert.Ignore("No matching sample hand for Limit test " + fileName);
            }

            string expectedLimitString = _expectedBuyins[limitTestId - 1];

            TestLimit(expectedLimitString, fileName);
        }

        protected void TestLimit(string expectedLimitString, string fileName)
        {
            string handText = SampleHandHistoryRepository.GetBuyinExampleHandHistoryText(format, Site, fileName);
            
            Assert.AreEqual(expectedLimitString.Replace("e", "€").Replace("A", "Ante-"), GetSummmaryParser().ParseGameDescriptor(handText).Buyin.ToString(CultureInfo.InvariantCulture), "IHandHistorySummaryParser: ParseBuyin");
            Assert.AreEqual(expectedLimitString.Replace("e", "€").Replace("A", "Ante-"), GetParser().ParseGameDescriptor(handText).Buyin.ToString(CultureInfo.InvariantCulture), "IHandHistoryParser: ParseBuyin");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void ParseLimit_Correct(int limitTestId)
        {
            TestLimit(limitTestId, "Buyin" + limitTestId);
        }
    }
}
