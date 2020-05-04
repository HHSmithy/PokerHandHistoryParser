using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.PokerFormatTests
{
    [TestFixture("PartyPoker", "13550319286", "GeneralHand")]
    [TestFixture("PokerStars", "109681313810", "GeneralHand")]
    [TestFixture("PokerStars", "109664415396", "ZoomHand")]
    [TestFixture("PokerStars", "109690806574", "SidePot")]
    [TestFixture("Merge", "53363692.2070", "GeneralHand")]
    [TestFixture("IPoker", "5383708755", "GeneralHand")]
    [TestFixture("OnGame", "5.361850810.464", "GeneralHand")]
    [TestFixture("Pacific", "349736402", "GeneralHand")]
    [TestFixture("Entraction", "2645975604","GeneralHand")]
    [TestFixture("FullTilt", "33728803548", "GeneralHand")]
    [TestFixture("MicroGaming", "5049092010", "GeneralHand")]
    [TestFixture("Winamax", "5281577.471.1382586707", "GeneralHand")]
    [TestFixture("WinningPoker", "261641541", "GeneralHand")]
    [TestFixture("BossMedia", "2076693331", "GeneralHand")]
    class HandParserPokerFormatTests_CashGame : HandParserPokerFormatTests
    {
        public HandParserPokerFormatTests_CashGame(string site, 
                                          string expectedHandId,
                                          string handFile)
            : base(PokerFormat.CashGame, site, expectedHandId, handFile)
        {
        }
    }
}
