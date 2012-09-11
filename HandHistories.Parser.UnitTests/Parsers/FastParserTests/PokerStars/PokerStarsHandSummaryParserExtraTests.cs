using System;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.PokerStars
{
    [TestFixture]
    class PokerStarsHandSummaryParserExtraTests : HandHistoryParserBaseTests 
    {
        public PokerStarsHandSummaryParserExtraTests() : base("PokerStars")
        {
        }

        private void TestFullHandHistory(HandHistory expectedHand, string fileName)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "ExtraHands", fileName);

            HandHistorySummary actualHand = GetParser().ParseFullHandHistory(handText, true);

            Assert.AreEqual(expectedHand.GameDescription, actualHand.GameDescription);
            Assert.AreEqual(expectedHand.DealerButtonPosition, actualHand.DealerButtonPosition);
            Assert.AreEqual(expectedHand.DateOfHandUtc, actualHand.DateOfHandUtc);
            Assert.AreEqual(expectedHand.HandId, actualHand.HandId);
            Assert.AreEqual(expectedHand.NumPlayersSeated, actualHand.NumPlayersSeated);
            Assert.AreEqual(expectedHand.TableName, actualHand.TableName);
        }

        private void TestFullHandHistorySummary(HandHistorySummary expectedSummary, string fileName)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "ExtraHands", fileName);

            HandHistorySummary actualSummary = GetSummmaryParser().ParseFullHandSummary(handText, true);

            Assert.AreEqual(expectedSummary.GameDescription, actualSummary.GameDescription);
            Assert.AreEqual(expectedSummary.DealerButtonPosition, actualSummary.DealerButtonPosition);
            Assert.AreEqual(expectedSummary.DateOfHandUtc, actualSummary.DateOfHandUtc);
            Assert.AreEqual(expectedSummary.HandId, actualSummary.HandId);
            Assert.AreEqual(expectedSummary.NumPlayersSeated, actualSummary.NumPlayersSeated);
            Assert.AreEqual(expectedSummary.TableName, actualSummary.TableName);
        }

        // Issue with names with colons in
        [Test]
        public void PlayerWithColon()
        {
            HandHistorySummary expectedSummary = new HandHistorySummary()
                                                     {
                                                         GameDescription = new GameDescriptor()
                                                                               {
                                                                                   PokerFormat = PokerFormat.CashGame,
                                                                                   GameType = GameType.NoLimitHoldem,
                                                                                   Limit = Limit.FromSmallBlindBigBlind(0.10m, 0.25m, Currency.USD),
                                                                                   SeatType = SeatType.FromMaxPlayers(9),
                                                                                   Site = SiteName.PokerStars,
                                                                                   TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.Regular)
                                                                               },
                                                         DateOfHandUtc = new DateTime(2012, 7, 18, 16, 25, 8),
                                                         DealerButtonPosition = 9,
                                                         HandId = 83504515230,
                                                         NumPlayersSeated = 9,
                                                         TableName = "Hygiea IV 40-100 bb"
                                                     };

            TestFullHandHistorySummary(expectedSummary, "PlayerWithColon");
        }

        // Issue with date parsing during daylight savings
        [Test]
        public void DateIssue1()
        {
            HandHistorySummary expectedSummary = new HandHistorySummary()
            {
                GameDescription = new GameDescriptor()
                {
                    PokerFormat = PokerFormat.CashGame,
                    GameType = GameType.NoLimitHoldem,
                    Limit = Limit.FromSmallBlindBigBlind(0.50m, 1.00m, Currency.USD),
                    SeatType = SeatType.FromMaxPlayers(6),
                    Site = SiteName.PokerStars,
                    TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.Regular)
                },
                DateOfHandUtc = new DateTime(2011, 5, 7, 3, 51, 38),
                DealerButtonPosition = 4,                
                HandId = 61777648755,
                NumPlayersSeated = 4,
                TableName = "Tezcatlipoca III"
            };

            TestFullHandHistorySummary(expectedSummary, "DateIssue1");
        }

        [Test]
        public void DateIssue2()
        {
            HandHistorySummary expectedSummary = new HandHistorySummary()
            {
                GameDescription = new GameDescriptor()
                {
                    PokerFormat = PokerFormat.CashGame,
                    GameType = GameType.CapNoLimitHoldem,
                    Limit = Limit.FromSmallBlindBigBlind(1.0m, 2.00m, Currency.USD),
                    SeatType = SeatType.FromMaxPlayers(9),
                    Site = SiteName.PokerStars,
                    TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.Regular)
                },
                DateOfHandUtc = new DateTime(2011, 5, 10, 11, 27, 21),
                DealerButtonPosition = 1,
                HandId = 61910233643,
                NumPlayersSeated = 7,
                TableName = "Toutatis III"
            };

            TestFullHandHistorySummary(expectedSummary, "DateIssue2");
        }

        [Test]
        public void TableNameWithDash()
        {
            HandHistorySummary expectedSummary = new HandHistorySummary()
            {
                GameDescription = new GameDescriptor()
                {
                    PokerFormat = PokerFormat.CashGame,
                    GameType = GameType.PotLimitOmaha,
                    Limit = Limit.FromSmallBlindBigBlind(25.0m, 50.00m, Currency.USD),
                    SeatType = SeatType.FromMaxPlayers(2),
                    Site = SiteName.PokerStars,
                    TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.Regular)
                },
                DateOfHandUtc = new DateTime(2011, 5, 19, 00, 41, 04),
                DealerButtonPosition = 1,
                HandId = 62279382715,
                NumPlayersSeated = 2,
                TableName = "Isildur's PLO 50"
            };

            TestFullHandHistorySummary(expectedSummary, "TableNameWithDash");
        }

        [Test]
        public void SidePot()
        {
            HandHistory expectedHand = new HandHistory()
            {
                GameDescription = new GameDescriptor()
                {
                    PokerFormat = PokerFormat.CashGame,
                    GameType = GameType.NoLimitHoldem,
                    Limit = Limit.FromSmallBlindBigBlind(0.05m, 0.10m, Currency.EURO),
                    SeatType = SeatType.FromMaxPlayers(9),
                    Site = SiteName.PokerStars,
                    TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.Regular)
                },
                DateOfHandUtc = new DateTime(2012, 9, 10, 23, 43, 58),
                DealerButtonPosition = 9,
                HandId = 85998509763,
                NumPlayersSeated = 8,
                TableName = "Rarahu IV Fast,40-100 bb"
            };

            TestFullHandHistory(expectedHand, "SidePot");
        }
    }
}
