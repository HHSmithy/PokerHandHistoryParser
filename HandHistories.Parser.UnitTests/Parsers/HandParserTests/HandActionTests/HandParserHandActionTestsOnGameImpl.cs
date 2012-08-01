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
    class HandParserHandActionTestsOnGameImpl : HandParserHandActionTests
    {
        public HandParserHandActionTestsOnGameImpl() : base("OnGame")
        {
        }

        protected override List<HandAction> ExpectedHandActionsBasicHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("GlassILass", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop),
                               new HandAction("EvilJihnny99", HandActionType.BIG_BLIND, 0.25m, Street.Preflop),
                               new HandAction("burtias", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("zatli74", HandActionType.RAISE, 1, Street.Preflop),
                               new HandAction("GlassILass", HandActionType.CALL, 0.75m, Street.Preflop),
                               new HandAction("EvilJihnny99", HandActionType.FOLD, 0, Street.Preflop),                        
                               new HandAction("GlassILass", HandActionType.CHECK, 0, Street.Flop),
                               new HandAction("zatli74", HandActionType.BET, 2.25m, Street.Flop),                              
                               new HandAction("GlassILass", HandActionType.FOLD, 0, Street.Flop),
                               new WinningsAction("zatli74", HandActionType.WINS, 2.14m, 0),
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("njoetrhawrg", HandActionType.SMALL_BLIND,10m, Street.Preflop),
                               new HandAction("el_pescado99", HandActionType.BIG_BLIND, 20m, Street.Preflop),
                               new HandAction("DM899", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("sickseeker", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("njoetrhawrg", HandActionType.FOLD, 0m, Street.Preflop),
                               new WinningsAction("el_pescado99", HandActionType.WINS, 20, 0),
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("AlexZander2", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                               new HandAction("kojac123", HandActionType.BIG_BLIND, 5m, Street.Preflop),
                               new HandAction("kallepertti9", HandActionType.RAISE, 20, Street.Preflop),
                               new HandAction("AlexZander2", HandActionType.RAISE, 30, Street.Preflop),
                               new HandAction("kojac123", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("kallepertti9", HandActionType.CALL, 15, Street.Preflop),                        
                               new HandAction("AlexZander2", HandActionType.BET, 40m, Street.Flop),                              
                               new HandAction("kallepertti9", HandActionType.FOLD, 0, Street.Flop),
                               new WinningsAction("AlexZander2", HandActionType.WINS, 73m, 0),
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("el_pescado99", HandActionType.SMALL_BLIND, 10m, Street.Preflop),
                               new HandAction("DM899", HandActionType.BIG_BLIND, 20m, Street.Preflop),
                               new HandAction("njoetrhawrg", HandActionType.RAISE, 50, Street.Preflop),
                               new HandAction("el_pescado99", HandActionType.RAISE, 170, Street.Preflop),
                               new HandAction("DM899", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("njoetrhawrg", HandActionType.RAISE, 330, Street.Preflop),                        
                               new AllInAction("el_pescado99", 2143, Street.Preflop, true),
                               new HandAction("njoetrhawrg", HandActionType.CALL,  1943m, Street.Preflop),
                               new WinningsAction("el_pescado99", HandActionType.WINS, 4664m, 0),
                           };
            }
        }

        protected override List<HandAction> ExpectedOmahaHiLoHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("Buzzball x", HandActionType.SMALL_BLIND, 0.10m, Street.Preflop),
                               new HandAction("Hawkmoon14", HandActionType.BIG_BLIND, 0.10m, Street.Preflop),
                               new HandAction("bobcov", HandActionType.SMALL_BLIND, 0.10m, Street.Preflop),
                               new HandAction("bobcov", HandActionType.POSTS, 0.10m, Street.Preflop),
                               new HandAction("Anna_Nowak", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("Muppet70", HandActionType.FOLD, 0, Street.Preflop),                        
                               new HandAction("bobcov",  HandActionType.CHECK, 0, Street.Preflop),
                               new HandAction("lazos1973", HandActionType.CALL,  0.10m, Street.Preflop),
                               new HandAction("Buzzball x", HandActionType.CHECK, 0, Street.Preflop),
                               new HandAction("Hawkmoon14", HandActionType.RAISE, 0.50m, Street.Preflop),
                               new HandAction("bobcov", HandActionType.CALL, 0.50m, Street.Preflop),
                               new HandAction("lazos1973", HandActionType.FOLD, 0, Street.Preflop),                        
                               new HandAction("Buzzball x", HandActionType.FOLD, 0, Street.Preflop),                        
                               new HandAction("Hawkmoon14", HandActionType.BET, 0.80m, Street.Flop),      
                               new HandAction("bobcov", HandActionType.FOLD, 0m, Street.Flop),     
                               new WinningsAction("Hawkmoon14", HandActionType.WINS, 1.40m, 0),                               
                           };
            }
        }

        [Test]
        public void PlayerNameWithDashes_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("shmel77", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                                        new HandAction("yoyogi36", HandActionType.BIG_BLIND, 0.10m, Street.Preflop),
                                        new HandAction("Mivko", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("Rpc_alpetra", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("ugeroka", HandActionType.RAISE, 0.40m, Street.Preflop),
                                        new HandAction("---ich---", HandActionType.CALL, 0.40m, Street.Preflop),
                                        new HandAction("djiceflame70", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("shmel77", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("yoyogi36", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("ugeroka", HandActionType.CHECK, 0, Street.Flop),
                                        new HandAction("---ich---", HandActionType.CHECK, 0, Street.Flop),
                                        new HandAction("ugeroka", HandActionType.CHECK, 0, Street.Turn),
                                        new HandAction("---ich---", HandActionType.BET, 0.70m, Street.Turn),
                                        new HandAction("ugeroka", HandActionType.FOLD, 0, Street.Turn),
                                        new WinningsAction("---ich---", HandActionType.WINS, 0.89m, 0),
                                    };

            TestParseActions("NameWithDashes", expectedActions);
        }
    }
}
