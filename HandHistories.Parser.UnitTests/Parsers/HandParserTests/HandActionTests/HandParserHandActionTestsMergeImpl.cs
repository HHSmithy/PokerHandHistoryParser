using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.HandActionTests
{
    [TestFixture]
    class HandParserHandActionTestsMergeImpl : HandParserHandActionTests
    {
        public HandParserHandActionTestsMergeImpl() : base("Merge")
        {
        }

        protected override List<HandAction> ExpectedHandActionsBasicHand
        {
            get
            {            
                return new List<HandAction>()
                {
                    new HandAction("yuseff415", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                    new HandAction("nemi711", HandActionType.BIG_BLIND, 10m, Street.Preflop),
                    new HandAction("yuseff415", HandActionType.RAISE, 30m - 5m, Street.Preflop),
                    new HandAction("nemi711", HandActionType.CALL, 20m, Street.Preflop),
                    new HandAction("yuseff415", HandActionType.CHECK, 0, Street.Flop),
                    new HandAction("nemi711", HandActionType.CHECK, 0, Street.Flop),
                    new HandAction("yuseff415", HandActionType.CHECK, 0, Street.Turn),
                    new HandAction("nemi711", HandActionType.CHECK, 0, Street.Turn),
                    new HandAction("yuseff415", HandActionType.CHECK, 0, Street.River),
                    new HandAction("nemi711", HandActionType.CHECK, 0, Street.River),
                    new HandAction("yuseff415", HandActionType.SHOW, 0, Street.Showdown),
                    new HandAction("nemi711", HandActionType.SHOW, 0, Street.Showdown),                       
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsBasicHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("nemi711", WinningsActionType.WINS, 59.5m, 0), }; }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("dugaly", HandActionType.SMALL_BLIND, 1m, Street.Preflop),
                    new HandAction("GODEXISTSJK", HandActionType.BIG_BLIND, 2m, Street.Preflop),
                    new HandAction("anica11", HandActionType.RAISE, 5.75m, Street.Preflop),
                    new HandAction("fasthand007", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("m0ney2Blow", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("Hateordiehere", HandActionType.RAISE, 14m, Street.Preflop),
                    new HandAction("dugaly", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("GODEXISTSJK", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("anica11", HandActionType.RAISE, 34.66m - 5.75m, Street.Preflop),
                    new HandAction("Hateordiehere", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("anica11", HandActionType.MUCKS, 0, Street.Showdown),               
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsFoldedPreflop
        {
            get { return new List<WinningsAction>() { new WinningsAction("anica11", WinningsActionType.WINS, 51.66m, 0), }; }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("cashgreedy00", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                    new HandAction("bebenwong", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                    new HandAction("JennaMaroney", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("sportslove011", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("dk2112", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("nikcname1", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("rossiwstaxi", HandActionType.RAISE, 2m, Street.Preflop),
                    new HandAction("cashgreedy00", HandActionType.RAISE, 8m - 0.5m, Street.Preflop),
                    new HandAction("bebenwong", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("rossiwstaxi", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("cashgreedy00", HandActionType.MUCKS, 0, Street.Showdown),                       
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActions3BetHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("cashgreedy00", WinningsActionType.WINS, 11m, 0), }; }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("GODEXISTSJK", HandActionType.SMALL_BLIND, 1m, Street.Preflop),
                    new HandAction("anica11", HandActionType.BIG_BLIND, 2m, Street.Preflop),
                    new HandAction("fasthand007", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("m0ney2Blow", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("Hateordiehere", HandActionType.RAISE, 8m, Street.Preflop),
                    new HandAction("dugaly", HandActionType.RAISE, 22.25m, Street.Preflop),
                    new HandAction("GODEXISTSJK", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("anica11", HandActionType.RAISE, 49m - 2m, Street.Preflop),
                    new HandAction("Hateordiehere", HandActionType.FOLD, 0, Street.Preflop),
                    new AllInAction("dugaly", 200.59m, Street.Preflop, false),
                    new AllInAction("anica11", 152.10m, Street.Preflop, false),
                    new HandAction("anica11", HandActionType.SHOW, 0m, Street.Preflop),
                    new HandAction("dugaly", HandActionType.SHOW, 0m, Street.Preflop),
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsAllInHand
        {
            get
            {
                return new List<WinningsAction>()
                {  
                    new WinningsAction("dugaly", WinningsActionType.WINS, 21.74m, 1),                               
                    new WinningsAction("dugaly", WinningsActionType.WINS, 407.7m, 0)
                };
            }
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
                Assert.Ignore("HiLo parsing currently not supported.");
                throw new NotImplementedException();
            }
        }

        protected override List<WinningsAction> ExpectedWinnersOmahaHiLoHand
        {
            get { throw new NotImplementedException(); }
        }

        [Test]
        public void GameCancelledHand_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("", HandActionType.GAME_CANCELLED, 0, Street.Showdown)
                                    };

            TestParseActions("CancelledHand", expectedActions, new List<WinningsAction>());
        }
    }
}
