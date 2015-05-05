using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.HandActionTests
{
    class HandParserHandActionTestsBossMedia : HandParserHandActionTests
    {
        public HandParserHandActionTestsBossMedia()
            : base("BossMedia")
        {

        }

        protected override List<HandAction> ExpectedHandActionsBasicHand
        {
            get
            {
                return new List<HandAction>()
                                    {
                                        new HandAction("1Mentalist", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                                        new HandAction("ItalyToast", HandActionType.BIG_BLIND, 10m, Street.Preflop),

                                        new HandAction("Matte10", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("slejpner", HandActionType.CALL, 10, Street.Preflop),
                                        new HandAction("phallos", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("1Mentalist", HandActionType.CALL, 5, Street.Preflop),
                                        new HandAction("ItalyToast", HandActionType.CHECK, 0, Street.Preflop),

                                        new HandAction("1Mentalist", HandActionType.CHECK, 0, Street.Flop),                  
                                        new HandAction("ItalyToast", HandActionType.CHECK, 0, Street.Flop),
                                        new HandAction("slejpner", HandActionType.CHECK,0, Street.Flop),

                                        new HandAction("1Mentalist", HandActionType.CHECK, 0, Street.Turn),                  
                                        new HandAction("ItalyToast", HandActionType.CHECK, 0, Street.Turn),
                                        new HandAction("slejpner", HandActionType.CHECK,0, Street.Turn),

                                        new HandAction("1Mentalist", HandActionType.BET, 22.50m, Street.River),                  
                                        new HandAction("ItalyToast", HandActionType.FOLD, 0, Street.River),
                                        new HandAction("slejpner", HandActionType.FOLD,0, Street.River),

                                        new WinningsAction("1Mentalist", HandActionType.WINS, 29.26m, 0),  
                                    };
            }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                return new List<HandAction>()
                                    {
                                        new HandAction("ItalyToast", HandActionType.SMALL_BLIND, 100m, Street.Preflop),
                                        new HandAction("ToyBoy66", HandActionType.BIG_BLIND, 200m, Street.Preflop),

                                        new HandAction("ItalyToast", HandActionType.RAISE, 300, Street.Preflop),
                                        new HandAction("ToyBoy66", HandActionType.FOLD, 0, Street.Preflop),

                                        new WinningsAction("ItalyToast", HandActionType.WINS, 400m, 0),  
                                    };
            }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                return new List<HandAction>()
                                    {
                                        new HandAction("Phyre", HandActionType.SMALL_BLIND, 100m, Street.Preflop),
                                        new HandAction("AllinAnna", HandActionType.BIG_BLIND, 200m, Street.Preflop),

                                        new HandAction("ItalyToast", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("SAMERRRR", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("Supervic", HandActionType.RAISE, 400m, Street.Preflop),
                                        new HandAction("Phyre", HandActionType.RAISE, 1300m, Street.Preflop),
                                        new HandAction("AllinAnna", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("Supervic", HandActionType.CALL, 1000m, Street.Preflop),

                                        new HandAction("Phyre", HandActionType.CHECK, 0m, Street.Flop),
                                        new HandAction("Supervic", HandActionType.BET, 2200m, Street.Flop),
                                        new HandAction("Phyre", HandActionType.FOLD, 0m, Street.Flop),

                                        new WinningsAction("Supervic", HandActionType.WINS, 2985m, 0)
                                    };
            }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                return new List<HandAction>()
                                    {
                                        new HandAction("Phyre", HandActionType.SMALL_BLIND, 100m, Street.Preflop),
                                        new HandAction("AllinAnna", HandActionType.BIG_BLIND, 200m, Street.Preflop),

                                        new HandAction("ItalyToast", HandActionType.RAISE, 450m, Street.Preflop),
                                        new HandAction("SAMERRRR", HandActionType.RAISE, 1650, Street.Preflop),
                                        new HandAction("Supervic", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("Phyre", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("AllinAnna", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("ItalyToast", HandActionType.RAISE, 4800m, Street.Preflop),
                                        new HandAction("SAMERRRR", HandActionType.RAISE, 14322.51m, Street.Preflop, true),
                                        new HandAction("ItalyToast", HandActionType.CALL, 8635m, Street.Preflop, true),

                                        new WinningsAction("SAMERRRR", HandActionType.WINS, 17555m, 0),  
                                    };
            }
        }

        protected override List<HandAction> ExpectedOmahaHiLoHand
        {
            get
            {
                return new List<HandAction>()
                                    {
                                        new HandAction("Von MÃ¶gen", HandActionType.SMALL_BLIND, 2.50m, Street.Preflop),
                                        new HandAction("ItalyToast", HandActionType.BIG_BLIND, 5m, Street.Preflop),

                                        new HandAction("KrossKajsa", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("Hannes_nr1", HandActionType.RAISE, 14.37m, Street.Preflop),
                                        new HandAction("Von MÃ¶gen", HandActionType.CALL, 11.87m, Street.Preflop),
                                        new HandAction("ItalyToast", HandActionType.CALL, 9.37m, Street.Preflop),

                                        new HandAction("Von MÃ¶gen", HandActionType.CHECK, 0, Street.Flop),
                                        new HandAction("ItalyToast", HandActionType.CHECK, 0m, Street.Flop),
                                        new HandAction("Hannes_nr1", HandActionType.BET, 32.33m, Street.Flop),
                                        new HandAction("Von MÃ¶gen", HandActionType.FOLD, 0m, Street.Flop),
                                        new HandAction("ItalyToast", HandActionType.CALL, 32.33m, Street.Flop),

                                        new HandAction("ItalyToast", HandActionType.CHECK, 0m, Street.Turn),
                                        new HandAction("Hannes_nr1", HandActionType.CHECK, 0m, Street.Turn),

                                        new HandAction("ItalyToast", HandActionType.BET, 82m, Street.River),
                                        new HandAction("Hannes_nr1", HandActionType.FOLD, 0m, Street.River),

                                        new WinningsAction("ItalyToast", HandActionType.WINS, 105.08m, 0),  
                                    };
            }
        }
    }
}
