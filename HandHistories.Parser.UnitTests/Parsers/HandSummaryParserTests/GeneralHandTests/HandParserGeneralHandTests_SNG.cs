using System;
using System.Globalization;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.GeneralHandTests
{
    [TestFixture("Winamax", 619672044606324737, "01/07/2016 19:00:58", 1, 6, 0.0, 600.00, "GeneralHand")]
    class HandParserGeneralHandTests_SNG : HandParserGeneralHandTests 
    {
        public HandParserGeneralHandTests_SNG(string site, 
                                          long expectedHandId,
                                          string expectedDateOfHand,
                                          int expectedDealerButtonPosition,
                                          int expectedNumberOfPlayers,
                                          double? expectedRake,
                                          double? expectedPotSize,
                                          string handFile)
            : base(PokerFormat.SitAndGo, site, expectedHandId, expectedDateOfHand, expectedDealerButtonPosition, expectedNumberOfPlayers, expectedRake, expectedPotSize, handFile)
        {
        }
    }
}
