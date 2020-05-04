using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.HandActionTests
{
    [TestFixture]
    class HandParserHandActionTestsPokerStarsImpl : HandParserHandActionTests
    {
        public HandParserHandActionTestsPokerStarsImpl() : base("PokerStars")
        {
        }


        [Test]
        public void PlayerLeavesTableMidHand_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("Plz3betMe", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                new HandAction("Luxuswasser", HandActionType.BIG_BLIND, 1m, Street.Preflop),

                new HandAction("JJScvbnm", HandActionType.RAISE, 3, Street.Preflop),
                new HandAction("Mark_999dk", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("BSTUSim", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("nanotehn77", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("Plz3betMe", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("Luxuswasser", HandActionType.FOLD, 0, Street.Preflop),

                new HandAction("JJScvbnm", HandActionType.UNCALLED_BET, 2, Street.Preflop),
                new HandAction("JJScvbnm", HandActionType.MUCKS,0, Street.Showdown)
            };

            var expectedWinners = new List<WinningsAction>()
            {
                new WinningsAction("JJScvbnm", WinningsActionType.WINS, 2.50m, 0),
            };

            TestParseActions("LeavesTableMidHand", expectedActions, expectedWinners);
        }

        [Test]
        public void StrangePlayerNames_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("here_we_gizo", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                new HandAction("PAARTYPAN", HandActionType.BIG_BLIND, 1m, Street.Preflop),

                new HandAction("liscla223", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("wo_ooly :D", HandActionType.RAISE, 3, Street.Preflop),
                new HandAction("Jon9ball", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("here_we_gizo", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("PAARTYPAN", HandActionType.FOLD, 0, Street.Preflop),

                new HandAction("wo_ooly :D", HandActionType.UNCALLED_BET, 2, Street.Preflop),
                new HandAction("wo_ooly :D", HandActionType.MUCKS,0, Street.Showdown)
            };

            var expectedWinners = new List<WinningsAction>()
            {
                new WinningsAction("wo_ooly :D", WinningsActionType.WINS, 2.50m, 0),
            };

            TestParseActions("StrangePlayerNames", expectedActions, expectedWinners);
        }

        [Test]
        public void AllInHand_NeedsRaiseAdjusting_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("BiatchPeople", HandActionType.SMALL_BLIND, 150m, Street.Preflop),
                new HandAction("Bluf_To_Much", HandActionType.BIG_BLIND, 300m, Street.Preflop),
                                        
                new HandAction("Ravenswood13", HandActionType.FOLD, 0m, Street.Preflop),                                     
                new HandAction("Crazy Elior", HandActionType.RAISE, 600m, Street.Preflop),
                new HandAction("joiso", HandActionType.RAISE, 900m, Street.Preflop),
                new HandAction("Ilari FIN", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("LewisFriend", HandActionType.CALL, 900m, Street.Preflop),
                new HandAction("BiatchPeople", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("Bluf_To_Much", HandActionType.CALL, 600m, Street.Preflop),
                new HandAction("Crazy Elior", HandActionType.CALL, 300m, Street.Preflop),

                new HandAction("Bluf_To_Much", HandActionType.CHECK, 0m, Street.Flop),
                new HandAction("Crazy Elior", HandActionType.CHECK, 0m, Street.Flop),
                new HandAction("joiso", HandActionType.BET, 300m, Street.Flop),
                new HandAction("LewisFriend", HandActionType.CALL, 300m, Street.Flop),
                new HandAction("Bluf_To_Much", HandActionType.CALL, 300m, Street.Flop),
                new HandAction("Crazy Elior", HandActionType.RAISE, 382.50m, Street.Flop, true),
                new HandAction("joiso", HandActionType.CALL, 82.50m, Street.Flop),
                new HandAction("LewisFriend", HandActionType.CALL, 82.50m, Street.Flop),
                new HandAction("Bluf_To_Much", HandActionType.CALL, 82.50m, Street.Flop),
                                        
                new HandAction("Bluf_To_Much", HandActionType.CHECK, 0m, Street.Turn),
                new HandAction("joiso", HandActionType.BET, 600m, Street.Turn),
                new HandAction("LewisFriend", HandActionType.CALL, 600m, Street.Turn),
                new HandAction("Bluf_To_Much", HandActionType.CALL, 600m, Street.Turn),

                new HandAction("Bluf_To_Much", HandActionType.BET, 600m, Street.River),
                new HandAction("joiso", HandActionType.FOLD, 0, Street.River),
                new HandAction("LewisFriend", HandActionType.RAISE, 1200m, Street.River),
                new HandAction("Bluf_To_Much", HandActionType.CALL, 600m, Street.River),

                new HandAction("LewisFriend", HandActionType.SHOW, 0, Street.Showdown),
                new HandAction("Bluf_To_Much", HandActionType.SHOW, 0, Street.Showdown),
                new HandAction("Crazy Elior", HandActionType.SHOW, 0, Street.Showdown),                     
            };

            var expectedWinners = new List<WinningsAction>()
            {
                new WinningsAction("Bluf_To_Much", WinningsActionType.WINS_SIDE_POT, 2100, 1),
                new WinningsAction("LewisFriend", WinningsActionType.WINS_SIDE_POT, 2100, 1),
                new WinningsAction("Crazy Elior", WinningsActionType.WINS, 2637.50m, 0),
                new WinningsAction("LewisFriend", WinningsActionType.WINS, 2637.50m, 0),  
            };

            TestParseActions("NeedsRaiseAdjusting", expectedActions, expectedWinners);
        }

        [Test]
        public void AllInHand_NeedsRaiseAdjusting_BigBlindOptionRaisesOption_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("idirabotai", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                new HandAction("jweeke", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                new HandAction("idirabotai", HandActionType.CALL, 0.5m, Street.Preflop),
                new HandAction("jweeke", HandActionType.RAISE, 2m, Street.Preflop),
                new HandAction("idirabotai", HandActionType.CALL, 2m, Street.Preflop),

                new HandAction("jweeke", HandActionType.BET, 4m, Street.Flop),
                new HandAction("idirabotai", HandActionType.CALL, 4m, Street.Flop),

                new HandAction("jweeke", HandActionType.CHECK, 0, Street.Turn),
                new HandAction("idirabotai", HandActionType.BET, 7m, Street.Turn),
                new HandAction("jweeke", HandActionType.FOLD, 0, Street.Turn),

                new HandAction("idirabotai", HandActionType.UNCALLED_BET, 7m, Street.Turn),
                new HandAction("idirabotai", HandActionType.MUCKS, 0, Street.Showdown)
            };

            var expectedWinners = new List<WinningsAction>()
            {
              new WinningsAction("idirabotai", WinningsActionType.WINS, 13.50m, 0),
            };

            TestParseActions("BigBlindOptionRaisesOption", expectedActions, expectedWinners);
        }

        protected override List<HandAction> ExpectedHandActionsBasicHand
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("Player_L", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                    new HandAction("H6U5r", HandActionType.BIG_BLIND, 0.1m, Street.Preflop),

                    new HandAction("Player_L", HandActionType.ANTE, 0.02m, Street.Preflop),
                    new HandAction("H6U5r", HandActionType.ANTE, 0.02m, Street.Preflop),
                    new HandAction("LovID", HandActionType.ANTE, 0.02m, Street.Preflop),
                    new HandAction("Tunks2", HandActionType.ANTE, 0.02m, Street.Preflop),
                    new HandAction("Nightfox82", HandActionType.ANTE, 0.02m, Street.Preflop),
                    new HandAction("Asaki1", HandActionType.ANTE, 0.02m, Street.Preflop),


                    new HandAction("LovID", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("Tunks2", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("Nightfox82", HandActionType.RAISE, 0.32m, Street.Preflop),
                    new HandAction("Asaki1", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("Player_L", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("H6U5r", HandActionType.CALL, 0.22m, Street.Preflop),

                    new HandAction("H6U5r", HandActionType.CHECK, 0m, Street.Flop),
                    new HandAction("Nightfox82", HandActionType.CHECK, 0m, Street.Flop),

                    new HandAction("H6U5r", HandActionType.CHECK, 0m, Street.Turn),
                    new HandAction("Nightfox82", HandActionType.BET, 0.54m, Street.Turn),
                    new HandAction("H6U5r", HandActionType.CALL, 0.54m, Street.Turn),

                    new HandAction("H6U5r", HandActionType.CHECK, 0m, Street.River),
                    new HandAction("Nightfox82", HandActionType.CHECK, 0m, Street.River),

                    new HandAction("H6U5r", HandActionType.SHOW, 0, Street.Showdown),
                    new HandAction("Nightfox82", HandActionType.SHOW, 0, Street.Showdown),
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsBasicHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("Nightfox82", WinningsActionType.WINS, 1.81m, 0) }; }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("idirabotai", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                    new HandAction("jweeke", HandActionType.BIG_BLIND, 1m, Street.Preflop),

                    new HandAction("idirabotai", HandActionType.CALL, 0.5m, Street.Preflop),
                    new HandAction("jweeke", HandActionType.RAISE, 2m, Street.Preflop),
                    new HandAction("idirabotai", HandActionType.FOLD, 0, Street.Preflop),

                    new HandAction("jweeke", HandActionType.UNCALLED_BET, 2, Street.Preflop),
                               
                    new HandAction("jweeke", HandActionType.MUCKS, 0, Street.Showdown)
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsFoldedPreflop
        {
            get { return new List<WinningsAction>() { new WinningsAction("jweeke", WinningsActionType.WINS, 2m, 0)}; }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("Tunks2", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                    new HandAction("Nightfox82", HandActionType.BIG_BLIND, 0.10m, Street.Preflop),

                    new HandAction("Player_L", HandActionType.ANTE, 0.02m, Street.Preflop),
                    new HandAction("H6U5r", HandActionType.ANTE, 0.02m, Street.Preflop),
                    new HandAction("LovID", HandActionType.ANTE, 0.02m, Street.Preflop),
                    new HandAction("Tunks2", HandActionType.ANTE, 0.02m, Street.Preflop),
                    new HandAction("Nightfox82", HandActionType.ANTE, 0.02m, Street.Preflop),
                    new HandAction("Asaki1", HandActionType.ANTE, 0.02m, Street.Preflop),

                    new HandAction("Asaki1", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("Player_L", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("H6U5r", HandActionType.RAISE, 0.3m, Street.Preflop),
                    new HandAction("LovID", HandActionType.RAISE, 0.9m, Street.Preflop),
                    new HandAction("Tunks2", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("Nightfox82", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("H6U5r", HandActionType.FOLD, 0, Street.Preflop),

                    new HandAction("LovID", HandActionType.UNCALLED_BET, 0.60m, Street.Preflop),
                    new HandAction("LovID", HandActionType.MUCKS, 0, Street.Showdown)
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActions3BetHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("LovID", WinningsActionType.WINS, 0.87m, 0), }; }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("numbush", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                               new HandAction("DonKingKong", HandActionType.BIG_BLIND, 0.10m, Street.Preflop),

                               new HandAction("Captain2323", HandActionType.ANTE, 0.02m, Street.Preflop),
                               new HandAction("matze1987", HandActionType.ANTE, 0.02m, Street.Preflop),
                               new HandAction("numbush", HandActionType.ANTE, 0.02m, Street.Preflop),
                               new HandAction("DonKingKong", HandActionType.ANTE, 0.02m, Street.Preflop),
                               new HandAction("Gaben13", HandActionType.ANTE, 0.02m, Street.Preflop),

                               new HandAction("Gaben13", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Captain2323", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("matze1987", HandActionType.RAISE, 0.3m, Street.Preflop),
                               new HandAction("numbush", HandActionType.RAISE, 1.25m, Street.Preflop),
                               new HandAction("DonKingKong", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("matze1987", HandActionType.CALL, 1m, Street.Preflop),
                               
                               new HandAction("numbush", HandActionType.BET, 2, Street.Flop),
                               new HandAction("matze1987", HandActionType.RAISE, 10.94m, Street.Flop,true),
                               new HandAction("numbush", HandActionType.CALL, 8.94m, Street.Flop),

                               new HandAction("numbush", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("matze1987", HandActionType.SHOW, 0, Street.Showdown),
                              
                           };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsAllInHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("numbush", WinningsActionType.WINS, 23.57m, 0) }; }
        }

        protected override List<HandAction> ExpectedHandActionsUncalledBetHand
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("here_we_gizo", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                    new HandAction("PAARTYPAN", HandActionType.BIG_BLIND, 1m, Street.Preflop),

                    new HandAction("liscla223", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("Beeethoven87", HandActionType.RAISE, 3, Street.Preflop),
                    new HandAction("Jon9ball", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("here_we_gizo", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("PAARTYPAN", HandActionType.FOLD, 0, Street.Preflop),

                    new HandAction("Beeethoven87", HandActionType.UNCALLED_BET, 2, Street.Preflop),                  
                    new HandAction("Beeethoven87", HandActionType.MUCKS,0, Street.Showdown)                
                };
            }
        }

        protected override List<WinningsAction> ExpectedWinnersHandActionsUncalledBetHand
        {
            get { return new List<WinningsAction>() { new WinningsAction("Beeethoven87", WinningsActionType.WINS, 2.50m, 0), }; }
        }

        protected override List<HandAction> ExpectedOmahaHiLoHand
        {
            get
            {
                return new List<HandAction>()
                {
                    new HandAction("ColFortune81", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                    new HandAction("Belkiss2", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                    new HandAction("ribby53", HandActionType.POSTS, 1m, Street.Preflop),

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
                return new List<WinningsAction>() {
                    new WinningsAction("Belkiss2", WinningsActionType.WINS, 1.67m, 0),
                    new WinningsAction("Belkiss2", WinningsActionType.WINS, 1.67m, 0) 
                };
            }
        }
    }
}