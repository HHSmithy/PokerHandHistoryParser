using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.RunItTwiceTests
{
    [TestFixture]
    class FullTiltRunItTwiceTests : HandParserRunItTwiceTests
    {
        public FullTiltRunItTwiceTests()
            : base("FullTilt")
        {
        }

        [Test]
        public void RunItTwiceTest_1()
        {
            var expectedRun1 = new List<HandAction>()
            {
                new HandAction("1mperial", HandActionType.SHOW, Street.Showdown),
                new HandAction("Darkking_pt", HandActionType.SHOW, Street.Showdown),
            };

            var expectedWinners1 = new List<WinningsAction>()
            {
                new WinningsAction("Darkking_pt", WinningsActionType.WINS, 311m, 0),
            };

            var expectedRun2 = new List<HandAction>()
            {
                new HandAction("1mperial", HandActionType.SHOW, Street.Showdown),
                new HandAction("Darkking_pt", HandActionType.SHOW, Street.Showdown),
            };

            var expectedWinners2 = new List<WinningsAction>()
            {
                new WinningsAction("1mperial", WinningsActionType.WINS, 311m, 0),
            };

            RunItTwiceTest(expectedRun1, expectedRun2, expectedWinners1, expectedWinners2, "Td 3d 3h Qh Ac", "Td 3d 3h Qh 2h", "RunItTwice1");
        }
    }
}
