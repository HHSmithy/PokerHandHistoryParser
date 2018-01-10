using System;
using System.Globalization;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.GeneralHandTests
{
    [TestFixture("OnGame", 55560000001L, "6/11/2014 18:10:37", 6, 2, 0.0, 400.0, "GeneralHand")]
    [TestFixture("PartyPoker", 15258431574L, "5/12/2016 21:41:57", 5, 6, null, null, "GeneralHand")]
    [TestFixture("Winamax", 657563431500000000L, "04/03/2016 20:21:55", 8, 3, 0.0, 1175.00, "GeneralHand")]
    class HandParserGeneralHandTests_MTT : HandParserGeneralHandTests
    {
        public HandParserGeneralHandTests_MTT(string site,
                                          long expectedHandId,
                                          string expectedDateOfHand,
                                          int expectedDealerButtonPosition,
                                          int expectedNumberOfPlayers,
                                          double? expectedRake,
                                          double? expectedPotSize,
                                          string handFile)
            : base(PokerFormat.MultiTableTournament, site, expectedHandId, expectedDateOfHand, expectedDealerButtonPosition, expectedNumberOfPlayers, expectedRake, expectedPotSize, handFile)
        {
        }
    }
}
