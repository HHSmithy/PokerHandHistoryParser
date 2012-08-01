using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.Tables
{
    [TestFixture("PokerStars", "Podarkes III", "Ancha V", "Spica VI", "Centaurus VIII")]
    [TestFixture("PartyPoker", "Heads Up #2412772 (No DP)", "Table  194981 (No DP)", "Speed #2356320 (No DP)", "Jackpot #2409628 (No DP)")]
    [TestFixture("OnGame", "[SPEED] Austin [243945479]", "San Marcos [244090560]", "Aleppo [244068606]", "Nijmegen [244308222]")]
    [TestFixture("IPoker", "(Shallow) Vaal (No DP Heads Up), 165636221", "Ambia, 98575671", "Anglicus, 687196931", "Tilsonburg, 51071661")]
    [TestFixture("Pacific", "Oviedo (Real Money)", "Barreiras (Real Money)", "Asmara (Real Money)", "Apatzingan (Real Money)")]
    [TestFixture("Merge", "Baja (56067014)", "Bad Beat - Rue St Catherine (56116487)", "Deal It Twice - Mississippi (56196868)", "Ming Tombs (56176485)")]
    [TestFixture("Entraction", "Zaragoza", "Zaragoza", "Saravane", "Waco")]
    class HandParserTableNameTests : HandHistoryParserBaseTests
    {
        private readonly string[] _expectedTables;

        public HandParserTableNameTests(string site, params string [] expectedTables)
            : base(site)
        {
            _expectedTables = expectedTables;            
        }
        
        private void TestTableName(int tableTestNumber)
        {
            string handText = SampleHandHistoryRepository.GetTableExampleHandHistoryText(PokerFormat.CashGame, Site, tableTestNumber);

            string expectedTableName = _expectedTables[tableTestNumber - 1];

            Assert.AreEqual(expectedTableName, GetSummmaryParser().ParseTableName(handText), "IHandHistorySummaryParser: ParseTableName");
            Assert.AreEqual(expectedTableName, GetParser().ParseTableName(handText), "IHandHistoryParser: ParseTableName");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void ParseTableName_Correct(int tableId)
        {
            TestTableName(tableId);
        }
    }
}
