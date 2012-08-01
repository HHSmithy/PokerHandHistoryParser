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
                               new HandAction("yrrrhh33", HandActionType.SMALL_BLIND, 1m, Street.Preflop),
                               new HandAction("lhjynfobn", HandActionType.BIG_BLIND, 2m, Street.Preflop),
                               new HandAction("igalo1979", HandActionType.CALL, 2m, Street.Preflop),
                               new HandAction("YienRang", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("lillil32", HandActionType.RAISE, 6m, Street.Preflop),
                               new HandAction("pepealas5", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("yrrrhh33", HandActionType.CALL, 5m, Street.Preflop),
                               new HandAction("lhjynfobn", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("igalo1979", HandActionType.CALL, 4m, Street.Preflop),
                               new HandAction("yrrrhh33", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("igalo1979", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("lillil32", HandActionType.BET, 18m, Street.Flop),
                               new HandAction("yrrrhh33", HandActionType.FOLD, 0m, Street.Flop),
                               new HandAction("igalo1979", HandActionType.CALL, 18m, Street.Flop),
                               new HandAction("igalo1979", HandActionType.CHECK, 0m, Street.Turn),
                               new HandAction("lillil32", HandActionType.CHECK, 0m, Street.Turn),
                               new HandAction("igalo1979", HandActionType.CHECK, 0m, Street.River),
                               new HandAction("lillil32", HandActionType.BET, 30m, Street.River),
                               new HandAction("igalo1979", HandActionType.CALL, 30m, Street.River),
                               new HandAction("lillil32", HandActionType.SHOW, 0, Street.Showdown),
                               new WinningsAction("lillil32", HandActionType.WINS, 113m, 0),                               
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("TTR116060183", HandActionType.SMALL_BLIND, 10m, Street.Preflop),
                               new HandAction("yutant", HandActionType.BIG_BLIND, 20m, Street.Preflop),
                               new HandAction("BurnN0tice", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Armstrongc", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("TTR116060183", HandActionType.FOLD, 0, Street.Preflop),
                               new WinningsAction("yutant", HandActionType.WINS, 30m, 0),                               
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("JohnJordan", HandActionType.SMALL_BLIND, 1m, Street.Preflop),
                               new HandAction("wingirl3", HandActionType.BIG_BLIND, 2m, Street.Preflop),
                               new HandAction("IPullGuard", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("MrJeremiahPokers", HandActionType.RAISE, 6m, Street.Preflop),
                               new HandAction("HEISENBERGG", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("doubting", HandActionType.CALL, 6m, Street.Preflop),
                               //Raises don't account for previous bets
                               new HandAction("JohnJordan", HandActionType.RAISE, 26m - 1m, Street.Preflop),
                               new HandAction("wingirl3", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("MrJeremiahPokers", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("doubting", HandActionType.CALL, 20m, Street.Preflop),
                               new HandAction("JohnJordan", HandActionType.BET, 26m, Street.Flop),
                               new HandAction("doubting", HandActionType.RAISE, 68m, Street.Flop),
                               new AllInAction("JohnJordan", 162m - 26m, Street.Flop, true),
                               new HandAction("doubting", HandActionType.CALL, 94m, Street.Flop),
                               new HandAction("doubting", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("JohnJordan", HandActionType.SHOW, 0, Street.Showdown),
                               new WinningsAction("doubting", HandActionType.WINS, 381m, 0),                               
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("RunningNuts1", HandActionType.SMALL_BLIND, 1m, Street.Preflop),
                               new HandAction("pepealas5", HandActionType.BIG_BLIND, 2m, Street.Preflop),
                               new HandAction("RunningNuts1", HandActionType.RAISE, 6m - 1m, Street.Preflop),
                               new HandAction("pepealas5", HandActionType.CALL, 4m, Street.Preflop),
                               new HandAction("pepealas5", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("RunningNuts1", HandActionType.BET, 6m, Street.Flop),
                               new HandAction("pepealas5", HandActionType.RAISE, 16m, Street.Flop),
                               new HandAction("RunningNuts1", HandActionType.CALL, 10m, Street.Flop),
                               new HandAction("pepealas5", HandActionType.BET, 22m, Street.Turn),
                               new HandAction("RunningNuts1", HandActionType.CALL, 22m, Street.Turn),
                               new HandAction("pepealas5", HandActionType.CHECK, 0m, Street.River),
                               new AllInAction("RunningNuts1", 56m, Street.River, true),
                               new HandAction("pepealas5", HandActionType.FOLD, 0m, Street.River),
                               new WinningsAction("RunningNuts1", HandActionType.WINS, 143m, 0),                               
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
                                        new HandAction("MrC0ld", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop),
                                        new HandAction("1715097", HandActionType.BIG_BLIND, 0.5m, Street.Preflop),
                                        new HandAction("MrSmartMoney", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("WOCKAFLOCKAJOPPA", HandActionType.RAISE, 1.75m, Street.Preflop),
                                        new HandAction("elcapacapa", HandActionType.CALL, 1.75m, Street.Preflop),
                                        new HandAction("Straddles", HandActionType.RAISE, 7.75m, Street.Preflop),
                                        new HandAction("MrC0ld", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("1715097", HandActionType.CALL, 7.25m, Street.Preflop),                                      
                                        new HandAction("WOCKAFLOCKAJOPPA", HandActionType.CALL, 6m, Street.Preflop),
                                        new HandAction("elcapacapa", HandActionType.CALL, 6m, Street.Preflop),

                                        new HandAction("1715097", HandActionType.BET, 15.62m, Street.Flop),                                      
                                        new HandAction("WOCKAFLOCKAJOPPA", HandActionType.FOLD, 0m, Street.Flop),
                                        new HandAction("elcapacapa", HandActionType.FOLD, 0m, Street.Flop),
                                        new AllInAction("Straddles", 44.68m, Street.Flop, true),
                                        new AllInAction("1715097", 19.94m, Street.Flop, false),

                                        new HandAction("Straddles", HandActionType.SHOW, 0, Street.Showdown),
                                        new HandAction("1715097", HandActionType.SHOW, 0, Street.Showdown),
                                        new WinningsAction("Straddles", HandActionType.WINS, 9.12m, 0),
                                        new WinningsAction("1715097", HandActionType.WINS, 99.37m, 0),
                                    };

            TestParseActions("AllInHandWithShowdown2", expectedActions);
        }

        [Test]
        public void PlayerParse_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("klunkepose", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                                        new HandAction("zh100", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                                        new HandAction("klunkepose", HandActionType.RAISE, 2m - 0.5m, Street.Preflop),
                                        new HandAction("zh100", HandActionType.CALL, 1m, Street.Preflop),

                                        new HandAction("zh100", HandActionType.BET, 4m, Street.Flop),                                      
                                        new HandAction("klunkepose", HandActionType.RAISE, 16m, Street.Flop),
                                        new HandAction("zh100", HandActionType.RAISE, 52m - 4m, Street.Flop),
                                        new AllInAction("klunkepose", 39.12m, Street.Flop, false),
                                        new HandAction("zh100", HandActionType.CALL, 3.12m, Street.Flop),

                                        new HandAction("zh100", HandActionType.SHOW, 0, Street.Showdown),
                                        new HandAction("klunkepose", HandActionType.SHOW, 0, Street.Showdown),
                                        new WinningsAction("klunkepose", HandActionType.WINS, 113.24m, 0),
                                    };

            TestParseActions("PlayerParseError", expectedActions);
        }

        [Test]
        public void NeedTwoHoleCardsError_Fixed()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("leokadia19", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                                        new HandAction("carpediem424", HandActionType.BIG_BLIND, 10m, Street.Preflop),
                                        new HandAction("numbersnletters", HandActionType.SITTING_OUT, 0m, Street.Preflop),
                                        new HandAction("PAYNLES", HandActionType.RAISE, 20m, Street.Preflop),                                      
                                        new HandAction("leokadia19", HandActionType.RAISE, 80m - 5m, Street.Preflop),
                                        new AllInAction("carpediem424", 256m - 10m, Street.Preflop, true),
                                        new HandAction("PAYNLES", HandActionType.FOLD, 0m, Street.Preflop),
                                        new HandAction("leokadia19", HandActionType.CALL, 176m, Street.Preflop),
                                        new HandAction("carpediem424", HandActionType.SHOW, 0, Street.Showdown),
                                        new HandAction("leokadia19", HandActionType.SHOW, 0, Street.Showdown),
                                        new WinningsAction("carpediem424", HandActionType.WINS, 530m, 0),
                                    };

            TestParseActions("NeedTwoHoleCardsError", expectedActions);
        }
    }
}
