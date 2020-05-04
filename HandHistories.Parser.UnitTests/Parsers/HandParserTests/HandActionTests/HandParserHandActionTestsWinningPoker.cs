using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using NUnit.Framework;
using System;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.HandActionTests
{
    [TestFixture]
    class HandParserHandActionTestsWinningPokerImpl : HandParserHandActionTests
    {
        public HandParserHandActionTestsWinningPokerImpl()
            : base("WinningPoker")
        {
        }

        [Test]
        public void Sittingout_Works()
        {
//Player 5x5 = 10 has small blind(15)
//Player Scout327 sitting out
//Player PLOwned has big blind(30)
//Player sugar baby raises(90)
//Player 5x5 = 10 folds
//Player PLOwned calls(60)
//***FLOP * **: [6d 7d 7c]
//Player PLOwned bets(64)
//Player sugar baby calls(64)
//*** TURN***: [6d 7d 7c]
//[Kc]
//Player PLOwned checks
//Player sugar baby checks
//*** RIVER ***: [6d 7d 7c Kc]
//[6h]
//Player PLOwned checks
//Player sugar baby checks
//Player PLOwned mucks cards
//------ Summary ------
//Pot: 320.75. Rake 2.17. JP fee 0.08
//Board: [6d 7d 7c Kc 6h]
//* Player sugar baby shows: Two pairs.Js and 7s[5c Jh 8c Jd]. Bets: 154. Collects: 320.75. Wins: 166.75.


            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("5x5=10", HandActionType.SMALL_BLIND, 15m, Street.Preflop),
                new HandAction("PLOwned", HandActionType.BIG_BLIND, 30m, Street.Preflop),

                new HandAction("sugar baby", HandActionType.RAISE, 90m, Street.Preflop),
                new HandAction("5x5=10", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("PLOwned", HandActionType.CALL, 60m, Street.Preflop),

                new HandAction("PLOwned", HandActionType.BET, 64m, Street.Flop),
                new HandAction("sugar baby", HandActionType.CALL, 64m, Street.Flop),

                new HandAction("PLOwned", HandActionType.CHECK, 0m, Street.Turn),
                new HandAction("sugar baby", HandActionType.CHECK, 0m, Street.Turn),

                new HandAction("PLOwned", HandActionType.CHECK, 0m, Street.River),
                new HandAction("sugar baby", HandActionType.CHECK, 0m, Street.River),

            };

            var expectedWinners = new List<WinningsAction>(){
                new WinningsAction("sugar baby", WinningsActionType.WINS, 320.75m, 0)
            };

            TestParseActions("SitOut", expectedActions, expectedWinners);
        }

        [Test]
        public void WaitingBB_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("Aquasces1", HandActionType.SMALL_BLIND, 2m, Street.Preflop),
                new HandAction("COMON-JOE-JUG", HandActionType.BIG_BLIND, 4m, Street.Preflop),
                new HandAction("888game888", HandActionType.POSTS, 2m, Street.Preflop),

                new HandAction("888game888", HandActionType.CALL, 2m, Street.Preflop),
                new HandAction("Commons", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("Aquasces1", HandActionType.RAISE, 14m, Street.Preflop),
                new HandAction("COMON-JOE-JUG", HandActionType.CALL, 12, Street.Preflop),
                new HandAction("888game888", HandActionType.CALL, 12, Street.Preflop),

                new HandAction("Aquasces1", HandActionType.CHECK, 0m, Street.Flop),
                new HandAction("COMON-JOE-JUG", HandActionType.CHECK, 0m, Street.Flop),
                new HandAction("888game888", HandActionType.CHECK, 0m, Street.Flop),

                new HandAction("Aquasces1", HandActionType.CHECK, 0m, Street.Turn),
                new HandAction("COMON-JOE-JUG", HandActionType.CHECK, 0m, Street.Turn),
                new HandAction("888game888", HandActionType.BET, 46m, Street.Turn),
                new HandAction("Aquasces1", HandActionType.FOLD, 0m, Street.Turn),
                new HandAction("COMON-JOE-JUG", HandActionType.FOLD, 0m, Street.Turn),
                new HandAction("888game888", HandActionType.UNCALLED_BET, 46m, Street.Turn),
            };

            var expectedWinners = new List<WinningsAction>(){
                new WinningsAction("888game888", WinningsActionType.WINS, 46m, 0)
            };

            TestParseActions("WaitBB", expectedActions, expectedWinners);
        }

        [Test]
        public void Straddle_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("L6U11C2K1Y3", HandActionType.SMALL_BLIND, 0.10m, Street.Preflop),
                new HandAction("rds44sdr", HandActionType.BIG_BLIND, 0.25m, Street.Preflop),
                new HandAction("D3SISION", HandActionType.POSTS, 0.50m, Street.Preflop),

                new HandAction("ButtonSmasher", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("Cellar door", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("bbone", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("L6U11C2K1Y3", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("rds44sdr", HandActionType.CALL, 0.25m, Street.Preflop),
                new HandAction("D3SISION", HandActionType.CHECK, 0m, Street.Preflop),

                new HandAction("rds44sdr", HandActionType.CHECK, 0m, Street.Flop),
                new HandAction("D3SISION", HandActionType.BET, 0.50m, Street.Flop),
                new HandAction("rds44sdr", HandActionType.FOLD, 0m, Street.Flop),
                new HandAction("D3SISION", HandActionType.UNCALLED_BET, 0.50m, Street.Flop),
            };

            var expectedWinners = new List<WinningsAction>(){
                new WinningsAction("D3SISION", WinningsActionType.WINS, 0.80m, 0)
            };

            TestParseActions("Straddle", expectedActions, expectedWinners);
        }

        [Test]
        public void Posting_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("bbone", HandActionType.SMALL_BLIND, 0.10m, Street.Preflop),
                new HandAction("ButtonSmasher", HandActionType.BIG_BLIND, 0.25m, Street.Preflop),
                new HandAction("kenzielee", HandActionType.POSTS, 0.25m, Street.Preflop),

                new HandAction("kenzielee", HandActionType.RAISE, 0.25m, Street.Preflop),
                new HandAction("Cellar door", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("4ofakind7", HandActionType.CALL, 0.50m, Street.Preflop),
                new HandAction("Garzvorgh", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("bbone", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("ButtonSmasher", HandActionType.FOLD, 0m, Street.Preflop),

                new HandAction("kenzielee", HandActionType.BET, 0.52m, Street.Flop),
                new HandAction("4ofakind7", HandActionType.FOLD, 0m, Street.Flop),
                new HandAction("kenzielee", HandActionType.UNCALLED_BET, 0.52m, Street.Flop),
            };

            var expectedWinners = new List<WinningsAction>(){
                new WinningsAction("kenzielee", WinningsActionType.WINS, 1.04m, 0)
            };

            TestParseActions("Post", expectedActions, expectedWinners);
        }

        [Test]
        public void PostingDead_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("choptop", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop),
                                        new HandAction("Venvellator", HandActionType.BIG_BLIND, 0.50m, Street.Preflop),
                                        new HandAction("TheKunttzz", HandActionType.POSTS, 0.75m, Street.Preflop),

                                        new HandAction("TheKunttzz", HandActionType.CHECK, 0, Street.Preflop),
                                        new HandAction("tonyaces", HandActionType.RAISE, 1.50m, Street.Preflop),
                                        new HandAction("Cellar door", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("ConverseX", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("choptop", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("Venvellator", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("TheKunttzz", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("tonyaces", HandActionType.UNCALLED_BET, 1m, Street.Preflop),
                                    };

            var expectedWinners = new List<WinningsAction>(){
                new WinningsAction("tonyaces", WinningsActionType.WINS, 2m, 0)
            };

            TestParseActions("PostingDead", expectedActions, expectedWinners);
        }

        [Test]
        public void StrangePlayerNames_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("do not-call", HandActionType.SMALL_BLIND, 2m, Street.Preflop),
                new HandAction("OhGoodGolly", HandActionType.BIG_BLIND, 4m, Street.Preflop),

                new HandAction("dont.do.it", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("COMON-JOE-JUG", HandActionType.RAISE, 14, Street.Preflop),
                new HandAction("do not-call", HandActionType.CALL, 12m, Street.Preflop),
                new HandAction("OhGoodGolly", HandActionType.CALL, 10, Street.Preflop),

                new HandAction("do not-call", HandActionType.CHECK, 0, Street.Flop),
                new HandAction("OhGoodGolly", HandActionType.CHECK, 0, Street.Flop),
                new HandAction("COMON-JOE-JUG", HandActionType.BET, 39.75m, Street.Flop),
                new HandAction("do not-call", HandActionType.FOLD, 0, Street.Flop),
                new HandAction("OhGoodGolly", HandActionType.FOLD, 0, Street.Flop),

                new HandAction("COMON-JOE-JUG", HandActionType.UNCALLED_BET, 39.75m, Street.Flop),
            };

            var expectedWinners = new List<WinningsAction>(){
                new WinningsAction("COMON-JOE-JUG", WinningsActionType.WINS, 39.75m, 0),
            };

            TestParseActions("StrangePlayerNames", expectedActions, expectedWinners);
        }

        [Test]
        public void PlayerNameWithParanthesis_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("VanishingFlames", HandActionType.SMALL_BLIND, 2m, Street.Preflop),
                new HandAction("cscwildcat1", HandActionType.BIG_BLIND, 4m, Street.Preflop),

                new HandAction("((((??????!!!!!!))))", HandActionType.RAISE, 8m, Street.Preflop),
                new HandAction("brokejew", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("VanishingFlames", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("cscwildcat1", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("((((??????!!!!!!))))", HandActionType.UNCALLED_BET, 4m, Street.Preflop),
            };

            var expectedWinners = new List<WinningsAction>(){
                new WinningsAction("((((??????!!!!!!))))", WinningsActionType.WINS, 10m, 0)
            };

            TestParseActions("PlayerNameWithParanthesis", expectedActions, expectedWinners);
        }

        [Test]
        public void UncalledBet_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("Shipologist", HandActionType.SMALL_BLIND, 0.10m, Street.Preflop),
                new HandAction("borjilius79", HandActionType.BIG_BLIND, 0.25m, Street.Preflop),

                new HandAction("LadyStack", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("FetusMunch", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("xXxAudiA5xXx", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("Daviciko", HandActionType.RAISE, 0.75m, Street.Preflop),
                new HandAction("Shipologist", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("borjilius79", HandActionType.FOLD, 0, Street.Preflop),

                new HandAction("Daviciko", HandActionType.UNCALLED_BET, 0.50m, Street.Preflop),
            };

            var expectedWinners = new List<WinningsAction>(){
                new WinningsAction("Daviciko", WinningsActionType.WINS, 0.60m,0),
            };

            TestParseActions("UncalledBet", expectedActions, expectedWinners);
        }

        protected override List<HandAction> ExpectedHandActionsBasicHand
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("HanSoloDolo", HandActionType.SMALL_BLIND, 2m, Street.Preflop),
                    new HandAction("ap1104", HandActionType.BIG_BLIND, 4m, Street.Preflop),

                    new HandAction("Pokergodacolyte", HandActionType.RAISE, 12, Street.Preflop),
                    new HandAction("cuzimwhite", HandActionType.CALL, 12, Street.Preflop),
                    new HandAction("primume", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("HanSoloDolo", HandActionType.CALL, 10m, Street.Preflop),
                    new HandAction("ap1104", HandActionType.CALL, 8m, Street.Preflop),

                    new HandAction("HanSoloDolo", HandActionType.CHECK, 0m, Street.Flop),
                    new HandAction("ap1104", HandActionType.CHECK, 0m, Street.Flop),
                    new HandAction("Pokergodacolyte", HandActionType.CHECK, 0m, Street.Flop),
                    new HandAction("cuzimwhite", HandActionType.CHECK, 0m, Street.Flop),

                    new HandAction("HanSoloDolo", HandActionType.CHECK, 0m, Street.Turn),
                    new HandAction("ap1104", HandActionType.CHECK, 0m, Street.Turn),
                    new HandAction("Pokergodacolyte", HandActionType.CHECK, 0m, Street.Turn),
                    new HandAction("cuzimwhite", HandActionType.CHECK, 0m, Street.Turn),

                    new HandAction("HanSoloDolo", HandActionType.CHECK, 0m, Street.River),
                    new HandAction("ap1104", HandActionType.CHECK, 0m, Street.River),
                    new HandAction("Pokergodacolyte", HandActionType.CHECK, 0m, Street.River),
                    new HandAction("cuzimwhite", HandActionType.CHECK, 0m, Street.River),
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsBasicHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("HanSoloDolo", WinningsActionType.WINS, 45.75m, 0), }; }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("LadyStack", HandActionType.SMALL_BLIND, 0.1m, Street.Preflop),
                    new HandAction("FetusMunch", HandActionType.BIG_BLIND, 0.25m, Street.Preflop),

                    new HandAction("xXxAudiA5xXx", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("johna52801", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("Shipologist", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("borjilius79", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("LadyStack", HandActionType.RAISE, 0.70m, Street.Preflop),
                    new HandAction("FetusMunch", HandActionType.FOLD, 0, Street.Preflop),

                    new HandAction("LadyStack", HandActionType.UNCALLED_BET, 0.55m, Street.Preflop),
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsFoldedPreflop
        {
            get { return new List<WinningsAction>() { new WinningsAction("LadyStack", WinningsActionType.WINS, 0.50m, 0) }; }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("digbick30", HandActionType.SMALL_BLIND, 2m, Street.Preflop),
                    new HandAction("STOPCRYINGB79", HandActionType.BIG_BLIND, 4m, Street.Preflop),

                    new HandAction("digbick30", HandActionType.RAISE, 6, Street.Preflop),
                    new HandAction("STOPCRYINGB79", HandActionType.RAISE, 20, Street.Preflop),
                    new HandAction("digbick30", HandActionType.CALL, 16m, Street.Preflop),

                    new HandAction("STOPCRYINGB79", HandActionType.CHECK, 0, Street.Flop),
                    new HandAction("digbick30", HandActionType.CHECK, 0, Street.Flop),

                    new HandAction("STOPCRYINGB79", HandActionType.CHECK, 0, Street.Turn),
                    new HandAction("digbick30", HandActionType.CHECK, 0, Street.Turn),

                    new HandAction("STOPCRYINGB79", HandActionType.CHECK, 0, Street.River),
                    new HandAction("digbick30", HandActionType.BET, 15.66m, Street.River),
                    new HandAction("STOPCRYINGB79", HandActionType.FOLD, 0, Street.River),

                    new HandAction("digbick30", HandActionType.UNCALLED_BET, 15.66m, Street.River),
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActions3BetHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("digbick30", WinningsActionType.WINS, 47m, 0) }; }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("do not-call", HandActionType.SMALL_BLIND, 2m, Street.Preflop),
                    new HandAction("digbick30", HandActionType.BIG_BLIND, 4m, Street.Preflop),

                    new HandAction("do not-call", HandActionType.RAISE, 10, Street.Preflop),
                    new HandAction("digbick30", HandActionType.RAISE, 32, Street.Preflop),
                    new HandAction("do not-call", HandActionType.CALL, 24m, Street.Preflop),

                    new HandAction("digbick30", HandActionType.BET, 71m, Street.Flop),
                    new HandAction("do not-call", HandActionType.RAISE, 124m, Street.Flop, true),
                    new HandAction("digbick30", HandActionType.CALL, 53m, Street.Flop),
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsAllInHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("do not-call", WinningsActionType.WINS, 319m, 0) }; }
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
                return new List<HandAction>()
                {
                    new HandAction("ColFortune81", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                    new HandAction("Belkiss2", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                    new HandAction("ribby53", HandActionType.BIG_BLIND, 1m, Street.Preflop),

                    new HandAction("funkyOne81", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("ribby53", HandActionType.CHECK, 0, Street.Preflop),
                    new HandAction("Zident", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("mq260", HandActionType.CALL, 1m, Street.Preflop),
                    new HandAction("ColFortune81", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("Belkiss2", HandActionType.CHECK, 0m, Street.Preflop),
                               
                    new HandAction("Belkiss2", HandActionType.CHECK, 0m, Street.Flop),
                    new HandAction("ribby53", HandActionType.CHECK, 0m, Street.Flop),
                    new HandAction("mq260", HandActionType.CHECK, 0m, Street.Flop),

                    new HandAction("Belkiss2", HandActionType.CHECK, 0m, Street.Turn),
                    new HandAction("ribby53", HandActionType.CHECK, 0m, Street.Turn),
                    new HandAction("mq260", HandActionType.CHECK, 0m, Street.Turn),

                    new HandAction("Belkiss2", HandActionType.CHECK, 0m, Street.River),
                    new HandAction("ribby53", HandActionType.CHECK, 0m, Street.River),
                    new HandAction("mq260", HandActionType.CHECK, 0m, Street.River),

                    new HandAction("Belkiss2", HandActionType.SHOW, 0m, Street.Showdown),
                    new HandAction("ribby53", HandActionType.MUCKS,0m, Street.Showdown),
                    new HandAction("mq260", HandActionType.MUCKS,0m, Street.Showdown),

                   
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersOmahaHiLoHand
        {
            get 
            { 
                return new List<WinningsAction>() 
                { 
                    new WinningsAction("Belkiss2", WinningsActionType.WINS, 1.67m, 0),
                    new WinningsAction("Belkiss2", WinningsActionType.WINS, 1.67m, 0)
                }; 
            }
        }
    }
}
