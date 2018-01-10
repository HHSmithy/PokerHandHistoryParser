using System;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using HandHistories.Parser.Parsers.FastParser._888;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.Pacific
{
    using Parser = Poker888FastParserImpl;
    [TestFixture]
    class PacificFastParseExtraTests : HandHistoryParserBaseTests 
    {
        public PacificFastParseExtraTests()
            : base("Pacific")
        {
        }

        private HandHistory TestFullHandHistory(HandHistory expectedHand, string fileName)
        {
            HandHistory actualHand = GetExtraHand(fileName);

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

        private HandHistory GetExtraHand(string fileName)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "ExtraHands", fileName);

            HandHistory actualHand = GetParser().ParseFullHandHistory(handText, true);
            return actualHand;
        }

        [Test]
        public void SpecialCards()
        {
            var expectedBoard = BoardCards.FromCards("4dKs2hTcKd");

            var actualBoard = GetExtraHand("SpecialCards").ComumnityCards;

            Assert.AreEqual(expectedBoard, actualBoard);
        }
    }
}
