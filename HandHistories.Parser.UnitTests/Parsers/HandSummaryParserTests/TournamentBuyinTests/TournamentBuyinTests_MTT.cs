using HandHistories.Objects.GameDescription;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.HandSummaryParserTests.TournamentBuyinTests
{
    [TestFixture("PokerStars", new string[] { "S0-S0" })]
    class TournamentBuyinTests_MTT : TournamentBuyinTests
    {
        public TournamentBuyinTests_MTT(string site, params string[] expectedLimits)
            : base(PokerFormat.MultiTableTournament, site, expectedLimits)
        {
        }
    }
}
