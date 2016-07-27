using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.GameTypes
{    
    abstract class HandParserGameTypeTests : HandHistoryParserBaseTests 
    {
        PokerFormat format;

        public HandParserGameTypeTests(PokerFormat format, string site) : base(site)
        {
            this.format = format;
        }
        
        protected void TestGameType(GameType expected)
        {
            string handText = SampleHandHistoryRepository.GetGameTypeHandHistoryText(format, Site, expected);

            Assert.AreEqual(expected, GetSummmaryParser().ParseGameType(handText), "IHandHistorySummaryParser: ParseGameType");
            Assert.AreEqual(expected, GetParser().ParseGameType(handText), "IHandHistoryParser: ParseGameType");
        }
    }
}
