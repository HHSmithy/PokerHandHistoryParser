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
    class HandParserHandActionTestsWinamaxImpl : HandParserHandActionTests
    {

        public HandParserHandActionTestsWinamaxImpl()
             : base("Winamax")
        {
        }

        [Test]
        public void PostingBBPostingSB_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                {
                    new HandAction("totti6720", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop),
                    new HandAction("titi250", HandActionType.BIG_BLIND, 0.5m, Street.Preflop),
                    new HandAction("Dbrz34", HandActionType.POSTS_DEAD, 0.25m, Street.Preflop),
                    new HandAction("Dbrz34", HandActionType.POSTS, 0.5m, Street.Preflop),

                    new HandAction("Dbrz34", HandActionType.CHECK, Street.Preflop),
                    new HandAction("sined20", HandActionType.RAISE, 1.5m, Street.Preflop),
                    new HandAction("fanf4K UR0", HandActionType.FOLD, Street.Preflop),
                    new HandAction("totti6720", HandActionType.FOLD, Street.Preflop),
                    new HandAction("titi250", HandActionType.FOLD, Street.Preflop),
                    new HandAction("Dbrz34", HandActionType.CALL, 0.5m, Street.Preflop),

                    new HandAction("Dbrz34", HandActionType.CHECK, Street.Flop),
                    new HandAction("sined20", HandActionType.BET, 2.50m, Street.Flop),
                    new HandAction("Dbrz34", HandActionType.FOLD, Street.Flop),
                    new WinningsAction("sined20", HandActionType.WINS, 6.26m, 0)
                };

            TestParseActions("PostingDead", expectedActions);
        }

         protected override List<HandAction> ExpectedHandActionsBasicHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("totti6720", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop),
                               new HandAction("titi250", HandActionType.BIG_BLIND, 0.50m, Street.Preflop),
                               new HandAction("Dbrz34", HandActionType.CALL, 0.5m, Street.Preflop),
                               new HandAction("sined20", HandActionType.RAISE, 1.50m, Street.Preflop),
                               new HandAction("fanf4K UR0", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("totti6720", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("titi250", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Dbrz34", HandActionType.CALL, 1, Street.Preflop),
                               new HandAction("Dbrz34", HandActionType.CHECK, 0, Street.Flop),
                               new HandAction("sined20", HandActionType.BET, 2.50m, Street.Flop),
                               new HandAction("Dbrz34", HandActionType.FOLD, 0, Street.Flop),
                               new WinningsAction("sined20", HandActionType.WINS,6.01m,0)
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActionsFoldedPreflop
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("xNimzo49x", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop),
                               new HandAction("titi250", HandActionType.BIG_BLIND, 0.50m, Street.Preflop),
                               new HandAction("karine507", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("fanf4K UR0", HandActionType.RAISE, 1.25m, Street.Preflop),
                               new HandAction("xNimzo49x", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("titi250", HandActionType.FOLD, 0, Street.Preflop),
                               new WinningsAction("fanf4K UR0", HandActionType.WINS, 2,0)
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActions3BetHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("totti6720", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop),
                               new HandAction("titi250", HandActionType.BIG_BLIND, 0.50m, Street.Preflop),
                               new HandAction("sined20", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("fanf4K UR0", HandActionType.RAISE, 1.25m, Street.Preflop),
                               new HandAction("totti6720", HandActionType.RAISE, 3.00m, Street.Preflop),
                               new HandAction("titi250", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("fanf4K UR0", HandActionType.CALL, 2, Street.Preflop),
                               new HandAction("totti6720", HandActionType.BET, 4.7m, Street.Flop),
                               new HandAction("fanf4K UR0", HandActionType.FOLD, 0m, Street.Flop),
                               new WinningsAction("totti6720", HandActionType.WINS, 11.24m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActionsAllInHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("Matthieu_59_", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop),
                               new HandAction("PornstarX", HandActionType.BIG_BLIND, 0.5m, Street.Preflop),
                               new HandAction("nico86190", HandActionType.RAISE, 2.5m, Street.Preflop),
                               new HandAction("-LePianiste-", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("LEROISALO", HandActionType.RAISE, 22.85m, Street.Preflop, true),
                               new HandAction("Matthieu_59_", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("PornstarX", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("nico86190", HandActionType.CALL, 20.35m, Street.Preflop),
                               new HandAction("LEROISALO", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("nico86190", HandActionType.SHOW, 0, Street.Showdown),
                               new WinningsAction("nico86190", HandActionType.WINS, 43.45m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedOmahaHiLoHand
         {
             get { throw new NotImplementedException(); }
         }
    }
}
