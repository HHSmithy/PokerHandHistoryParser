using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.IsValidHandTests
{
    [TestFixture("PartyPoker")]
    [TestFixture("PokerStars")]
    [TestFixture("OnGame")]
    [TestFixture("IPoker")]
    [TestFixture("Pacific")]
    [TestFixture("Merge")]
    [TestFixture("Entraction")]
    internal class HandParserValidHandTests : HandHistoryParserBaseTests
    {
        public HandParserValidHandTests(string site)
            : base(site)
        {

        }
       
        [TestCase(true)]
        [TestCase(false)]
        public void IsValidHand_Works(bool expected)
        {
             string handText = SampleHandHistoryRepository.GetValidHandHandHistoryText(PokerFormat.CashGame, Site, expected);

            Assert.AreEqual(expected, GetSummmaryParser().IsValidHand(handText), "IHandHistorySummaryParser: IsValidHand");
            Assert.AreEqual(expected, GetParser().IsValidHand(handText), "IHandHistoryParser: IsValidHand");
        }

        [Test]
        public void SummaryParser_InvalidHand_RethrowFalse_ReturnsNull()
        {
            string handText = SampleHandHistoryRepository.GetValidHandHandHistoryText(PokerFormat.CashGame, Site, false);

            Assert.IsNull(GetSummmaryParser().ParseFullHandSummary(handText, false), "IHandHistorySummaryParser: ParseFullHandSummary");
        }

        [Test]
        public void FullParser_InvalidHand_RethrowFalse_ReturnsNull()
        {
            string handText = SampleHandHistoryRepository.GetValidHandHandHistoryText(PokerFormat.CashGame, Site, false);

            Assert.IsNull(GetParser().ParseFullHandHistory(handText, false), "IHandHistorySummaryParser: ParseFullHandSummary");
        }

        [Test]
        public void SummaryParser_InvalidHand_RethrowTrue_ThrowsException()
        {
            string handText = SampleHandHistoryRepository.GetValidHandHandHistoryText(PokerFormat.CashGame, Site, false);

            Assert.Throws<InvalidHandException>(() => GetSummmaryParser().ParseFullHandSummary(handText, true), "IHandHistorySummaryParser: ParseFullHandSummary");
        }

        [Test]
        public void FullParser_InvalidHand_RethrowTrue_ThrowsException()
        {
            string handText = SampleHandHistoryRepository.GetValidHandHandHistoryText(PokerFormat.CashGame, Site, false);

            Assert.Throws<InvalidHandException>(() => GetParser().ParseFullHandHistory(handText, true), "IHandHistorySummaryParser: ParseFullHandSummary");
        }
    }
}
