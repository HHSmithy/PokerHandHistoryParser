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

        [TestCase(@"HandActionTests\3BetHand")]
        [TestCase(@"HandActionTests\AllInHandWithShowdown")]
        [TestCase(@"HandActionTests\BasicHand")]
        [TestCase(@"HandActionTests\BigBlindOptionRaisesOption")]
        [TestCase(@"HandActionTests\FoldedPreflop")]
        [TestCase(@"HandActionTests\StrangePlayerNames")]
        public void TestFile(string path)
        {
            base.TestHand(path);
        }
    }
}
