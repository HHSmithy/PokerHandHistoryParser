using HandHistories.Parser.Parsers.FastParser.PokerStars;
using HandHistories.Writer.Writer.PokerStars;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandHistories.Writer.UnitTests.Writers.PokerStars
{
    [TestFixture]
    public class PokerStarsWriterTests : HandWriterTestBase
    {
        public PokerStarsWriterTests()
            : base(new PokerStarsFastParserImpl(), new PokerStarsHandWriter())
        {
        }

        [TestCase(@"ExtraHands\DateIssue1")]
        [TestCase(@"ExtraHands\DateIssue2")]
        [TestCase(@"HandActionTests\3BetHand")]
        [TestCase(@"HandActionTests\BasicHand")]
        [TestCase(@"HandActionTests\FoldedPreflop")]
        public void TestFile(string path)
        {
            base.TestHand(path);
        }
    }
}
