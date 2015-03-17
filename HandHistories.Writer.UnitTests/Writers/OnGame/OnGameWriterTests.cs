using HandHistories.Parser.Parsers.FastParser.OnGame;
using HandHistories.Parser.Parsers.FastParser.PokerStars;
using HandHistories.Writer.Writer.PokerStars;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandHistories.Writer.UnitTests.Writers.OnGame
{
    [TestFixture]
    public class OnGameWriterTests : HandWriterTestBase
    {
        public OnGameWriterTests()
            : base(new OnGameFastParserImpl(), new OnGameHandWriter())
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
