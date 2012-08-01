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
        public void StrangePlayerNames_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("fless836", HandActionType.SMALL_BLIND, 0.10m, Street.Preflop),
                                        new HandAction("wo_olly :D", HandActionType.BIG_BLIND, 0.25m, Street.Preflop),
                                        new HandAction("Mr.Negros", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("Khruchshev", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("TomiStars09", HandActionType.CALL, 0.25m, Street.Preflop),
                                        new HandAction("timido73", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("ribonka", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("mcZAG", HandActionType.FOLD, 0, Street.Preflop),                  
                                        new HandAction("Samushura", HandActionType.FOLD, 0, Street.Preflop),   
                                        new HandAction("fless836", HandActionType.FOLD, 0, Street.Preflop),   
                                        new HandAction("wo_olly :D", HandActionType.CHECK, 0, Street.Preflop),
                                        new HandAction("wo_olly :D", HandActionType.CHECK, 0, Street.Flop),
                                        new HandAction("TomiStars09", HandActionType.CHECK, 0, Street.Flop),
                                        new HandAction("wo_olly :D", HandActionType.CHECK, 0, Street.Turn),
                                        new HandAction("TomiStars09", HandActionType.CHECK, 0, Street.Turn),
                                        new HandAction("wo_olly :D", HandActionType.CHECK, 0, Street.River),
                                        new HandAction("TomiStars09", HandActionType.CHECK, 0, Street.River),
                                        new HandAction("wo_olly :D", HandActionType.SHOW, 0, Street.Showdown),
                                        new HandAction("TomiStars09", HandActionType.MUCKS, 0, Street.Showdown),
                                        new WinningsAction("wo_olly :D", HandActionType.WINS, 0.57m, 0),
                                    };

            TestParseActions("StrangePlayerNames", expectedActions);
        }

        [Test]
        public void UncalledBet_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("Playboyskien", HandActionType.SMALL_BLIND, 3, Street.Preflop),
                                        new HandAction("zarifula", HandActionType.BIG_BLIND, 6, Street.Preflop),
                                        new HandAction("$hark2u", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("yuginZu", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("Gith", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("STATUS QUO V", HandActionType.RAISE, 15, Street.Preflop),
                                        new HandAction("Playboyskien", HandActionType.CALL, 12m, Street.Preflop),
                                        new HandAction("zarifula", HandActionType.FOLD, 0, Street.Preflop),                                      
                                        new HandAction("Playboyskien", HandActionType.CHECK, 0, Street.Flop),
                                        new HandAction("STATUS QUO V", HandActionType.BET, 21.32m, Street.Flop),
                                        new AllInAction("Playboyskien", 259.68m, Street.Flop, true),
                                        new HandAction("STATUS QUO V", HandActionType.FOLD, 0, Street.Flop),
                                        new HandAction("Playboyskien", HandActionType.UNCALLED_BET, 238.36m, Street.Flop),

                                        new WinningsAction("Playboyskien", HandActionType.WINS, 75.84m, 0),
                                        new HandAction("Playboyskien", HandActionType.MUCKS, 0, Street.Showdown),
                                        
                                    };

            TestParseActions("UncalledBet", expectedActions);
        }

        [Test]
        public void AllInHand_NeedsRaiseAdjusting_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("Zypherin", HandActionType.SMALL_BLIND, 50m, Street.Preflop),
                                        new HandAction("LuckyGump", HandActionType.BIG_BLIND, 100m, Street.Preflop),
                                        new HandAction("Jeans89", HandActionType.ANTE, 20m, Street.Preflop),
                                        new HandAction("MrSmits", HandActionType.ANTE, 20m, Street.Preflop),
                                        new HandAction("Vaga_Lion", HandActionType.ANTE, 20m, Street.Preflop),
                                        new HandAction("mikki696", HandActionType.ANTE, 20m, Street.Preflop),
                                        new HandAction("Zypherin", HandActionType.ANTE, 20m, Street.Preflop),
                                        new HandAction("LuckyGump", HandActionType.ANTE, 20m, Street.Preflop),                                      
                                        new HandAction("Jeans89", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("MrSmits", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("Vaga_Lion", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("mikki696", HandActionType.RAISE, 280m, Street.Preflop),
                                        new HandAction("Zypherin", HandActionType.CALL, 230m, Street.Preflop),
                                        new HandAction("LuckyGump", HandActionType.RAISE, 1240m - 100m, Street.Preflop),
                                        new HandAction("mikki696", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("Zypherin", HandActionType.CALL, 960m, Street.Preflop),

                                        new HandAction("Zypherin", HandActionType.CHECK, 0m, Street.Flop),
                                        new HandAction("LuckyGump", HandActionType.BET, 1900m, Street.Flop),
                                        new HandAction("Zypherin", HandActionType.RAISE, 8300m, Street.Flop),
                                        new HandAction("LuckyGump", HandActionType.RAISE, 27775m - 1900m, Street.Flop),
                                        new AllInAction("Zypherin", 34894.72m - 8300m, Street.Flop, true),
                                        new HandAction("LuckyGump", HandActionType.CALL, 7119.72m, Street.Flop),
                                        
                                        new HandAction("Zypherin", HandActionType.SHOW, 0, Street.Showdown),
                                        new HandAction("LuckyGump", HandActionType.SHOW, 0, Street.Showdown),
                                        new WinningsAction("Zypherin", HandActionType.WINS, 72664.44m, 0),
                                        
                                    };

            TestParseActions("NeedsRaiseAdjusting", expectedActions);
        }

        [Test]
        public void AllInHand_NeedsRaiseAdjusting_BigBlindOptionRaisesOption_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("mypokerf", HandActionType.SMALL_BLIND, 10m, Street.Preflop),
                                        new HandAction("brtnboarder", HandActionType.BIG_BLIND, 20m, Street.Preflop),
                                        new HandAction("mypokerf", HandActionType.CALL, 10m, Street.Preflop),
                                        new HandAction("brtnboarder", HandActionType.RAISE, 80m, Street.Preflop),
                                        new HandAction("mypokerf", HandActionType.RAISE, 200m, Street.Preflop),
                                        new AllInAction("brtnboarder", 1813.65m - 100m, Street.Preflop, true),
                                        new HandAction("mypokerf", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("brtnboarder", HandActionType.UNCALLED_BET, 1593.65m, Street.Preflop),
                                        new WinningsAction("brtnboarder", HandActionType.WINS, 440m, 0),
                                        new HandAction("brtnboarder", HandActionType.MUCKS, 0, Street.Showdown)
                                    };

            TestParseActions("BigBlindOptionRaisesOption", expectedActions);
        }

        protected override List<HandAction> ExpectedHandActionsBasicHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("retic77", HandActionType.SMALL_BLIND, 0.10m, Street.Preflop),
                               new HandAction("Domus84", HandActionType.BIG_BLIND, 0.25m, Street.Preflop),
                                new HandAction("CAMARITA", HandActionType.ANTE, 0.05m, Street.Preflop),
                                new HandAction("Desert2DLX", HandActionType.ANTE, 0.05m, Street.Preflop),
                                new HandAction("DallasAP2", HandActionType.ANTE, 0.05m, Street.Preflop),
                                new HandAction("nemotoha", HandActionType.ANTE, 0.05m, Street.Preflop),
                                new HandAction("retic77", HandActionType.ANTE, 0.05m, Street.Preflop),
                                new HandAction("Domus84", HandActionType.ANTE, 0.05m, Street.Preflop),
                                new HandAction("MECO-LEO", HandActionType.ANTE, 0.05m, Street.Preflop),
                                new HandAction("Lillenman", HandActionType.ANTE, 0.05m, Street.Preflop),


                               new HandAction("MECO-LEO", HandActionType.CALL, 0.25m, Street.Preflop),
                               new HandAction("Lillenman", HandActionType.CALL, 0.25m, Street.Preflop),
                               new HandAction("CAMARITA", HandActionType.CALL, 0.25m, Street.Preflop),
                               new HandAction("Desert2DLX", HandActionType.CALL, 0.25m, Street.Preflop),
                               new HandAction("DallasAP2", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("nemotoha", HandActionType.CALL, 0.25m, Street.Preflop),
                               new HandAction("retic77", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("Domus84", HandActionType.CHECK, 0m, Street.Preflop),

                               new HandAction("Domus84", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("MECO-LEO", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("Lillenman", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("CAMARITA", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("Desert2DLX", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("nemotoha", HandActionType.CHECK, 0m, Street.Flop),

                                new HandAction("Domus84", HandActionType.BET, 1m, Street.Turn),
                                new HandAction("MECO-LEO", HandActionType.CALL, 1m, Street.Turn),
                                new HandAction("Lillenman", HandActionType.CALL, 1m, Street.Turn),
                                new HandAction("CAMARITA", HandActionType.CALL, 1m, Street.Turn),
                                new HandAction("Desert2DLX", HandActionType.CALL, 1m, Street.Turn),
                                new HandAction("nemotoha", HandActionType.FOLD, 0m, Street.Turn),

                               new HandAction("Domus84", HandActionType.CHECK, 0m, Street.River),
                               new HandAction("MECO-LEO", HandActionType.BET, 1m, Street.River),
                               new HandAction("Lillenman", HandActionType.FOLD, 0m, Street.River),
                               new HandAction("CAMARITA", HandActionType.FOLD, 0m, Street.River),
                               new HandAction("Desert2DLX", HandActionType.FOLD, 0m, Street.River),
                               new HandAction("Domus84", HandActionType.FOLD, 0m, Street.River),

                               new HandAction("MECO-LEO", HandActionType.UNCALLED_BET, 1, Street.River),
                               new WinningsAction("MECO-LEO", HandActionType.WINS, 6.68m, 0),
                               new HandAction("MECO-LEO", HandActionType.MUCKS, 0, Street.Showdown)
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("bingo185", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                               new HandAction("gaydaddy", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                               new HandAction("flipvampir", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("chrisvb75", HandActionType.RAISE, 3, Street.Preflop),
                               new HandAction("woezelenpip", HandActionType.RAISE, 9, Street.Preflop),
                               new HandAction("bingo185", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("gaydaddy", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("chrisvb75", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("woezelenpip", HandActionType.UNCALLED_BET, 6, Street.Preflop),
                               new WinningsAction("woezelenpip", HandActionType.WINS, 7.50m, 0),
                               new HandAction("woezelenpip", HandActionType.MUCKS, 0, Street.Showdown)
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("RECHUK", HandActionType.SMALL_BLIND, 0.08m, Street.Preflop),
                               new HandAction("Fjell_konge", HandActionType.BIG_BLIND, 0.16m, Street.Preflop),
                               new HandAction("Piotr280688", HandActionType.RAISE, 0.64m, Street.Preflop),
                               new HandAction("MS13ZEN", HandActionType.RAISE, 2, Street.Preflop),
                               new HandAction("newlander911", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("hefeplinz4:20pm", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("RECHUK", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Fjell_konge", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Piotr280688", HandActionType.CALL, 1.36m, Street.Preflop),
                               new HandAction("Piotr280688", HandActionType.CHECK, 0, Street.Flop),
                               new HandAction("MS13ZEN", HandActionType.BET, 1.76m, Street.Flop),
                               new HandAction("Piotr280688", HandActionType.CALL, 1.76m, Street.Flop),
                               new HandAction("Piotr280688", HandActionType.CHECK, 0, Street.Turn),
                               new HandAction("MS13ZEN", HandActionType.BET, 4.56m, Street.Turn),
                               new AllInAction("Piotr280688", 12.88m, Street.Turn, true),
                               new HandAction("MS13ZEN", HandActionType.FOLD, 0m, Street.Turn),
                               new HandAction("Piotr280688", HandActionType.UNCALLED_BET, 8.32m, Street.Turn),
                               new WinningsAction("Piotr280688", HandActionType.WINS, 16.12m, 0),
                               new HandAction("Piotr280688", HandActionType.MUCKS, 0, Street.Showdown)
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("niletamei", HandActionType.SMALL_BLIND, 0.08m, Street.Preflop),
                               new HandAction("RECHUK", HandActionType.BIG_BLIND, 0.16m, Street.Preflop),
                               new HandAction("Fjell_konge", HandActionType.RAISE, 0.48m, Street.Preflop),
                               new HandAction("Piotr280688", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("MS13ZEN", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("niletamei", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("RECHUK", HandActionType.CALL, 0.32m, Street.Preflop),
                               
                               new HandAction("RECHUK", HandActionType.CHECK, 0, Street.Flop),
                               new HandAction("Fjell_konge", HandActionType.BET, 0.48m, Street.Flop),
                               new HandAction("RECHUK", HandActionType.RAISE, 1.45m, Street.Flop),
                               new HandAction("Fjell_konge", HandActionType.RAISE, 4.32m - 0.48m, Street.Flop),
                               new HandAction("RECHUK", HandActionType.CALL, 2.87m, Street.Flop),

                               new HandAction("RECHUK", HandActionType.CHECK, 0m, Street.Turn),
                               new HandAction("Fjell_konge", HandActionType.BET, 5.44m, Street.Turn),
                               new AllInAction("RECHUK", 22.21m, Street.Turn, true),
                               new AllInAction("Fjell_konge", 7.56m, Street.Turn, false),


                               new HandAction("RECHUK", HandActionType.UNCALLED_BET, 9.21m, Street.Turn),
                               new HandAction("RECHUK", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("Fjell_konge", HandActionType.MUCKS, 0, Street.Showdown),
                               new WinningsAction("RECHUK", HandActionType.WINS, 34.18m, 0)
                           };
            }
        }

        protected override List<HandAction> ExpectedOmahaHiLoHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("mickeyr777", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                               new HandAction("JokerTKD", HandActionType.BIG_BLIND, 0.10m, Street.Preflop),
                               new HandAction("DOT19", HandActionType.RAISE, 0.35m, Street.Preflop),
                               new HandAction("Guffings", HandActionType.CALL, 0.35m, Street.Preflop),
                               new HandAction("HELVER4728", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("mickeyr777", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("JokerTKD", HandActionType.RAISE, 1.35m, Street.Preflop),
                               new HandAction("DOT19", HandActionType.CALL, 1.10m, Street.Preflop),
                               new HandAction("Guffings", HandActionType.CALL, 1.10m, Street.Preflop),
                               
                               new HandAction("JokerTKD", HandActionType.BET, 4.20m, Street.Flop),
                               new HandAction("DOT19", HandActionType.RAISE, 16.80m, Street.Flop),
                               new HandAction("Guffings", HandActionType.FOLD, 0, Street.Flop),
                               new AllInAction("JokerTKD",  6.40m, Street.Flop, false),
                               new HandAction("DOT19", HandActionType.UNCALLED_BET, 6.20m, Street.Flop),
                          
                               new HandAction("JokerTKD", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("DOT19", HandActionType.SHOW, 0, Street.Showdown),
                               new WinningsAction("DOT19", HandActionType.WINS, 24.45m, 0)
                           };
            }
        }
    }
}