using System;
using System.Globalization;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.GeneralHandTests
{
    [TestFixture("PartyPoker", 13550319286L, "1/6/2014 09:15:12", 3, 9, null, null, "GeneralHand")]
    [TestFixture("PokerStars", 109681313810L, "1/6/2014 12:21:05", 4, 6, 0.06, 1.27, "GeneralHand")]
    [TestFixture("PokerStars", 109664415396L, "1/6/2014 00:22:42", 1, 6, 0.00, 2.50, "ZoomHand")]
    [TestFixture("PokerStars", 109690806574L, "1/6/2014 16:19:47", 5, 6, 1.50, 39.78, "SidePot")]
    [TestFixture("Merge", 533636922070L, "4/17/2012 01:58:48", 8, 9, null, null, "GeneralHand")]
    [TestFixture("IPoker", 5383708755L, "1/6/2014 14:44:37", 6, 3, null, null, "GeneralHand")]
    [TestFixture("OnGame", 5361850810464L, "1/6/2014 00:00:00", 8, 6, 0.00, 1.00, "GeneralHand")]
    [TestFixture("Pacific", 349736402, "1/6/2014 22:40:28", 9, 2, null, null, "GeneralHand")]
    [TestFixture("Entraction", 2645975604, "5/30/2012 13:49:35", 4, 2, null, null, "GeneralHand")]
    [TestFixture("FullTilt", 33728803548, "1/6/2014 10:11:48", 6, 4, 0.00, 0.10, "GeneralHand")]
    [TestFixture("MicroGaming", 5049092010, "9/23/2013 14:27:42",4,6, null, null, "GeneralHand")]
    [TestFixture("Winamax",5281577471, "10/24/2013 03:51:47", 2, 2, 0.98, 15.00, "GeneralHand")]
    [TestFixture("Winamax", 8270134804, "12/15/2015 04:29:17", 0, 5, 0.00, 3.00, "NoRake")]
    [TestFixture("WinningPoker", 261641541, "3/9/2014 16:53:43", 5, 6, null, null, "GeneralHand")]
    [TestFixture("BossMedia", 2076693331L, "2/18/2014 0:15:38", 5, 5, 15.0, 10125.0, "GeneralHand")]
    class HandParserGeneralHandTests_CashGame : HandParserGeneralHandTests 
    {
        public HandParserGeneralHandTests_CashGame(string site, 
                                          long expectedHandId,
                                          string expectedDateOfHand,
                                          int expectedDealerButtonPosition,
                                          int expectedNumberOfPlayers,
                                          double? expectedRake,
                                          double? expectedPotSize,
                                          string handFile)
            : base(PokerFormat.CashGame, site, expectedHandId, expectedDateOfHand, expectedDealerButtonPosition, expectedNumberOfPlayers, expectedRake, expectedPotSize, handFile)
        {
        }
    }
}
