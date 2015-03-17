using System;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.HandActionTests
{
    [TestFixture]
    class HandParserHandActionTestsFullTiltImpl : HandParserHandActionTests
    {
        public HandParserHandActionTestsFullTiltImpl()
            : base("FullTilt")
        {
        }

        protected override List<HandAction> ExpectedHandActionsBasicHand
        {
            get
            {
                var actions = new List<HandAction>()
                {
                    new HandAction("Rene Lacoste", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                    new HandAction("ElkY", HandActionType.BIG_BLIND, 10m, Street.Preflop),
                    new HandAction("Rene Lacoste", HandActionType.RAISE, 15m, Street.Preflop),
                    new HandAction("ElkY", HandActionType.CALL, 10m, Street.Preflop),
                    new HandAction("ElkY", HandActionType.CHECK, 0m, Street.Flop),
                    new HandAction("Rene Lacoste", HandActionType.BET, 20m, Street.Flop),
                    new HandAction("ElkY", HandActionType.FOLD, 0m, Street.Flop),
                    new HandAction("Rene Lacoste", HandActionType.UNCALLED_BET, 20m, Street.Flop),
                    new HandAction("Rene Lacoste", HandActionType.MUCKS, 0m, Street.Flop),
                    new WinningsAction("Rene Lacoste", HandActionType.WINS, 39.50m, 0),
                };

                return actions;
            }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                var actions = new List<HandAction>()
                {
                    new HandAction("gosuoposum1", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                    new HandAction("goSuckout", HandActionType.BIG_BLIND, 10m, Street.Preflop),
                    new HandAction("gosuoposum1", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("goSuckout", HandActionType.UNCALLED_BET, 5m, Street.Preflop),
                    new HandAction("goSuckout", HandActionType.MUCKS, 0m, Street.Preflop),
                    new WinningsAction("goSuckout", HandActionType.WINS, 10m, 0),
                };

                return actions;
            }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                var actions = new List<HandAction>()
                {
                    new HandAction("jobetzu", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                    new HandAction("theking881", HandActionType.BIG_BLIND, 10m, Street.Preflop),
                    new HandAction("jobetzu", HandActionType.RAISE, 15m, Street.Preflop),
                    new HandAction("theking881", HandActionType.RAISE, 40m, Street.Preflop),
                    new HandAction("jobetzu", HandActionType.CALL, 30m, Street.Preflop),

                    new HandAction("theking881", HandActionType.BET, 30m, Street.Flop),
                    new HandAction("jobetzu", HandActionType.FOLD, 0m, Street.Flop),
                    new HandAction("theking881", HandActionType.UNCALLED_BET, 30m, Street.Flop),
                    new HandAction("theking881", HandActionType.MUCKS, 0m, Street.Flop),
                    new WinningsAction("theking881", HandActionType.WINS, 99.50m, 0),
                };

                return actions;
            }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                var actions = new List<HandAction>()
                {
                    new HandAction("theking881", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                    new HandAction("jobetzu", HandActionType.BIG_BLIND, 10m, Street.Preflop),
                    new HandAction("theking881", HandActionType.RAISE, 20m, Street.Preflop),
                    new HandAction("jobetzu", HandActionType.CALL, 15m, Street.Preflop),

                    new HandAction("jobetzu", HandActionType.CHECK, 0m, Street.Flop),
                    new HandAction("theking881", HandActionType.BET, 25m, Street.Flop),
                    new HandAction("jobetzu", HandActionType.CALL, 25m, Street.Flop),

                    new HandAction("jobetzu", HandActionType.CHECK, 0m, Street.Turn),
                    new HandAction("theking881", HandActionType.BET, 50m, Street.Turn),
                    new HandAction("jobetzu", HandActionType.RAISE, 120m, Street.Turn),
                    new HandAction("theking881", HandActionType.CALL, 70m, Street.Turn),

                    new HandAction("jobetzu", HandActionType.BET, 170m, Street.River),
                    new HandAction("theking881", HandActionType.CALL, 138m, Street.River, true),
                    new HandAction("jobetzu", HandActionType.UNCALLED_BET, 32m, Street.River),

                    new HandAction("jobetzu", HandActionType.SHOW, 0m, Street.Showdown),
                    new HandAction("theking881", HandActionType.MUCKS, 0m, Street.Showdown),
                    new WinningsAction("jobetzu", HandActionType.WINS, 615.50m, 0),
                };

                return actions;
            }
        }

        protected override List<HandAction> ExpectedOmahaHiLoHand
        {
            get
            {
                Assert.Ignore("Hand Actions not implemented");
                throw new NotImplementedException();
            }
        }
    }
}