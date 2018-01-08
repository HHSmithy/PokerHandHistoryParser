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
                new HandAction("Player3", HandActionType.SHOW,Street.Showdown),
                new HandAction("Player4", HandActionType.SHOW,Street.Showdown),
            };

            var expectedwinnersRun1 = new List<WinningsAction>()
            {
                new WinningsAction("Player3", WinningsActionType.WINS_SIDE_POT, 10m, 1),
                new WinningsAction("Player3", WinningsActionType.WINS, 1503.50m, 0),
            };

            var expectedRun2 = new List<HandAction>()
            {
                new HandAction("Player3", HandActionType.SHOW,Street.Showdown),
                new HandAction("Player4", HandActionType.SHOW,Street.Showdown),
            };

            var expectedwinnersRun2 = new List<WinningsAction>()
            {
                 new WinningsAction("Player3", WinningsActionType.WINS_SIDE_POT, 10m, 1),
                 new WinningsAction("Player3", WinningsActionType.WINS, 1503.50m, 0),
            };

            RunItTwiceTest(expectedRun1, expectedRun2, expectedwinnersRun1, expectedwinnersRun2, "3d Kd 9h 8h 4s", "3d Kd 9h 8h Kc", "RunItTwice1");
        }

        [Test]
        public void RunItTwiceTest_2()
        {
            var expectedRun1 = new List<HandAction>()
            {
                new HandAction("Gamz11", HandActionType.SHOW,Street.Showdown),
                new HandAction("Garnerus", HandActionType.SHOW,Street.Showdown),
            };

            var expectedwinnersRun1 = new List<WinningsAction>()
            {
                new WinningsAction("Gamz11", WinningsActionType.WINS, 40.30m, 0),
                new WinningsAction("Garnerus", WinningsActionType.WINS, 40.30m, 0),
            };

            var expectedRun2 = new List<HandAction>()
            {
                new HandAction("Gamz11", HandActionType.SHOW,Street.Showdown),
                new HandAction("Garnerus", HandActionType.SHOW,Street.Showdown),
               
            };

            var expectedwinnersRun2 = new List<WinningsAction>()
            {
                new WinningsAction("Gamz11", WinningsActionType.WINS, 40.30m, 0),
                new WinningsAction("Garnerus", WinningsActionType.WINS, 40.30m, 0),
            };

            RunItTwiceTest(expectedRun1, expectedRun2, expectedwinnersRun1, expectedwinnersRun2, "Qd Qs 4h Ts 7s", "6s 8h 6h Td 6c", "RunItTwice2");
        }
    }
}
