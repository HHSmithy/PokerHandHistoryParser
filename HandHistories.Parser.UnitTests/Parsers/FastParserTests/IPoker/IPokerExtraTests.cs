using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.IPoker
{
    [TestFixture]
    abstract class IPokerExtraTests : HandHistoryParserBaseTests 
    {
        protected readonly PokerFormat _format;
        protected IPokerExtraTests(PokerFormat format) : base("IPoker")
        {
            _format = format;
        }

        protected HandHistory TestFullHandHistory(HandHistory expectedHand, string fileName)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(_format, Site, "ExtraHands", fileName);

            HandHistory actualHand = GetParser().ParseFullHandHistory(handText, true);

            Assert.AreEqual(expectedHand.GameDescription, actualHand.GameDescription);
            Assert.AreEqual(expectedHand.DealerButtonPosition, actualHand.DealerButtonPosition);
            Assert.AreEqual(expectedHand.DateOfHandUtc, actualHand.DateOfHandUtc);
            Assert.AreEqual(expectedHand.HandId, actualHand.HandId);
            Assert.AreEqual(expectedHand.NumPlayersSeated, actualHand.NumPlayersSeated);
            Assert.AreEqual(expectedHand.TableName, actualHand.TableName);

            return actualHand;
        }

        protected HandHistorySummary TestFullHandHistorySummary(HandHistorySummary expectedSummary, string fileName)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(_format, Site, "ExtraHands", fileName);

            HandHistorySummary actualSummary = GetSummmaryParser().ParseFullHandSummary(handText, true);

            Assert.AreEqual(expectedSummary.GameDescription, actualSummary.GameDescription);
            Assert.AreEqual(expectedSummary.DealerButtonPosition, actualSummary.DealerButtonPosition);
            Assert.AreEqual(expectedSummary.DateOfHandUtc, actualSummary.DateOfHandUtc);
            Assert.AreEqual(expectedSummary.HandId, actualSummary.HandId);
            Assert.AreEqual(expectedSummary.NumPlayersSeated, actualSummary.NumPlayersSeated);
            Assert.AreEqual(expectedSummary.TableName, actualSummary.TableName);

            return actualSummary;
        }
    }
}
