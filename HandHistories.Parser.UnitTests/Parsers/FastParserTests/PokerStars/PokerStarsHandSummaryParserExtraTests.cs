using System;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
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

        private HandHistory TestFullHandHistory(HandHistory expectedHand, string fileName)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "ExtraHands", fileName);

            HandHistory actualHand = GetParser().ParseFullHandHistory(handText, true);

            Assert.AreEqual(expectedHand.GameDescription, actualHand.GameDescription);
            Assert.AreEqual(expectedHand.DealerButtonPosition, actualHand.DealerButtonPosition);
            Assert.AreEqual(expectedHand.DateOfHandUtc, actualHand.DateOfHandUtc);
            Assert.AreEqual(expectedHand.HandId, actualHand.HandId);
            Assert.AreEqual(expectedHand.NumPlayersSeated, actualHand.NumPlayersSeated);
            Assert.AreEqual(expectedHand.TableName, actualHand.TableName);

            return actualHand;
        }

        private HandHistorySummary TestFullHandHistorySummary(HandHistorySummary expectedSummary, string fileName)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "ExtraHands", fileName);

            HandHistorySummary actualSummary = GetSummmaryParser().ParseFullHandSummary(handText, true);

            Assert.AreEqual(expectedSummary.GameDescription, actualSummary.GameDescription);
            Assert.AreEqual(expectedSummary.DealerButtonPosition, actualSummary.DealerButtonPosition);
            Assert.AreEqual(expectedSummary.DateOfHandUtc, actualSummary.DateOfHandUtc);
            Assert.AreEqual(expectedSummary.HandId, actualSummary.HandId);
            Assert.AreEqual(expectedSummary.NumPlayersSeated, actualSummary.NumPlayersSeated);
            Assert.AreEqual(expectedSummary.TableName, actualSummary.TableName);

            return actualSummary;
        }

        [Test]
        public void ZoomHand()
        {
            HandHistorySummary expectedSummary = new HandHistorySummary()
            {
                GameDescription = new GameDescriptor()
                {
                    PokerFormat = PokerFormat.CashGame,
                    GameType = GameType.PotLimitOmaha,
                    Limit = Limit.FromSmallBlindBigBlind(1m, 2m, Currency.USD),
                    SeatType = SeatType.FromMaxPlayers(6),
                    Site = SiteName.PokerStars,
                    TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.Zoom)
                },
                DateOfHandUtc = new DateTime(2014, 2, 21, 17, 45, 8),
                DealerButtonPosition = 1,
                HandId = 132630000000,
                NumPlayersSeated = 6,
                TableName = "Diotima"
            };

            TestFullHandHistorySummary(expectedSummary, "ZoomHand");
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
                    GameType = GameType.NoLimitHoldem,
                    Limit = Limit.FromSmallBlindBigBlind(1.0m, 2.00m, Currency.USD),
                    SeatType = SeatType.FromMaxPlayers(9),
                    Site = SiteName.PokerStars,
                    TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.Cap)
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
        public void ShowsDownSingleCard()
        {
            HandHistory expectedHand = new HandHistory()
            {
                GameDescription = new GameDescriptor()
                {
                    PokerFormat = PokerFormat.CashGame,
                    GameType = GameType.FixedLimitHoldem,
                    Limit = Limit.FromSmallBlindBigBlind(1m, 2m, Currency.USD),
                    SeatType = SeatType.FromMaxPlayers(6),
                    Site = SiteName.PokerStars,
                    TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.Regular)
                },
                DateOfHandUtc = new DateTime(2012, 9, 11, 4, 15, 11),
                DealerButtonPosition = 1,
                HandId = 86005187865,
                NumPlayersSeated = 6,
                TableName = "Stavropolis III 40-100 bb"
            };

            HandHistory actualHand = TestFullHandHistory(expectedHand, "ShowsDownSingleCard");

            Assert.IsFalse(actualHand.Players["Zaza5573"].hasHoleCards);
        }

        [Test]
        public void MucksHand()
        {
            HandHistory expectedHand = new HandHistory()
            {
                GameDescription = new GameDescriptor()
                {
                    PokerFormat = PokerFormat.CashGame,
                    GameType = GameType.PotLimitOmahaHiLo,
                    Limit = Limit.FromSmallBlindBigBlind(0.05m, 0.10m, Currency.USD),
                    SeatType = SeatType.FromMaxPlayers(6),
                    Site = SiteName.PokerStars,
                    TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.Regular)
                },
                DateOfHandUtc = new DateTime(2012, 9, 11, 7, 51, 48),
                DealerButtonPosition = 4,
                HandId = 86008517721,
                NumPlayersSeated = 6,
                TableName = "Muscida V 40-100 bb"
            };

            HandHistory actualHand = TestFullHandHistory(expectedHand, "MucksHand");
        }

        [Test]
        public void PlayerNameWithSlashesAndSquareBrackets()
        {
            HandHistory expectedHand = new HandHistory()
            {
                GameDescription = new GameDescriptor()
                {
                    PokerFormat = PokerFormat.CashGame,
                    GameType = GameType.NoLimitHoldem,
                    Limit = Limit.FromSmallBlindBigBlind(0.25m, 0.50m, Currency.USD),
                    SeatType = SeatType.FromMaxPlayers(6),
                    Site = SiteName.PokerStars,
                    TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.Cap)
                },
                DateOfHandUtc = new DateTime(2012, 9, 11, 12, 39, 12),
                DealerButtonPosition = 3,
                HandId = 86015904171,
                NumPlayersSeated = 4,
                TableName = "Acamar IV CAP, Fast,20-50 bb"
            };

            var expectedActions = new List<HandAction>()
            {
                new HandAction("vitinja", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop),
                new HandAction("/\\ntiHer[]", HandActionType.BIG_BLIND, 0.50m, Street.Preflop),
                new HandAction("Catharina111", HandActionType.CALL, 0.50m, Street.Preflop),
                new HandAction("Willo2319", HandActionType.CALL, 0.50m, Street.Preflop),
                new HandAction("vitinja", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("/\\ntiHer[]", HandActionType.CHECK, 0, Street.Preflop),
                new HandAction("/\\ntiHer[]", HandActionType.BET, 1, Street.Flop),
                new HandAction("Catharina111", HandActionType.FOLD, 0m, Street.Flop),
                new HandAction("Willo2319", HandActionType.CALL, 1m, Street.Flop),
                new HandAction("/\\ntiHer[]", HandActionType.BET, 1.50m, Street.Turn),
                new HandAction("Willo2319", HandActionType.CALL, 1.50m, Street.Turn),
                new HandAction("/\\ntiHer[]", HandActionType.CHECK, 0m, Street.River),
                new HandAction("Willo2319", HandActionType.CHECK, 0m, Street.River),
                new HandAction("/\\ntiHer[]", HandActionType.SHOW, 0m, Street.Showdown),
                new HandAction("Willo2319", HandActionType.SHOW, 0m, Street.Showdown),                
            };

            var expectedWinners = new List<WinningsAction>()
            {
                new WinningsAction("Willo2319", WinningsActionType.WINS, 6.45m, 0),        
            };

            HandHistory actualHand = TestFullHandHistory(expectedHand, "PlayerNameWithSlashesAndSquareBrackets");

            Assert.AreEqual(expectedActions, actualHand.HandActions);
            Assert.AreEqual(expectedWinners, actualHand.Winners);
        }

        [Test]
        public void PlayerDisconnected()
        {
            var handText = SampleHandHistoryRepository.GetGeneralHandHistoryText(PokerFormat.CashGame, Site, "Disconnect");
            HandHistorySummary hand = GetSummmaryParser().ParseFullHandSummary(handText, true);

            Assert.IsFalse(hand.Cancelled);
        }
    }
}
