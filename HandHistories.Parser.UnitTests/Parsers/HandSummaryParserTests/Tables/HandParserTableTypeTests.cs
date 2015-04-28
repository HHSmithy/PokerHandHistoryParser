using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.Tables
{
    [TestFixture("PartyPoker", "Regular", "Regular", "Regular", "Regular")]
    [TestFixture("PokerStars", "Regular", "Regular", "Deep", "Regular", "Zoom", "Cap")]
    [TestFixture("OnGame", "Regular", "Regular", "Regular", "Speed")]
    [TestFixture("IPoker", "Regular", "Regular", "Regular", "Shallow")]
    [TestFixture("Pacific", "Regular", "Regular", "Regular", "Regular", "Jackpot")]
    [TestFixture("Merge", "Regular", "Jackpot", "Regular", "Regular")]
    [TestFixture("Entraction", "Regular", "Regular", "Regular", "Regular")]
    [TestFixture("FullTilt", "Regular", "Regular", "Regular", "Regular")]
    class HandParserTableTypeTests : HandHistoryParserBaseTests
    {
        private readonly string[] _expectedTableTypeStrings;

        public HandParserTableTypeTests(string site, params string[] expectedTableTypeStrings)
            : base(site)
        {
            _expectedTableTypeStrings = expectedTableTypeStrings;
        }

        private void TestTableType(int tableTestNumber)
        {
            string handText = SampleHandHistoryRepository.GetTableExampleHandHistoryText(PokerFormat.CashGame, Site, tableTestNumber);

            if (handText == null)
            {
                Assert.Ignore("No Table" + tableTestNumber + ".txt found.");
            }

            string expectedTableTypeString = _expectedTableTypeStrings[tableTestNumber - 1];

            Assert.AreEqual(expectedTableTypeString, GetSummmaryParser().ParseTableType(handText).ToString(), "IHandHistorySummaryParser: ParseTableName");
            Assert.AreEqual(expectedTableTypeString, GetParser().ParseTableType(handText).ToString(), "IHandHistoryParser: ParseTableName");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void ParseTableType_Correct(int tableId)
        {            
            TestTableType(tableId);
        }
    }
}
