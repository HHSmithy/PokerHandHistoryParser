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
                return new List<HandAction>()
                           {
                               new HandAction("Evora__", HandActionType.SMALL_BLIND, 5, Street.Preflop),
                               new HandAction("prestonmaddo", HandActionType.BIG_BLIND, 10, Street.Preflop),
                               new HandAction("Sm1lingSnake", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Stanozololo", HandActionType.RAISE, 22, Street.Preflop),
                               new HandAction("WACONOIA", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("RunBKK", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Evora__", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("prestonmaddo", HandActionType.CALL, 12, Street.Preflop),
                               new HandAction("prestonmaddo", HandActionType.CHECK, 0, Street.Flop),
                               new HandAction("Stanozololo", HandActionType.CHECK, 0, Street.Flop),
                               new HandAction("prestonmaddo", HandActionType.CHECK, 0, Street.Turn),
                               new HandAction("Stanozololo", HandActionType.CHECK, 0, Street.Turn),
                               new HandAction("prestonmaddo", HandActionType.BET, 380, Street.River),
                               new HandAction("Stanozololo", HandActionType.FOLD, 0, Street.River),
                               new HandAction("prestonmaddo", HandActionType.MUCKS, 0, Street.River),
                               new WinningsAction("prestonmaddo", HandActionType.WINS, 426.55m, 0),
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("l_ . - l - . _l", HandActionType.SMALL_BLIND, 1, Street.Preflop),
                               new HandAction("feretboy666", HandActionType.BIG_BLIND, 2, Street.Preflop),
                               new HandAction("Buddy Guy", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("erttre99", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("wlips", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Pumwaree", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("l_ . - l - . _l", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("feretboy666", HandActionType.MUCKS, 0, Street.Preflop),
                               new WinningsAction("feretboy666", HandActionType.WINS, 3,  0),                              
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                return new List<HandAction>()
                                      {
                                          new HandAction("ElvisViking", HandActionType.SMALL_BLIND, 5, Street.Preflop),
                                          new HandAction("Zl0iman", HandActionType.BIG_BLIND, 10, Street.Preflop),
                                          new HandAction("HPS1964", HandActionType.RAISE, 20, Street.Preflop),
                                          new HandAction("BRNS16", HandActionType.FOLD, 0, Street.Preflop),
                                          new HandAction("DoNDeRei", HandActionType.FOLD, 0, Street.Preflop),
                                          new HandAction("poker446822", HandActionType.FOLD, 0, Street.Preflop),
                                          new HandAction("ElvisViking", HandActionType.CALL, 15, Street.Preflop),
                                          new HandAction("Zl0iman", HandActionType.RAISE, 60, Street.Preflop),
                                          new HandAction("HPS1964", HandActionType.CALL, 50, Street.Preflop),
                                          new HandAction("ElvisViking", HandActionType.CALL, 50, Street.Preflop),
                                          new HandAction("ElvisViking", HandActionType.CHECK, 0, Street.Flop),
                                          new HandAction("Zl0iman", HandActionType.CHECK, 0, Street.Flop),
                                          new HandAction("HPS1964", HandActionType.BET, 103.50m, Street.Flop),
                                          new HandAction("ElvisViking", HandActionType.RAISE, 207, Street.Flop),
                                          new HandAction("Zl0iman", HandActionType.FOLD, 0, Street.Flop),
                                          new HandAction("HPS1964", HandActionType.CALL, 103.50m, Street.Flop),
                                          new HandAction("ElvisViking", HandActionType.CHECK, 0, Street.Turn),
                                          new HandAction("HPS1964", HandActionType.CHECK, 0, Street.Turn),
                                          new HandAction("ElvisViking", HandActionType.CHECK, 0, Street.River),
                                          new HandAction("HPS1964", HandActionType.CHECK, 0, Street.River),
                                          new HandAction("ElvisViking", HandActionType.SHOW, 0, Street.River),
                                          new HandAction("HPS1964", HandActionType.SHOW, 0, Street.River),
                                          new WinningsAction("HPS1964", HandActionType.WINS, 621, 0),
                                      };
            }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                return new List<HandAction>()
                                      {
                                          new HandAction("Slask17", HandActionType.SMALL_BLIND, 5, Street.Preflop),
                                          new HandAction("WillmaF1cken", HandActionType.BIG_BLIND, 10, Street.Preflop),
                                          new HandAction("Slask17", HandActionType.RAISE, 20, Street.Preflop),
                                          new HandAction("WillmaF1cken", HandActionType.RAISE, 80, Street.Preflop),
                                          new HandAction("Slask17", HandActionType.RAISE, 195, Street.Preflop),
                                          new AllInAction("WillmaF1cken", 3341.11m, Street.Preflop, true),
                                          new AllInAction("Slask17", 978.95m, Street.Preflop, true),
                                          new HandAction("Slask17", HandActionType.SHOW, 0, Street.River),
                                          new HandAction("WillmaF1cken", HandActionType.SHOW, 0, Street.River),
                                          new WinningsAction("WillmaF1cken", HandActionType.WINS_SIDE_POT, 2232.16m,  1),
                                          new WinningsAction("WillmaF1cken", HandActionType.WINS, 2396.90m, 0),
                                      };
            }
        }

        protected override List<HandAction> ExpectedOmahaHiLoHand
        {
            get
            {
                return new List<HandAction>()
                                      {
                                          new HandAction("Scoopydoo81", HandActionType.SMALL_BLIND, 2, Street.Preflop),
                                          new HandAction("janusfaced", HandActionType.BIG_BLIND, 4, Street.Preflop),                                                                      new HandAction("wembrinator", HandActionType.FOLD, 0, Street.Preflop),
                                          new HandAction("robertp10", HandActionType.FOLD, 0, Street.Preflop),
                                          new HandAction("Scoopydoo81", HandActionType.CALL, 2, Street.Preflop),
                                          new HandAction("janusfaced", HandActionType.CHECK, 0, Street.Preflop),
                                          new HandAction("Scoopydoo81", HandActionType.CHECK, 0, Street.Flop),
                                          new HandAction("janusfaced", HandActionType.CHECK, 0, Street.Flop),
                                          new HandAction("Scoopydoo81", HandActionType.CHECK, 0, Street.Turn),
                                          new HandAction("janusfaced", HandActionType.CHECK, 0, Street.Turn),
                                          new HandAction("Scoopydoo81", HandActionType.CHECK, 0, Street.River),
                                          new HandAction("janusfaced", HandActionType.CHECK, 0, Street.River),
                                          new HandAction("Scoopydoo81", HandActionType.SHOW, 0, Street.River),
                                          new HandAction("Scoopydoo81", HandActionType.SHOWS_FOR_LOW, 0, Street.River),
                                          new HandAction("janusfaced", HandActionType.SHOW, 0, Street.River),
                                          new WinningsAction("janusfaced", HandActionType.WINS, 3.80m, 0),
                                          new WinningsAction("Scoopydoo81", HandActionType.WINS_THE_LOW, 3.80m, 0),
                                      };
            }
        }
    }
}