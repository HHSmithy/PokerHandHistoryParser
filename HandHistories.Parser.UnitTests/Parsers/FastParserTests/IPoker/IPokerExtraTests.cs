using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.IPoker
{
    [TestFixture]
    class IPokerExtraTests : HandHistoryParserBaseTests 
    {
        public IPokerExtraTests() : base("IPoker")
        {
        }

        private HandHistory TestFullHandHistory(HandHistory expectedHand, string fileName)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "ExtraHands", fileName);

            HandHistory actualHand = GetParser().ParseFullHandHistory(handText, true);

            ClassicAssert.AreEqual(expectedHand.GameDescription, actualHand.GameDescription);
            ClassicAssert.AreEqual(expectedHand.DealerButtonPosition, actualHand.DealerButtonPosition);
            ClassicAssert.AreEqual(expectedHand.DateOfHandUtc, actualHand.DateOfHandUtc);
            ClassicAssert.AreEqual(expectedHand.HandId, actualHand.HandId);
            ClassicAssert.AreEqual(expectedHand.NumPlayersSeated, actualHand.NumPlayersSeated);
            ClassicAssert.AreEqual(expectedHand.TableName, actualHand.TableName);

            return actualHand;
        }

        private HandHistorySummary TestFullHandHistorySummary(HandHistorySummary expectedSummary, string fileName)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "ExtraHands", fileName);

            HandHistorySummary actualSummary = GetSummmaryParser().ParseFullHandSummary(handText, true);

            ClassicAssert.AreEqual(expectedSummary.GameDescription, actualSummary.GameDescription);
            ClassicAssert.AreEqual(expectedSummary.DealerButtonPosition, actualSummary.DealerButtonPosition);
            ClassicAssert.AreEqual(expectedSummary.DateOfHandUtc, actualSummary.DateOfHandUtc);
            ClassicAssert.AreEqual(expectedSummary.HandId, actualSummary.HandId);
            ClassicAssert.AreEqual(expectedSummary.NumPlayersSeated, actualSummary.NumPlayersSeated);
            ClassicAssert.AreEqual(expectedSummary.TableName, actualSummary.TableName);

            return actualSummary;
        }

        [Test]
        public void HandIssue1()
        {
            HandHistory expectedSummary = new HandHistory()
            {
                GameDescription = new GameDescriptor()
                {
                    PokerFormat = PokerFormat.CashGame,
                    GameType = GameType.NoLimitHoldem,
                    Limit = Limit.FromSmallBlindBigBlind(5m, 10m, Currency.USD),
                    SeatType = SeatType.FromMaxPlayers(6),
                    Site = SiteName.IPoker,
                    TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.Regular)
                },
                DateOfHandUtc = new DateTime(2014, 1, 1, 22, 50, 13),
                DealerButtonPosition = 1,
                HandId = 987654,
                NumPlayersSeated = 5,
                TableName = "Andreapol__No_DP_"
            };
            TestFullHandHistorySummary(expectedSummary, "DataMinerHandIssue1");
        }
    }
}
