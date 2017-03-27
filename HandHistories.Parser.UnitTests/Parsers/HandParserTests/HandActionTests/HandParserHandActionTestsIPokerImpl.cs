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
    class HandParserHandActionTestsIPokerImpl : HandParserHandActionTests
    {
        public HandParserHandActionTestsIPokerImpl() : base("IPoker")
        {
        }

        protected override List<HandAction> ExpectedHandActionsBasicHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("joemags", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                               new HandAction("Dullaghan", HandActionType.BIG_BLIND, 0.1m, Street.Preflop),
                               new HandAction("Frozean", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("joemags", HandActionType.CALL, 0.05m, Street.Preflop),
                               new HandAction("Dullaghan", HandActionType.CHECK, 0m, Street.Preflop),

                               new HandAction("joemags", HandActionType.BET, 0.1m, Street.Flop),
                               new HandAction("Dullaghan", HandActionType.RAISE, 0.2m, Street.Flop),
                               new HandAction("joemags", HandActionType.CALL, 0.1m, Street.Flop),

                               new HandAction("joemags", HandActionType.CHECK, 0m, Street.Turn),
                               new HandAction("Dullaghan", HandActionType.BET, 0.2m, Street.Turn),
                               new HandAction("joemags", HandActionType.CALL, 0.2m, Street.Turn),

                               new HandAction("joemags", HandActionType.BET, 0.20m, Street.River),
                               new HandAction("Dullaghan", HandActionType.RAISE, 0.5m, Street.River),
                               new HandAction("joemags", HandActionType.FOLD, 0m, Street.River),

                               new WinningsAction("Dullaghan", HandActionType.WINS, 1.61m, 0),                               
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("Frozean", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                               new HandAction("Blemishers", HandActionType.BIG_BLIND, 0.1m, Street.Preflop),
                               new HandAction("17111982", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("SongOfIceAndFire", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Frozean", HandActionType.FOLD, 0, Street.Preflop),
                               new WinningsAction("Blemishers", HandActionType.WINS, 0.15m, 0),                               
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("Amalfitano1", HandActionType.ANTE, 0.02m, Street.Preflop),
                               new HandAction("killAA007", HandActionType.ANTE, 0.02m, Street.Preflop),
                               new HandAction("Amalfitano1", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                               new HandAction("killAA007", HandActionType.BIG_BLIND, 0.10m, Street.Preflop),
                               new HandAction("Amalfitano1", HandActionType.RAISE, 0.35m, Street.Preflop),
                               new HandAction("killAA007", HandActionType.RAISE, 1.10m, Street.Preflop),
                               new HandAction("Amalfitano1",HandActionType.RAISE, 11.70m, Street.Preflop),
                               new HandAction("killAA007",HandActionType.CALL, 5.88m, Street.Preflop, true),
                               
                               new HandAction("Amalfitano1", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("killAA007", HandActionType.SHOW, 0, Street.Showdown),
                               new WinningsAction("Amalfitano1", HandActionType.WINS, 18.28m, 0),                               
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("Amalfitano1", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                               new HandAction("Taras2107", HandActionType.BIG_BLIND, 0.1m, Street.Preflop),
                               new HandAction("Amalfitano1", HandActionType.RAISE, 0.15m, Street.Preflop),
                               new HandAction("Taras2107", HandActionType.RAISE, 0.30m, Street.Preflop),
                               new HandAction("Amalfitano1", HandActionType.RAISE, 19.05m, Street.Preflop, true),
                               new HandAction("Taras2107", HandActionType.CALL, 1.30m, Street.Preflop, true),
                               
                               new HandAction("Amalfitano1", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("Taras2107", HandActionType.SHOW, 0, Street.Showdown),
                               new WinningsAction("Amalfitano1", HandActionType.WINS, 17.55m, 0),
                               new WinningsAction("Taras2107", HandActionType.WINS, 3.18m, 0),                                         
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

        protected override List<HandAction> ExpectedOmahaHiLoHand
        {
            get
            {
                Assert.Ignore("No Omaha Hi-Lo Testcase");
                throw new NotImplementedException();
            }
        }

        [Test]
        public void AllInHand_WithAction7_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("bf324846", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                                        new HandAction("SSfullHH", HandActionType.BIG_BLIND, 0.10m, Street.Preflop),
                                        new HandAction("bf324846", HandActionType.RAISE, 0.25m, Street.Preflop),
                                        new HandAction("SSfullHH", HandActionType.RAISE, 0.4m, Street.Preflop),
                                        new HandAction("bf324846", HandActionType.CALL, 0.2m, Street.Preflop),

                                        new HandAction("SSfullHH", HandActionType.CHECK, 0m, Street.Flop),
                                        new HandAction("bf324846", HandActionType.BET, 0.7m, Street.Flop),
                                        new HandAction("SSfullHH", HandActionType.CALL, 0.7m, Street.Flop),
                                      
                                        new HandAction("SSfullHH", HandActionType.BET, 2.40m, Street.Turn),
                                        new HandAction("bf324846", HandActionType.CALL, 2.40m, Street.Turn),

                                        new HandAction("SSfullHH", HandActionType.BET, 1.37m, Street.River, true),     
                                        new HandAction("bf324846", HandActionType.CALL, 1.37m, Street.River), 

                                        new HandAction("bf324846", HandActionType.SHOW, 0, Street.Showdown),
                                        new HandAction("SSfullHH", HandActionType.SHOW, 0, Street.Showdown),

                                        new WinningsAction("bf324846", HandActionType.WINS, 9.28m, 0)
                                    };

            TestParseActions("AllInHandWithShowdown2", expectedActions);
        }

        

        [Test]
        //[Ignore("Issues with local dev environment running tests - this should work but can't verify and don't want to break builds.")]
        public void AnteParsingTest_Fixed()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("WWR141388412", HandActionType.ANTE, 0.02m, Street.Preflop),
                                        new HandAction("keepfishing68", HandActionType.ANTE, 0.02m, Street.Preflop),
                                        new HandAction("chcake515151", HandActionType.ANTE, 0.02m, Street.Preflop),
                                        new HandAction("WWR141388412", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                                        new HandAction("keepfishing68", HandActionType.BIG_BLIND, 0.10m, Street.Preflop),

                                        new HandAction("chcake515151", HandActionType.FOLD, 0.00m, Street.Preflop),
                                        new HandAction("WWR141388412", HandActionType.CALL, 0.05m, Street.Preflop),
                                        new HandAction("keepfishing68", HandActionType.CHECK, 0.0m, Street.Preflop),
                                    
                                        new HandAction("WWR141388412", HandActionType.CHECK, 0.0m, Street.Flop),                                      
                                        new HandAction("keepfishing68", HandActionType.CHECK, 0.0m, Street.Flop),  

                                        new HandAction("WWR141388412", HandActionType.CHECK, 0m, Street.Turn),                                      
                                        new HandAction("keepfishing68", HandActionType.CHECK, 0m, Street.Turn),   
                                
                                        new HandAction("WWR141388412", HandActionType.BET, 0.1m, Street.River),                                      
                                        new HandAction("keepfishing68", HandActionType.FOLD, 0m, Street.River),
                                     
                                        //new HandAction("WWR141388412", HandActionType.SHOW, 0m, Street.Showdown),                                   
                                        //new HandAction("keepfishing68", HandActionType.SHOW, 0m, Street.Showdown),   
                                        new WinningsAction("WWR141388412", HandActionType.WINS, 0.35m, 0),
                                    };

            TestParseActions("AnteAction", expectedActions);
        }

        [Test]
        //[Ignore("Issues with local dev environment running tests - this should work but can't verify and don't want to break builds.")]
        public void PostingDeadParsingTest_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        
                                        new HandAction("Player5", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                                        new HandAction("HERO", HandActionType.BIG_BLIND, 10m, Street.Preflop),
                                        new HandAction("Player1", HandActionType.POSTS, 10m, Street.Preflop),
                                        new HandAction("Player1", HandActionType.POSTS_DEAD, 5m, Street.Preflop),

                                        new HandAction("Player8", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("Player10", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("Player1", HandActionType.CHECK, 0m, Street.Preflop),
                                        new HandAction("Player3", HandActionType.FOLD, 0m, Street.Preflop),                                      
                                        new HandAction("Player5", HandActionType.CALL, 5m, Street.Preflop),  
                                        new HandAction("HERO", HandActionType.CHECK, 0m, Street.Preflop),  

                                        new HandAction("Player5", HandActionType.BET, 24.50m, Street.Flop),                                      
                                        new HandAction("HERO", HandActionType.CALL, 24.50m, Street.Flop),   
                                        new HandAction("Player1", HandActionType.FOLD, 0m, Street.Flop),   

                                        new HandAction("Player5", HandActionType.BET, 60m, Street.Turn),                                      
                                        new HandAction("HERO", HandActionType.FOLD, 0m, Street.Turn),
                                     
                                        //new HandAction("WWR141388412", HandActionType.SHOW, 0m, Street.Showdown),                                   
                                        //new HandAction("keepfishing68", HandActionType.SHOW, 0m, Street.Showdown),   
                                        new WinningsAction("Player5", HandActionType.WINS, 140m, 0),
                                    };

            TestParseActions("PostingDead", expectedActions);
        }
    }
}
