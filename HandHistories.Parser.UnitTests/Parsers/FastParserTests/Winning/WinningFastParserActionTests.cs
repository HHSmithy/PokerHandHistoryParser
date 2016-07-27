using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.FastParser.Winamax;
using HandHistories.Parser.Parsers.FastParser.Winning;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.Winamax
{
    using Parser = WinningPokerNetworkFastParserImpl;
    [TestFixture]
    class WinningFastParserActionTests : HandHistoryParserBaseTests
    {
        public WinningFastParserActionTests()
            : base("WinningPoker")
        {
        }

        protected WinningPokerNetworkFastParserImpl GetFastParser()
        {
            return new WinningPokerNetworkFastParserImpl();
        }

        PlayerList EmptyPlayerlist = new PlayerList();
        List<HandAction> EmptyActions = new List<HandAction>();

        [Test]
        public void ParseRegularActionLine_Bet_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("Player digbick30 bets (15.66)", Street.Flop, EmptyPlayerlist,EmptyActions, false);

            Assert.AreEqual(new HandAction("digbick30", HandActionType.BET, 15.66m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Check_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("Player STOPCRYINGB79 checks", Street.Flop, EmptyPlayerlist, EmptyActions, false);

            Assert.AreEqual(new HandAction("STOPCRYINGB79", HandActionType.CHECK, 0m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Call_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("Player digbick30 calls (16)", Street.Flop, EmptyPlayerlist, EmptyActions, false);

            Assert.AreEqual(new HandAction("digbick30", HandActionType.CALL, 16m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Raise_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("Player STOPCRYINGB79 raises (20)", Street.Flop, EmptyPlayerlist, EmptyActions, false);

            Assert.AreEqual(new HandAction("STOPCRYINGB79", HandActionType.RAISE, 20m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Fold_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("Player STOPCRYINGB79 folds", Street.Flop, EmptyPlayerlist, EmptyActions, false);

            Assert.AreEqual(new HandAction("STOPCRYINGB79", HandActionType.FOLD, 0m, Street.Flop), handAction);
        }

        [Test]
        public void ParseShowdownActionLine_Wins_Works()
        {
            List<HandAction> actions = new List<HandAction>();
            HandAction handAction = Parser.ParseWinningsAction("*Player johna52801 shows: One pair of Js [Jd Js]. Bets: 4.07. Collects: 7.73. Wins: 3.66.", EmptyPlayerlist, false);

            Assert.AreEqual(new WinningsAction("johna52801", HandActionType.WINS, 7.73m, 0), handAction);
        }
    }
}
