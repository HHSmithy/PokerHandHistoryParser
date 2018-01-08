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

        [Test]
        public void ParseHandActions_AllInOnFlop()
        {
            var expected = new List<HandAction>()
            {
                new HandAction("1mperial", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                new HandAction("Postrail", HandActionType.BIG_BLIND, 10m, Street.Preflop),
                new HandAction("Darkking_pt", HandActionType.FOLD, Street.Preflop),
                new HandAction("ginalisa18", HandActionType.CALL, 10m, Street.Preflop),
                new HandAction("1mperial", HandActionType.FOLD, Street.Preflop),
                new HandAction("Postrail", HandActionType.RAISE, 30m, Street.Preflop),
                new HandAction("ginalisa18", HandActionType.RAISE, 70m, Street.Preflop),
                new HandAction("Postrail", HandActionType.CALL, 40m, Street.Preflop),

                new HandAction("Postrail", HandActionType.CHECK, 0m, Street.Flop),
                new HandAction("ginalisa18", HandActionType.BET, 120m, Street.Flop),
                new HandAction("Postrail", HandActionType.RAISE, 220m, Street.Flop, true),
                new HandAction("ginalisa18", HandActionType.CALL, 100m, Street.Flop, true),
                new HandAction("Postrail", HandActionType.SHOW, 0m, Street.Showdown),
                new HandAction("ginalisa18", HandActionType.SHOW, 0m, Street.Showdown),
                
            };

            var expectedWinners = new List<WinningsAction>() 
            { 
                new WinningsAction("Postrail", WinningsActionType.WINS, 603m, 0),
            };

            TestParseActions("AllInOnFlop", expected, expectedWinners);
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
                    new HandAction("Rene Lacoste", HandActionType.MUCKS, 0m, Street.Showdown),
                    
                };

                return actions;
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsBasicHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("Rene Lacoste", WinningsActionType.WINS, 39.50m, 0), }; }
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
                    new HandAction("goSuckout", HandActionType.MUCKS, 0m, Street.Showdown),
                    
                };

                return actions;
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsFoldedPreflop
        {
            get { return new List<WinningsAction>() { new WinningsAction("goSuckout", WinningsActionType.WINS, 10m, 0), }; }
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
                    new HandAction("theking881", HandActionType.MUCKS, 0m, Street.Showdown),
                    
                };

                return actions;
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActions3BetHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("theking881", WinningsActionType.WINS, 99.50m, 0), }; }
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
                    
                };

                return actions;
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsAllInHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("jobetzu", WinningsActionType.WINS, 615.50m, 0), }; }
        }

        protected override List<HandAction> ExpectedHandActionsUncalledBetHand
        {
            get
            {
                Assert.Ignore();
                throw new NotImplementedException();
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsUncalledBetHand
        {
            get { throw new NotImplementedException(); }
        }

        protected override List<HandAction> ExpectedOmahaHiLoHand
        {
            get
            {
                Assert.Ignore("Hand Actions not implemented");
                throw new NotImplementedException();
            }
        }

        protected override List<WinningsAction> ExpectedWinnersOmahaHiLoHand
        {
            get { throw new NotImplementedException(); }
        }
    }
}