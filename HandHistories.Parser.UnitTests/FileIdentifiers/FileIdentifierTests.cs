using HandHistories.Objects.GameDescription;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.FileIdentifiers;
using HandHistories.Parser.UnitTests.Parsers.Base;

namespace HandHistories.Parser.UnitTests.FileIdentifiers
{
    [TestFixture("PokerStars")]
    [TestFixture("OnGame")]
    [TestFixture("IPoker")]
    [TestFixture("Pacific")]
    [TestFixture("FullTilt")]
    [TestFixture("MicroGaming")]
    [TestFixture("Winamax")]
    [TestFixture("BossMedia")]
    [TestFixture("PartyPoker")]
    class FileIdentifierTests : HandHistoryParserBaseTests
    {
        public FileIdentifierTests(string site):
            base(site)
        {
        }

        [Test]
        public void IdentifySite()
        {
            var handText = SampleHandHistoryRepository.GetGeneralHandHistoryText(PokerFormat.CashGame, Site, "GeneralHand");

            var IdentifiedSite = FileIdentifier.IdentifyHand(handText);

            Assert.AreEqual(Site, IdentifiedSite);
        }

        [TestCase(1)]
        public void IdentifySite2(int number)
        {
            switch (Site)
            {
                case SiteName.IPoker:
                    break;
                default:
                    Assert.Ignore();
                    break;
            }

            var handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "IdTest", "IdTest" + number);

            var IdentifiedSite = FileIdentifier.IdentifyHand(handText);

            Assert.AreEqual(Site, IdentifiedSite);
        }
    }
}
