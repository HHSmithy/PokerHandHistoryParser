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
            Street street = Street.Flop;
            List<HandAction> actions = new List<HandAction>();
            HandAction handAction = Parser.ParseRegularAction("totti6720 bets 4.70€", street, actions, 0);

            Assert.AreEqual(new HandAction("totti6720", HandActionType.BET, 4.7m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Check_Works()
        {
            Street street = Street.Flop;
            List<HandAction> actions = new List<HandAction>();
            HandAction handAction = Parser.ParseRegularAction("RICO97133 checks", street, actions, 0);

            Assert.AreEqual(new HandAction("RICO97133", HandActionType.CHECK, 0m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Call_Works()
        {
            Street street = Street.Flop;
            List<HandAction> actions = new List<HandAction>();
            HandAction handAction = Parser.ParseRegularAction("sharon59221 calls 1.35€", street, actions, 0);

            Assert.AreEqual(new HandAction("sharon59221", HandActionType.CALL, 1.35m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_CallNoCurrency_Works()
        {
            Street street = Street.Flop;
            List<HandAction> actions = new List<HandAction>();
            HandAction handAction = Parser.ParseRegularAction("sharon59221 calls 1.35", street, actions, 0);

            Assert.AreEqual(new HandAction("sharon59221", HandActionType.CALL, 1.35m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Raise_Works()
        {
            Street street = Street.Flop;
            List<HandAction> actions = new List<HandAction>();
            HandAction handAction = Parser.ParseRegularAction("trasto51 raises 1.05€ to 1.35€", street, actions, 0);

            Assert.AreEqual(new HandAction("trasto51", HandActionType.RAISE, 1.35m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_RaiseNoCurrency_Works()
        {
            Street street = Street.Flop;
            List<HandAction> actions = new List<HandAction>();
            HandAction handAction = Parser.ParseRegularAction("Doksrill raises 765 to 1305", street, actions, 0);

            Assert.AreEqual(new HandAction("Doksrill", HandActionType.RAISE, 1305m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Fold_Works()
        {
            Street street = Street.Flop;
            List<HandAction> actions = new List<HandAction>();
            HandAction handAction = Parser.ParseRegularAction("-LePianiste- folds", street, actions, 0);

            Assert.AreEqual(new HandAction("-LePianiste-", HandActionType.FOLD, 0m, Street.Flop), handAction);
        }


        [Test]
        public void ParseRegularActionLine_UncalledBet_Works()
        {
            List<HandAction> actions = new List<HandAction>();
            HandAction handAction = Parser.ParseRegularAction("Uncalled bet of 1€ returned to tonQtaChatte", Street.Preflop, actions, 0);

            Assert.AreEqual(new HandAction("tonQtaChatte", HandActionType.UNCALLED_BET, 1m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_UncalledBet2_Works()
        {
            List<HandAction> actions = new List<HandAction>();
            HandAction handAction = Parser.ParseRegularAction("Uncalled bet of 210€ returned to generaltuvas", Street.Preflop, actions, 0);

            Assert.AreEqual(new HandAction("generaltuvas", HandActionType.UNCALLED_BET, 210m, Street.Preflop), handAction);
        }
    }
}
