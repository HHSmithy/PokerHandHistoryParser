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
                               new AllInAction("killAA007",5.88m, Street.Preflop,false),
                               
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
                               new AllInAction("Amalfitano1", 19.05m, Street.Preflop,true),
                               new AllInAction("Taras2107", 1.30m, Street.Preflop, false),
                               
                               new HandAction("Amalfitano1", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("Taras2107", HandActionType.SHOW, 0, Street.Showdown),
                               new WinningsAction("Amalfitano1", HandActionType.WINS, 17.55m, 0),
                               new WinningsAction("Taras2107", HandActionType.WINS, 3.18m, 0),                                         
                           };

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

                                        new AllInAction("SSfullHH", 1.37m, Street.River,true),     
                                        new HandAction("bf324846", HandActionType.CALL, 1.37m, Street.River), 

                                        new HandAction("bf324846", HandActionType.SHOW, 0, Street.Showdown),
                                        new HandAction("SSfullHH", HandActionType.SHOW, 0, Street.Showdown),

                                        new WinningsAction("bf324846", HandActionType.WINS, 9.28m, 0)
                                    };

            TestParseActions("AllInHandWithShowdown2", expectedActions);
        }

        

        [Test]
        [Ignore("Issues with local dev environment running tests - this should work but can't verify and don't want to break builds.")]
        public void AnteParsingTest_Fixed()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("Ge007", HandActionType.ANTE, 0.02m, Street.Preflop),
                                        new HandAction("joinboy", HandActionType.ANTE, 0.02m, Street.Preflop),
                                        new HandAction("xvala", HandActionType.ANTE, 0.02m, Street.Preflop),
                                        new HandAction("StrangerFish1", HandActionType.ANTE, 0.02m, Street.Preflop),
                                        new HandAction("morphius007", HandActionType.ANTE, 0.02m, Street.Preflop),
                                        new HandAction("morphius007", HandActionType.ANTE, 0.02m, Street.Preflop),
                                        new HandAction("Ge007", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                                        new HandAction("joinboy", HandActionType.BIG_BLIND, 0.10m, Street.Preflop),
                                        new HandAction("xvala", HandActionType.FOLD, 0.00m, Street.Preflop),
                                        new HandAction("StrangerFish1", HandActionType.CALL, 0.10m, Street.Preflop),
                                        new HandAction("morphius007", HandActionType.RAISE, 0.40m - 0.02m, Street.Preflop),
                                        new HandAction("ijdnakuri", HandActionType.FOLD, 0.00m, Street.Preflop),
                                        new HandAction("Ge007", HandActionType.CALL, 0.35m, Street.Preflop),
                                        new HandAction("joinboy", HandActionType.CALL, 0.30m, Street.Preflop),
                                        new HandAction("StrangerFish1", HandActionType.CALL, 0.30m, Street.Preflop),                                      
                                        new HandAction("Ge007", HandActionType.CHECK, 0.0m, Street.Flop),                                      
                                        new HandAction("joinboy", HandActionType.CHECK, 0.0m, Street.Flop),                                      
                                        new HandAction("StrangerFish1", HandActionType.BET, 1.29m, Street.Flop),                                      
                                        new HandAction("morphius007", HandActionType.CALL, 1.29m, Street.Flop),                                      
                                        new HandAction("Ge007", HandActionType.FOLD, 0.0m, Street.Flop),                                      
                                        new HandAction("joinboy", HandActionType.FOLD, 0.0m, Street.Flop),    
                                        new HandAction("StrangerFish1", HandActionType.BET, 2.15m, Street.Turn),                                      
                                        new HandAction("morphius007", HandActionType.CALL, 2.15m, Street.Turn),                                   
                                        new HandAction("StrangerFish1", HandActionType.BET, 4.30m, Street.River),                                      
                                        new AllInAction("morphius007", 8.36m, Street.River, true),                                   
                                        new HandAction("StrangerFish1", HandActionType.BET, 4.36m, Street.River),                                      
                                        new HandAction("StrangerFish1", HandActionType.SHOW, 0m, Street.Showdown),                                   
                                        new HandAction("morphius007", HandActionType.SHOW, 0m, Street.Showdown),   
                                        new WinningsAction("StrangerFish1", HandActionType.WINS, 24.32m, 0),
                                    };

            TestParseActions("AnteAction", expectedActions);
        }
    }
}
