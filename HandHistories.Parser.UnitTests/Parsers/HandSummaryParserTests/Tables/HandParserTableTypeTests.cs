using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.Tables
{
    [TestFixture("PartyPoker", new string[] { "Regular", "Regular", "Regular", "Regular" })]
    [TestFixture("PokerStars", new string[] { "Regular", "Regular", "Deep", "Regular", "Zoom", "Cap" })]
    [TestFixture("OnGame", new string[] { "Regular", "Regular", "Regular", "Speed" })]
    [TestFixture("IPoker", new string[] { "Regular", "Regular", "Regular", "Shallow" })]
    [TestFixture("Pacific", new string[] { "Regular", "Regular", "Regular", "Regular", "Jackpot" })]
    [TestFixture("Merge", new string[] { "Regular", "Jackpot", "Regular", "Regular" })]
    [TestFixture("Entraction", new string[] { "Regular", "Regular", "Regular", "Regular" })]
    [TestFixture("FullTilt", new string[] { "Regular", "Regular", "Regular", "Regular" })]
    class HandParserTableTypeTests : HandHistoryParserBaseTests
    {
        private readonly string[] _expectedTableTypeStrings;

        public HandParserTableTypeTests(string site, string[] expectedTableTypeStrings)
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
