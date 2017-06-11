using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.IsValidHandTests
{
    [TestFixture("PartyPoker")]
    [TestFixture("PokerStars")]
    [TestFixture("OnGame")]
    [TestFixture("IPoker")]
    [TestFixture("Pacific")]
    [TestFixture("Merge")]
    [TestFixture("Entraction")]
    [TestFixture("FullTilt")]
    [TestFixture("MicroGaming")]
    [TestFixture("MicroGaming")]
    [TestFixture("Winamax")]
    [TestFixture("WinningPoker")]
    [TestFixture("WinningPoker", 2)]
    internal class HandParserValidHandTests : HandHistoryParserBaseTests
    {
        int testNumber = 1;

        public HandParserValidHandTests(string site, int testNumber)
            : base(site)
        {
            this.testNumber = testNumber;
        }

        public HandParserValidHandTests(string site)
            : base(site)
        {

        }
       
        [TestCase(true)]
        [TestCase(false)]
        public void IsValidHand_Works(bool expected)
        {
            string handText = SampleHandHistoryRepository.GetValidHandHandHistoryText(PokerFormat.CashGame, Site, expected, testNumber);

            Assert.AreEqual(expected, GetSummmaryParser().IsValidHand(handText), "IHandHistorySummaryParser: IsValidHand");
            Assert.AreEqual(expected, GetParser().IsValidHand(handText), "IHandHistoryParser: IsValidHand");
        }

        [Test]
        public void SummaryParser_InvalidHand_RethrowFalse_ReturnsNull()
        {
            string handText = SampleHandHistoryRepository.GetValidHandHandHistoryText(PokerFormat.CashGame, Site, false, testNumber);

            Assert.IsNull(GetSummmaryParser().ParseFullHandSummary(handText, false), "IHandHistorySummaryParser: ParseFullHandSummary");
        }

        [Test]
        public void FullParser_InvalidHand_RethrowFalse_ReturnsNull()
        {
            string handText = SampleHandHistoryRepository.GetValidHandHandHistoryText(PokerFormat.CashGame, Site, false, testNumber);

            Assert.IsNull(GetParser().ParseFullHandHistory(handText, false), "IHandHistorySummaryParser: ParseFullHandSummary");
        }

        [Test]
        public void SummaryParser_InvalidHand_RethrowTrue_ThrowsException()
        {
            string handText = SampleHandHistoryRepository.GetValidHandHandHistoryText(PokerFormat.CashGame, Site, false, testNumber);

            Assert.Throws<InvalidHandException>(() => GetSummmaryParser().ParseFullHandSummary(handText, true), "IHandHistorySummaryParser: ParseFullHandSummary");
        }

        [Test]
        public void FullParser_InvalidHand_RethrowTrue_ThrowsException()
        {
            string handText = SampleHandHistoryRepository.GetValidHandHandHistoryText(PokerFormat.CashGame, Site, false, testNumber);

            Assert.Throws<InvalidHandException>(() => GetParser().ParseFullHandHistory(handText, true), "IHandHistorySummaryParser: ParseFullHandSummary");
        }

        [Test]
        public void IsHandCancelled_Works()
        {
            switch (Site)
            {
                case SiteName.PartyPoker:
                case SiteName.FullTilt:
                case SiteName.IPoker:
                case SiteName.OnGame:
                case SiteName.BossMedia:
                case SiteName.Ladbrokes:
                case SiteName.Pacific:
                case SiteName.Winamax:
                case SiteName.Merge:
                case SiteName.Entraction:
                    Assert.Ignore("Site do not produce cancelled handhistories: " + Site);
                    break;
            }

            string cancelledHandText = SampleHandHistoryRepository.GetCancelledHandHandHistoryText(PokerFormat.CashGame, Site);
            bool isCancelled;
            Assert.IsTrue(GetSummmaryParser().IsValidOrCancelledHand(cancelledHandText, out isCancelled), "IHandHistorySummaryParser: IsCancelledHand");
            Assert.IsTrue(isCancelled, "IHandHistoryParser: IsCancelledHand");
        }

        List<IHandHistoryParser> GetAllParsers()
        {
            var factory = new HandHistories.Parser.Parsers.Factory.HandHistoryParserFactoryImpl();
            return new List<IHandHistoryParser>()
            {
                factory.GetFullHandHistoryParser(SiteName.Entraction),
                factory.GetFullHandHistoryParser(SiteName.FullTilt),
                factory.GetFullHandHistoryParser(SiteName.Pacific),
                factory.GetFullHandHistoryParser(SiteName.PartyPoker),
                factory.GetFullHandHistoryParser(SiteName.PokerStars),
                factory.GetFullHandHistoryParser(SiteName.OnGame),
                factory.GetFullHandHistoryParser(SiteName.Merge),
                factory.GetFullHandHistoryParser(SiteName.MicroGaming),
                factory.GetFullHandHistoryParser(SiteName.IPoker),
                factory.GetFullHandHistoryParser(SiteName.Winamax),
                factory.GetFullHandHistoryParser(SiteName.WinningPoker),
            };
        }

        [Test]
        public void IsValidHand_Unique()
        {
            string handText = SampleHandHistoryRepository.GetValidHandHandHistoryText(PokerFormat.CashGame, Site, true, testNumber);

            var handParser = GetParser();
            Assert.AreEqual(true, handParser.IsValidHand(handText), "IHandHistoryParser: IsValidHand");

            foreach (var otherParser in GetAllParsers()
                .Where(p => p.SiteName != handParser.SiteName))
            {
                try
                {
                    Assert.IsFalse(otherParser.IsValidHand(handText), "IHandHistoryParser: Should be invalid hand");
                }
                catch
                {
                    continue;//When the parser throws that indicates that it is an invalid hand
                }
            }
        }
    }
}
