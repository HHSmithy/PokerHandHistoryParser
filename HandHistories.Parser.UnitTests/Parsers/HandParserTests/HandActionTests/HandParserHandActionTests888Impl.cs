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
    class HandParserHandActionTests888Impl : HandParserHandActionTests
    {
         public HandParserHandActionTests888Impl()
             : base("Pacific")
        {
        }

         protected override List<HandAction> ExpectedHandActionsBasicHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("engils", HandActionType.SMALL_BLIND, 500m, Street.Preflop),
                               new HandAction("ThrCnBeOnly1", HandActionType.BIG_BLIND, 1000m, Street.Preflop),
                               new HandAction("OprahTiltfre", HandActionType.RAISE, 5000m, Street.Preflop),
                               new HandAction("LitFaSlane", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("fifafairplay", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("engils", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("ThrCnBeOnly1", HandActionType.FOLD, 0m, Street.Preflop),
                               new WinningsAction("OprahTiltfre", HandActionType.WINS, 2500m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActionsFoldedPreflop
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("iSmokeMeth", HandActionType.SMALL_BLIND, 0.50m, Street.Preflop),
                               new HandAction("maybmaybnot", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                               new HandAction("suicram", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("AUT_Camper", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("PR_Eqiuty", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("sander1900", HandActionType.RAISE, 3.50m, Street.Preflop),
                               new HandAction("iSmokeMeth", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("maybmaybnot", HandActionType.FOLD, 0m, Street.Preflop),
                               new WinningsAction("sander1900", HandActionType.WINS, 2.50m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActions3BetHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("iSmokeMeth", HandActionType.SMALL_BLIND, 0.50m, Street.Preflop),
                               new HandAction("maybmaybnot", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                               new HandAction("suicram", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("AUT_Camper", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("PR_Eqiuty", HandActionType.RAISE, 3m, Street.Preflop),
                               new HandAction("sander1900", HandActionType.RAISE, 10.50m, Street.Preflop),
                               new HandAction("iSmokeMeth", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("maybmaybnot", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("PR_Eqiuty", HandActionType.CALL, 7.50m, Street.Preflop),
                               new HandAction("PR_Eqiuty", HandActionType.CHECK, 0, Street.Flop),
                               new HandAction("sander1900", HandActionType.BET, 22.50m, Street.Flop),
                               new HandAction("PR_Eqiuty", HandActionType.FOLD, 0, Street.Flop),
                               new WinningsAction("sander1900", HandActionType.WINS, 21.38m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActionsAllInHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("ThrCnBeOnly1", HandActionType.SMALL_BLIND, 500m, Street.Preflop),
                               new HandAction("OprahTiltfre", HandActionType.BIG_BLIND, 1000m, Street.Preflop),
                               new HandAction("LitFaSlane", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("fifafairplay", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("engils", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("SANP3R", HandActionType.FOLD, 0, Street.Preflop),
                               new AllInAction("ThrCnBeOnly1", 4500m, Street.Preflop, true),
                               new HandAction("OprahTiltfre", HandActionType.CALL, 4000, Street.Preflop),
                               new HandAction("ThrCnBeOnly1", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("OprahTiltfre", HandActionType.SHOW, 0, Street.Showdown),
                               new WinningsAction("ThrCnBeOnly1", HandActionType.WINS, 9995, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedOmahaHiLoHand
         {
             get { throw new NotImplementedException(); }
         }
    }
}
