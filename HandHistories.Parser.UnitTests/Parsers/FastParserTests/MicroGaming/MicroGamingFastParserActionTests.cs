using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.FastParser.MicroGaming;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.Winamax
{
    using Parser = MicroGamingFastParserImpl;
    [TestFixture]
    class MicroGamingFastParserActionTests : HandHistoryParserBaseTests
    {
        public MicroGamingFastParserActionTests()
            : base("MicroGaming")
        {
        }

        protected MicroGamingFastParserImpl GetFastParser()
        {
            return new MicroGamingFastParserImpl();
        }

        PlayerList Playerlist1 = new PlayerList()
            {
                new Player("TestPlayer1", 100, 1),
                new Player("TestPlayer2", 100, 2),
                new Player("TestPlayer3", 100, 3),
                new Player("TestPlayer4", 100, 4),
                new Player("TestPlayer5", 100, 5),
                new Player("TestPlayer6", 100, 6),
            };

        [Test]
        public void ParseRegularActionLine_UncalledBet_Works()
        {
            List<HandAction> actions = new List<HandAction>();
            HandAction handAction = Parser.ParseActionFromActionLine("<Action seq=\"11\" type=\"MoneyReturned\" seat=\"6\" value=\"117.50\"/>", Street.Flop, Playerlist1, actions);

            Assert.AreEqual(new HandAction("TestPlayer6", HandActionType.UNCALLED_BET, 117.50m, Street.Flop), handAction);
        }

        //[Test]
        //public void ParseRegularActionLine_Check_Works()
        //{
        //    HandAction handAction = Parser.ParseRegularAction("Player STOPCRYINGB79 checks", Street.Flop, EmptyPlayerlist, false);

        //    Assert.AreEqual(new HandAction("STOPCRYINGB79", HandActionType.CHECK, 0m, Street.Flop), handAction);
        //}

        //[Test]
        //public void ParseRegularActionLine_Call_Works()
        //{
        //    HandAction handAction = Parser.ParseRegularAction("Player digbick30 calls (16)", Street.Flop, EmptyPlayerlist, false);

        //    Assert.AreEqual(new HandAction("digbick30", HandActionType.CALL, 16m, Street.Flop), handAction);
        //}

        //[Test]
        //public void ParseRegularActionLine_Raise_Works()
        //{
        //    HandAction handAction = Parser.ParseRegularAction("Player STOPCRYINGB79 raises (20)", Street.Flop, EmptyPlayerlist, false);

        //    Assert.AreEqual(new HandAction("STOPCRYINGB79", HandActionType.RAISE, 20m, Street.Flop), handAction);
        //}

        //[Test]
        //public void ParseRegularActionLine_Fold_Works()
        //{
        //    HandAction handAction = Parser.ParseRegularAction("Player STOPCRYINGB79 folds", Street.Flop, EmptyPlayerlist, false);

        //    Assert.AreEqual(new HandAction("STOPCRYINGB79", HandActionType.FOLD, 0m, Street.Flop), handAction);
        //}

        //[Test]
        //public void ParseShowdownActionLine_Wins_Works()
        //{
        //    List<HandAction> actions = new List<HandAction>();
        //    HandAction handAction = Parser.ParseWinningsAction("*Player johna52801 shows: One pair of Js [Jd Js]. Bets: 4.07. Collects: 7.73. Wins: 3.66.", EmptyPlayerlist, false);

        //    Assert.AreEqual(new WinningsAction("johna52801", HandActionType.WINS, 7.73m, 0), handAction);
        //}
    }
}
