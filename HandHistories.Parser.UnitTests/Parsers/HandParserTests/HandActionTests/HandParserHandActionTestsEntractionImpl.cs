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
    class HandParserHandActionTestsEntractionImpl : HandParserHandActionTests
    {
         public HandParserHandActionTestsEntractionImpl()
             : base("Entraction")
        {
        }

         protected override List<HandAction> ExpectedHandActionsBasicHand
         {
             get
             {
                 return new List<HandAction>()
                {
                    new HandAction("Pinokio1", HandActionType.SMALL_BLIND, 0.25m, Street.Preflop),
                    new HandAction("pauli1", HandActionType.BIG_BLIND, 0.50m, Street.Preflop),
                    new HandAction("Pinokio1", HandActionType.CALL, 0.25m, Street.Preflop),
                    new HandAction("pauli1", HandActionType.CHECK, 0, Street.Preflop),                                              
                    new HandAction("Pinokio1", HandActionType.CHECK, 0, Street.Flop),
                    new HandAction("pauli1", HandActionType.BET, 0.50m, Street.Flop),
                    new HandAction("Pinokio1", HandActionType.CALL, 0.50m, Street.Flop),
                    new HandAction("Pinokio1", HandActionType.CHECK, 0, Street.Turn),
                    new HandAction("pauli1", HandActionType.CHECK, 0, Street.Turn),
                    new HandAction("Pinokio1", HandActionType.BET, 2.50m, Street.River),
                    new HandAction("pauli1", HandActionType.RAISE, 7.75m, Street.River),
                    new HandAction("Pinokio1", HandActionType.RAISE, 10.50m, Street.River),
                    new HandAction("pauli1", HandActionType.CALL, 5.25m, Street.River),          
                    new HandAction("Pinokio1", HandActionType.SHOW, 0, Street.Showdown),
                    new HandAction("pauli1", HandActionType.SHOW, 0, Street.Showdown),                             
                };
             }
         }

         protected override List<WinningsAction> ExpectedWinnersHandActionsBasicHand
         {
             get { return new List<WinningsAction>() { new WinningsAction("pauli1", WinningsActionType.WINS, 27.25m, 0), }; }
         }

         protected override List<HandAction> ExpectedHandActionsFoldedPreflop
         {
             get
             {
                 return new List<HandAction>()
                    {
                        new HandAction("NO12", HandActionType.BIG_BLIND, 50, Street.Preflop),
                        new HandAction("del1verance", HandActionType.SMALL_BLIND, 25, Street.Preflop),
                        new HandAction("del1verance", HandActionType.FOLD, 0, Street.Preflop),
                        new HandAction("NO12", HandActionType.UNCALLED_BET, 25, Street.Preflop),     
                                                    
                    };
             }
         }

         protected override List<WinningsAction> ExpectedWinnersHandActionsFoldedPreflop
         {
             get { return new List<WinningsAction>() { new WinningsAction("NO12", WinningsActionType.WINS, 50, 0), }; }
         }

         protected override List<HandAction> ExpectedHandActions3BetHand
         {
             get
             {
                 return new List<HandAction>()
                {
                    new HandAction("wELMA", HandActionType.SMALL_BLIND, 0.50m, Street.Preflop),
                    new HandAction("Vrddhi", HandActionType.BIG_BLIND, 1, Street.Preflop),
                    new HandAction("OffMeNut", HandActionType.RAISE, 3.50m, Street.Preflop),
                    new HandAction("stook", HandActionType.RAISE, 7.75m, Street.Preflop),
                    new HandAction("wELMA", HandActionType.CALL, 7.25m, Street.Preflop),
                    new HandAction("Vrddhi", HandActionType.FOLD, 0, Street.Preflop),
                    new HandAction("OffMeNut", HandActionType.CALL, 4.25m, Street.Preflop),                               
                    new HandAction("wELMA", HandActionType.CHECK, 0, Street.Flop),
                    new HandAction("OffMeNut", HandActionType.CHECK, 0, Street.Flop),
                    new HandAction("stook", HandActionType.BET, 23, Street.Flop),
                    new HandAction("wELMA", HandActionType.FOLD, 0, Street.Flop),
                    new HandAction("OffMeNut", HandActionType.FOLD, 0, Street.Flop),
                    new HandAction("stook", HandActionType.UNCALLED_BET, 23, Street.Flop),
                                                  
                };
             }
         }

         protected override List<WinningsAction> ExpectedWinnersHandActions3BetHand
         {
             get { return new List<WinningsAction>() { new WinningsAction("stook", WinningsActionType.WINS, 23.04m, 0), }; }
         }

         protected override List<HandAction> ExpectedHandActionsAllInHand
         {
             get
             {
                 return new List<HandAction>()
                    {
                        new HandAction("YoOyYo", HandActionType.BIG_BLIND, 100, Street.Preflop),
                        new HandAction("bglegend22", HandActionType.SMALL_BLIND, 50, Street.Preflop),
                        new HandAction("bglegend22", HandActionType.RAISE, 150, Street.Preflop),
                        new HandAction("YoOyYo", HandActionType.RAISE, 500, Street.Preflop),                                              
                        new HandAction("bglegend22", HandActionType.CALL, 400, Street.Preflop),
                        new HandAction("YoOyYo", HandActionType.BET, 900m, Street.Flop),
                        new AllInAction("bglegend22", 3559.76m, Street.Flop, false),
                        new AllInAction("YoOyYo", 1450m, Street.Flop, false),
                        new HandAction("bglegend22", HandActionType.UNCALLED_BET, 1209.76m, Street.Flop),
                        new HandAction("YoOyYo", HandActionType.SHOW, 0, Street.Showdown),
                        new HandAction("bglegend22", HandActionType.SHOW, 0, Street.Showdown),
                                                     
                    };
             }
         }

         protected override List<WinningsAction> ExpectedWinnersHandActionsAllInHand
         {
             get { return new List<WinningsAction>() { new WinningsAction("bglegend22", WinningsActionType.WINS, 5898.50m, 0), }; }
         }

         protected override List<HandAction> ExpectedOmahaHiLoHand
         {
             get
             {
                 return new List<HandAction>()
                    {
                        new HandAction("Ballyhoo", HandActionType.BIG_BLIND, 100, Street.Preflop),
                        new HandAction("YoOyYo", HandActionType.SMALL_BLIND, 50, Street.Preflop),
                        new HandAction("YoOyYo", HandActionType.RAISE, 150, Street.Preflop),
                        new HandAction("Ballyhoo", HandActionType.CALL, 100, Street.Preflop),
                        new HandAction("Ballyhoo", HandActionType.CHECK, 0, Street.Flop),
                        new HandAction("YoOyYo", HandActionType.BET, 100, Street.Flop),
                        new HandAction("Ballyhoo", HandActionType.RAISE, 200, Street.Flop),
                        new HandAction("YoOyYo", HandActionType.CALL, 100, Street.Flop),
                        new HandAction("Ballyhoo", HandActionType.BET, 200, Street.Turn),
                        new HandAction("YoOyYo", HandActionType.RAISE, 400m, Street.Turn),
                        new HandAction("Ballyhoo", HandActionType.CALL, 200, Street.Turn),
                        new HandAction("Ballyhoo", HandActionType.CHECK, 0, Street.River),
                        new HandAction("YoOyYo", HandActionType.BET, 200m, Street.River),
                        new HandAction("Ballyhoo", HandActionType.CALL, 200, Street.River),      
                        new HandAction("YoOyYo", HandActionType.SHOW, 0, Street.Showdown),
                        new HandAction("Ballyhoo", HandActionType.SHOW, 0, Street.Showdown),
                        
                    };
             }
         }

         protected override List<WinningsAction> ExpectedWinnersOmahaHiLoHand
         {
             get
             {
                 return new List<WinningsAction>() {
                    new WinningsAction("YoOyYo", WinningsActionType.WINS, 999.25m, 0),                               
                    new WinningsAction("Ballyhoo", WinningsActionType.WINS, 999.25m, 0),    
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

         protected override List<WinningsAction> ExpectedWinnersHandActionsUncalledBetHand
         {
             get { throw new NotImplementedException(); }
         }
    }
}
