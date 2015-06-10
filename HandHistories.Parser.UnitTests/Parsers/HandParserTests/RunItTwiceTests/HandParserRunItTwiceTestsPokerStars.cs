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
    class PokerStarsRunItTwiceTests : HandParserRunItTwiceTests
    {
        public PokerStarsRunItTwiceTests()
            : base("PokerStars")
        {
        }

        [Test]
        public void RunItTwiceTest_1()
        {
            var expectedRun1 = new List<HandAction>()
            {
                new HandAction("FLATC@T", HandActionType.SHOW,Street.Showdown),
                new WinningsAction("FLATC@T", HandActionType.WINS_SIDE_POT, 10m, 1),
                new HandAction("KENZA_MILOU", HandActionType.SHOW,Street.Showdown),
                new WinningsAction("FLATC@T", HandActionType.WINS, 1503.50m, 0),
            };

            var expectedRun2 = new List<HandAction>()
            {
                new HandAction("FLATC@T", HandActionType.SHOW,Street.Showdown),
                new WinningsAction("FLATC@T", HandActionType.WINS_SIDE_POT, 10m, 1),
                new HandAction("KENZA_MILOU", HandActionType.SHOW,Street.Showdown),
                new WinningsAction("FLATC@T", HandActionType.WINS, 1503.50m, 0),
            };

            RunItTwiceTest(expectedRun1, expectedRun2, "3d Kd 9h 8h Kc", "RunItTwice1");
        }

        [Test]
        public void RunItTwiceTest_2()
        {
            var expectedRun1 = new List<HandAction>()
            {
                new HandAction("Gamz11", HandActionType.SHOW,Street.Showdown),
                new HandAction("Garnerus", HandActionType.SHOW,Street.Showdown),
                new WinningsAction("Gamz11", HandActionType.WINS, 40.30m, 0),

                new WinningsAction("Garnerus", HandActionType.WINS, 40.30m, 0),
            };

            var expectedRun2 = new List<HandAction>()
            {
                new HandAction("Gamz11", HandActionType.SHOW,Street.Showdown),
                new HandAction("Garnerus", HandActionType.SHOW,Street.Showdown),
                new WinningsAction("Gamz11", HandActionType.WINS, 40.30m, 0),

                new WinningsAction("Garnerus", HandActionType.WINS, 40.30m, 0),
            };

            RunItTwiceTest(expectedRun1, expectedRun2, "6s 8h 6h Td 6c", "RunItTwice2");
        }
    }
}
