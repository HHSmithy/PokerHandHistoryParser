using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using NUnit.Framework;
using HandHistories.Parser.UnitTests.Parsers.Base;
using HandHistories.Parser.Parsers.FastParser.Winamax;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.Winamax
{
    using Parser = WinamaxFastParserImpl;
    [TestFixture]
    class WinamaxFastParserActionTests : HandHistoryParserBaseTests
    {
        public WinamaxFastParserActionTests()
            : base("Winamax")
        {
        }

        protected WinamaxFastParserImpl GetFastParser()
        {
            return new WinamaxFastParserImpl();
        }

        List<HandAction> EmptyActions = new List<HandAction>();

        [Test]
        public void ParseBlindActionLine_PostingDead_Works()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("totti6720", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop),
                new HandAction("titi250", HandActionType.BIG_BLIND, 0.5m, Street.Preflop),
            };

            HandAction lastAction = null;
            decimal SB = 0m;

            HandAction handAction = Parser.ParseBlindAction("Dbrz34 posts small blind 0.25€ out of position", actions, ref lastAction, ref SB);

            Assert.AreEqual(new HandAction("Dbrz34", HandActionType.POSTS, 0.25m, Street.Preflop), handAction);
        }
        

        [Test]
        public void ParseBlindActionLine_PostingAnte_Works()
        {
            List<HandAction> actions = new List<HandAction>();
            HandAction lastAction = null;
            decimal SB = 0m;

            HandAction handAction = Parser.ParseBlindAction("Player1 posts ante 25", actions, ref lastAction, ref SB);

            Assert.AreEqual(new HandAction("Player1", HandActionType.ANTE, 25m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseBlindActionLine_PostingSmallBlind_Works()
        {
            List<HandAction> actions = new List<HandAction>();
            HandAction lastAction = null;
            decimal SB = 0m;

            HandAction handAction = Parser.ParseBlindAction("xNimzo49x posts small blind 0.25€", actions, ref lastAction, ref SB);

            Assert.AreEqual(new HandAction("xNimzo49x", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseBlindActionLine_PostingBigBlind_Works()
        {
            List<HandAction> actions = new List<HandAction>();
            HandAction lastAction = null;
            decimal SB = 0m;

            HandAction handAction = Parser.ParseBlindAction("titi250 posts big blind 0.50€", actions, ref lastAction, ref SB);

            Assert.AreEqual(new HandAction("titi250", HandActionType.BIG_BLIND, 0.5m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Bet_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("totti6720 bets 4.70€", Street.Flop, EmptyActions, 0);

            Assert.AreEqual(new HandAction("totti6720", HandActionType.BET, 4.7m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Check_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("RICO97133 checks", Street.Flop, EmptyActions, 0);

            Assert.AreEqual(new HandAction("RICO97133", HandActionType.CHECK, 0m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Call_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("sharon59221 calls 1.35€", Street.Flop, EmptyActions, 0);

            Assert.AreEqual(new HandAction("sharon59221", HandActionType.CALL, 1.35m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_CallNoCurrency_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("sharon59221 calls 1.35", Street.Flop, EmptyActions, 0);

            Assert.AreEqual(new HandAction("sharon59221", HandActionType.CALL, 1.35m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Raise_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("trasto51 raises 1.05€ to 1.35€", Street.Flop, EmptyActions, 0);

            Assert.AreEqual(new HandAction("trasto51", HandActionType.RAISE, 1.35m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_RaiseNoCurrency_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("Doksrill raises 765 to 1305", Street.Flop, EmptyActions, 0);

            Assert.AreEqual(new HandAction("Doksrill", HandActionType.RAISE, 1305m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Fold_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("-LePianiste- folds", Street.Flop, EmptyActions, 0);

            Assert.AreEqual(new HandAction("-LePianiste-", HandActionType.FOLD, 0m, Street.Flop), handAction);
        }

        List<HandAction> UncalledBetTestActions_Preflop = new List<HandAction>()
        {
            new HandAction("", HandActionType.UNKNOWN, 0, Street.Preflop),
        };

        List<HandAction> UncalledBetTestActions_Flop = new List<HandAction>()
        {
            new HandAction("", HandActionType.UNKNOWN, 0, Street.Flop),
        };

        [Test]
        public void ParseRegularActionLine_UncalledBet_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("Uncalled bet of 1€ returned to tonQtaChatte", Street.Preflop, UncalledBetTestActions_Preflop, 0);

            Assert.AreEqual(new HandAction("tonQtaChatte", HandActionType.UNCALLED_BET, 1m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_UncalledBet2_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("Uncalled bet of 210€ returned to generaltuvas", Street.Preflop, UncalledBetTestActions_Preflop, 0);

            Assert.AreEqual(new HandAction("generaltuvas", HandActionType.UNCALLED_BET, 210m, Street.Preflop), handAction);
        }


        //Example:
        //Bradwong37 raises 17.58€ to 27.33€ and is all-in
        //tonQtaChatte raises 17.58€ to 44.91€
        //*** TURN *** [2d Qc 4d][9s]
        //*** RIVER *** [2d Qc 4d 9s][Ad]
        //Uncalled bet of 17.58€ returned to tonQtaChatte

        /// <summary>
        /// because winamax outpust the uncalledbets from allins on the river we need to adjust the street where it gets inserted
        /// </summary>
        [Test]
        public void ParseRegularActionLine_UncalledBet_FlopToRiverJump_Works()
        {
            HandAction handAction = Parser.ParseRegularAction("Uncalled bet of 210€ returned to generaltuvas", Street.River, UncalledBetTestActions_Flop, 0);

            Assert.AreEqual(new HandAction("generaltuvas", HandActionType.UNCALLED_BET, 210m, Street.Flop), handAction);
        }
    }
}
