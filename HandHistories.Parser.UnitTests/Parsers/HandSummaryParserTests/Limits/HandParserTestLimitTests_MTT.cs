using HandHistories.Objects.GameDescription;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.Limits
{
    [TestFixture("PokerStars", "$40-$80")]
    class HandParserLimitTests_MTT : HandParserLimitTests
    {
        public HandParserLimitTests_MTT(string site, params string[] expectedLimits)
            : base(PokerFormat.MultiTableTournament, site, expectedLimits)
        {
        }
    }
}
