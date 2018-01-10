using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.IPoker
{
    class IPokerExtraTests_CashGame : IPokerExtraTests
    {
        public IPokerExtraTests_CashGame()
            : base(PokerFormat.CashGame)
        {
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

        [Test]
        public void CommunityCardsIssueTest()
        {
            string handText = SampleHandHistoryRepository.GetHandExample(_format, Site, "ExtraHands", "CommunityCardsIssue");

            var actualCommunityCards = GetParser().ParseCommunityCards(handText);

            var expected = BoardCards.Parse("Jd 7s 2c 2s 9d");

            Assert.AreEqual(expected, actualCommunityCards);
        }
    }
}
