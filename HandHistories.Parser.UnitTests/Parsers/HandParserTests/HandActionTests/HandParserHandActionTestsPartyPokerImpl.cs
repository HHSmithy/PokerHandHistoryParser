using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.HandActionTests
{
    [TestFixture]
    class HandParserHandActionTestsPartyPokerImpl : HandParserHandActionTests
    {
        public HandParserHandActionTestsPartyPokerImpl() : base("PartyPoker")
        {
        }

        protected override List<HandAction> ExpectedHandActionsBasicHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("jott1982", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                               new HandAction("sawas222", HandActionType.BIG_BLIND, 0.10m, Street.Preflop),
                               new HandAction("Sajator", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("jott1982", HandActionType.CALL, 0.05m, Street.Preflop),
                               new HandAction("sawas222", HandActionType.CHECK, 0, Street.Preflop),
                               new HandAction("jott1982", HandActionType.CHECK, 0, Street.Flop),
                               new HandAction("sawas222", HandActionType.CHECK, 0, Street.Flop),
                               new HandAction("jott1982", HandActionType.CHECK, 0, Street.Turn),
                               new HandAction("sawas222", HandActionType.CHECK, 0, Street.Turn),
                               new HandAction("jott1982", HandActionType.CHECK, 0, Street.River),
                               new HandAction("sawas222", HandActionType.CHECK, 0, Street.River),
                               new HandAction("jott1982", HandActionType.SHOW, 0, Street.River),
                               new HandAction("sawas222", HandActionType.SHOW, 0, Street.River),
                               new WinningsAction("jott1982", HandActionType.WINS, 0.19m, 0),
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("Asmodei666", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                               new HandAction("udob57", HandActionType.BIG_BLIND, 0.1m, Street.Preflop),
                               new HandAction("Asmodei666", HandActionType.FOLD, 0, Street.Preflop),

                         
                               new HandAction("udob57", HandActionType.MUCKS, 0m,  Street.Preflop),   
                               new WinningsAction("udob57", HandActionType.WINS, 0.15m,  0),                              
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                return new List<HandAction>()
                                      {
                                          new HandAction("Kelevra_91", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                                          new HandAction("hulkhoden1969", HandActionType.BIG_BLIND, 0.1m, Street.Preflop),
                                          new HandAction("Kelevra_91", HandActionType.RAISE, 0.25m, Street.Preflop),
                                          new HandAction("hulkhoden1969", HandActionType.RAISE, 0.7m, Street.Preflop),
                                          new HandAction("Kelevra_91", HandActionType.CALL, 0.5m, Street.Preflop),

                                          new HandAction("hulkhoden1969", HandActionType.BET, 0.8m, Street.Flop),
                                          new HandAction("Kelevra_91", HandActionType.RAISE, 2.94m, Street.Flop),
                                          new HandAction("hulkhoden1969", HandActionType.FOLD, 0, Street.Flop),

                                          new HandAction("Kelevra_91", HandActionType.MUCKS,0,Street.Flop),

                                          new WinningsAction("Kelevra_91", HandActionType.WINS, 5.18m, 0),
                                      };
            }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                return new List<HandAction>()
                                      {
                                          new HandAction("kewen", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                                          new HandAction("xyxyTiltyxyx", HandActionType.BIG_BLIND, 0.1m, Street.Preflop),
                                          new HandAction("GrindAA", HandActionType.FOLD, 0, Street.Preflop),
                                          new HandAction("mumija80", HandActionType.FOLD, 0, Street.Preflop),
                                          new HandAction("dr. spaz", HandActionType.RAISE, 0.3m, Street.Preflop),
                                          new HandAction("TYRANNOMAN1", HandActionType.CALL, 0.3m, Street.Preflop),
                                          new HandAction("kewen", HandActionType.CALL, 0.25m, Street.Preflop),
                                          new HandAction("xyxyTiltyxyx", HandActionType.CALL, 0.2m, Street.Preflop),

                                          new HandAction("kewen", HandActionType.CHECK, 0, Street.Flop),
                                          new HandAction("xyxyTiltyxyx", HandActionType.CHECK, 0, Street.Flop),
                                          new HandAction("dr. spaz", HandActionType.BET, 1.10m, Street.Flop),
                                          new HandAction("TYRANNOMAN1", HandActionType.CALL, 1.10m, Street.Flop),
                                          new HandAction("kewen", HandActionType.FOLD, 0, Street.Flop),
                                          new HandAction("xyxyTiltyxyx", HandActionType.CALL, 1.10m, Street.Flop),

                                          new HandAction("xyxyTiltyxyx", HandActionType.CHECK, 0, Street.Turn),
                                          new HandAction("dr. spaz", HandActionType.BET, 3.70m, Street.Turn),
                                          new HandAction("TYRANNOMAN1", HandActionType.CALL, 3.70m, Street.Turn),
                                          new HandAction("xyxyTiltyxyx", HandActionType.CALL, 3.70m, Street.Turn),
                                          
                                          new HandAction("xyxyTiltyxyx", HandActionType.CHECK, 0m, Street.River),
                                          new AllInAction("dr. spaz", 4.90m, Street.River, true),
                                          new AllInAction("TYRANNOMAN1", 3.03m, Street.River, true),
                                          new HandAction("xyxyTiltyxyx", HandActionType.FOLD, 0m, Street.River),
                                          
                                          new HandAction("dr. spaz", HandActionType.SHOW, 0m, Street.River),
                                          new HandAction("TYRANNOMAN1", HandActionType.SHOW, 0m, Street.River),
                                          
                                          new WinningsAction("dr. spaz", HandActionType.WINS_SIDE_POT, 1.87m,  1),
                                          new WinningsAction("dr. spaz", HandActionType.WINS, 20.66m, 0),
                                      };
            }
        }

        protected override List<HandAction> ExpectedOmahaHiLoHand
        {
            get
            {
                return new List<HandAction>()
                                      {
                                          new HandAction("annajulia222", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                                          new HandAction("fistfock123", HandActionType.BIG_BLIND, 1m, Street.Preflop),                                                                      
                                          new HandAction("prinzgoldi", HandActionType.FOLD, 0, Street.Preflop),
                                          new HandAction("berndi1488", HandActionType.FOLD, 0, Street.Preflop),
                                          new HandAction("robertp10", HandActionType.RAISE, 3.50m, Street.Preflop),
                                          new HandAction("annajulia222", HandActionType.FOLD, 0, Street.Preflop),
                                          new HandAction("fistfock123", HandActionType.RAISE, 10m, Street.Preflop),
                                          new HandAction("robertp10", HandActionType.RAISE, 27.50m, Street.Preflop),
                                          new AllInAction("fistfock123", 19, Street.Preflop, true),
                                          

                                          new HandAction("robertp10", HandActionType.SHOW, 0, Street.River),
                                          new HandAction("fistfock123", HandActionType.SHOW, 0, Street.River),
                                          new HandAction("fistfock123", HandActionType.SHOWS_FOR_LOW, 0, Street.River),
                                          
                                          new WinningsAction("robertp10", HandActionType.WINS_SIDE_POT, 1m, 1),
                                          new WinningsAction("robertp10", HandActionType.WINS, 28.75m, 0),
                                          new WinningsAction("fistfock123", HandActionType.WINS_THE_LOW, 28.75m, 0),
                                      };
            }
        }
    }
}