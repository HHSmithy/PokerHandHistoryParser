using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.PokerFormatTests
{
    //[TestFixture("PartyPoker", 13550319286L, "GeneralHand")]
    //[TestFixture("PokerStars", 109681313810L, "GeneralHand")]
    //[TestFixture("PokerStars", 109664415396L, "ZoomHand")]
    //[TestFixture("PokerStars", 109690806574L, "SidePot")]
    //[TestFixture("Merge", 533636922070L, "GeneralHand")]
    [TestFixture("IPoker", 635000000L, "GeneralHand")]
    //[TestFixture("OnGame", 5361850810464L, "GeneralHand")]
    //[TestFixture("Pacific", 349736402, "GeneralHand")]
    //[TestFixture("Entraction", 2645975604, "GeneralHand")]
    //[TestFixture("FullTilt", 33728803548, "GeneralHand")]
    //[TestFixture("MicroGaming", 5049092010, "GeneralHand")]
    //[TestFixture("Winamax", 5281577471, "GeneralHand")]
    //[TestFixture("WinningPoker", 261641541, "GeneralHand")]
    [TestFixture("BossMedia", 123456123L, "GeneralHand")]
    class HandParserPokerFormatTests_SnG : HandParserPokerFormatTests
    {
        public HandParserPokerFormatTests_SnG(string site,
                                          long expectedHandId,
                                          string handFile)
            : base(PokerFormat.SitAndGo, site, expectedHandId, handFile)
        {
        }
    }
}
