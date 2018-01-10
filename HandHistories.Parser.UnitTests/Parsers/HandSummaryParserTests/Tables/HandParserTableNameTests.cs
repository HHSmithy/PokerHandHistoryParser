using System;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.Tables
{
    [TestFixture("PokerStars", "Sisyphus III 40-100 bb", "Alterf II 40-100 bb", "Alathfar IV 100-250 bb, Ante", "Kythera III 40-100 bb", "Klinkenberg Zoom 40-100 bb", "Antiphos IV CAP,20-50 bb")]
    [TestFixture("PartyPoker", "Avatele", "Barnsley", "Caracas", "Karlsruhe")]
    [TestFixture("OnGame", "Butte [362037295]", "Hallein [361993106]", "Kirkuk [361882901]", "[SPEED] Homs [361726027]")]
    [TestFixture("IPoker", "Abengibre, 817034516", "Akcakent, 817034504", "Canton, 754721371", "(Shallow) Haalderen, 755803691")]
    [TestFixture("Pacific", "Aberdeen", "Bottrop", "Carson", "Santarem", "Linz")]
    [TestFixture("Merge", "Baja (56067014)", "Bad Beat - Rue St Catherine (56116487)", "Deal It Twice - Mississippi (56196868)", "Ming Tombs (56176485)")]
    [TestFixture("Entraction", "Zaragoza", "Zaragoza", "Saravane", "Waco")]
    [TestFixture("FullTilt", "Lynn", "Dega", "Oveja", "Link")]
    [TestFixture("MicroGaming", "Turbo: Hijack 41 - €100 Max", "Turbo: Shuffle 111 - €4 Max", "Turbo: Micro NLHE 3 - €2 Max", "Cut-Off 17 - €200 Max")]
    [TestFixture("Winamax", "Istanbul", "Dublin", "Vienna 36","San Antonio")]
    [TestFixture("WinningPoker", "Braunite", "Baryte   (JP) - 2", "Baotite", "Oalitite   (JP)", "Wichita Falls 1/2 - 3", "Wichita Falls 1/2")]
    [TestFixture("BossMedia", "Diana 9", "Eva 9", "Moa 11")]
    [TestFixture("IGT", "Ada73", "Gwen 263")]
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

            if (handText == null)
            {
                Assert.Ignore("No Table" + tableTestNumber + ".txt found.");
            }

            string expectedTableName = _expectedTables[tableTestNumber - 1];

            Assert.AreEqual(expectedTableName, GetSummmaryParser().ParseTableName(handText), "IHandHistorySummaryParser: ParseTableName");
            Assert.AreEqual(expectedTableName, GetParser().ParseTableName(handText), "IHandHistoryParser: ParseTableName");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void ParseTableName_Correct(int tableId)
        {
            TestTableName(tableId);
        }
    }
}
