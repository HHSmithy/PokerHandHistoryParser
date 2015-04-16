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

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.PokerStars
{
    [TestFixture]
    class PokerStarsRunItTwiceTests : HandHistoryParserBaseTests 
    {
        public PokerStarsRunItTwiceTests()
            : base("PokerStars")
        {
        }

        void RunItTwiceTest(RunItTwice expected, string name)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "RunItTwiceTests", name);

            HandHistory actualHand = GetParser().ParseFullHandHistory(handText, true);

            Assert.AreEqual(expected.Board, actualHand.RunItTwiceData.Board);
            Assert.AreEqual(expected.Actions, actualHand.RunItTwiceData.Actions);
        }

        [Test]
        public void RunItTwiceTest_1()
        {
            RunItTwice RIT = new RunItTwice();
            RIT.Board = BoardCards.FromCards("3d Kd 9h 8h Kc");
            RIT.Actions = new List<HandAction>()
            {
                new HandAction("FLATC@T", HandActionType.SHOW,Street.Showdown),
                new WinningsAction("FLATC@T", HandActionType.WINS_SIDE_POT, 10m, 1),
                new HandAction("KENZA_MILOU", HandActionType.SHOW,Street.Showdown),
                new WinningsAction("FLATC@T", HandActionType.WINS, 1503.50m, 0),
            };

            RunItTwiceTest(RIT, "RunItTwice1");
        }
    }
}
