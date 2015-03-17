using HandHistories.Parser.Parsers.FastParser._888;
using HandHistories.Parser.Parsers.FastParser.PokerStars;
using HandHistories.Writer.Writer.Pacific;
using HandHistories.Writer.Writer.PokerStars;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandHistories.Writer.UnitTests.Writers.Pacific
{
    [TestFixture]
    public class PacificWriterTests : HandWriterTestBase
    {
        public PacificWriterTests()
            : base(new Poker888FastParserImpl(), new PacificHandWriter())
        {
        }

        [TestCase(@"HandActionTests\3BetHand")]
        [TestCase(@"HandActionTests\AllInHandWithShowdown")]
        [TestCase(@"HandActionTests\BasicHand")]
        [TestCase(@"HandActionTests\FoldedPreflop")]
        public void TestFile(string path)
        {
            base.TestHand(path);
        }
    }
}
