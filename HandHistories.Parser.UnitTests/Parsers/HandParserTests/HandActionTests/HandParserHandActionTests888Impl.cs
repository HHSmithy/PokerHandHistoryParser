﻿using System;
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

         [Test]
         public void ParseHandActions_PostingDead()
         {
             var expected = new List<HandAction>()
             {
                new HandAction("icebbberg", HandActionType.SMALL_BLIND, 1m, Street.Preflop),
                new HandAction("whoisdrew", HandActionType.BIG_BLIND, 2m, Street.Preflop),
                new HandAction("GYAMEPRO", HandActionType.POSTS, 3m, Street.Preflop),
                new HandAction("Mr.Pesto", HandActionType.FOLD, Street.Preflop),
                new HandAction("GYAMEPRO", HandActionType.CHECK, Street.Preflop),
                new HandAction("Suic1deKing", HandActionType.FOLD, Street.Preflop),
                new HandAction("gaokaipoker", HandActionType.FOLD, Street.Preflop),
                new HandAction("icebbberg", HandActionType.FOLD, Street.Preflop),
                new HandAction("whoisdrew", HandActionType.CHECK, Street.Preflop),


                new HandAction("whoisdrew", HandActionType.CHECK, Street.Flop),
                new HandAction("GYAMEPRO", HandActionType.CHECK, Street.Flop),

                new HandAction("whoisdrew", HandActionType.CHECK, Street.Turn),
                new HandAction("GYAMEPRO", HandActionType.CHECK, Street.Turn),

                new HandAction("whoisdrew", HandActionType.CHECK, Street.River),
                new HandAction("GYAMEPRO", HandActionType.CHECK, Street.River),

                new HandAction("whoisdrew", HandActionType.SHOW, Street.Showdown),
                new HandAction("GYAMEPRO", HandActionType.SHOW, Street.Showdown),

                new WinningsAction("GYAMEPRO", HandActionType.WINS, 5.70m, 0),   
             };

             TestParseActions("PostingDead", expected);
         }

         // due to our uncalledbetfix, we have an action that is not visible in the handhistories
         // uncalled bets are always on the showdown by our parser
         protected override List<HandAction> ExpectedHandActionsBasicHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("FCSM_1935", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop),
                               new HandAction("silas_tomkyn", HandActionType.BIG_BLIND, 0.10m, Street.Preflop),
                               new HandAction("FCSM_1935", HandActionType.CALL, 0.05m, Street.Preflop),
                               new HandAction("silas_tomkyn", HandActionType.CHECK, 0, Street.Preflop),
                               new HandAction("silas_tomkyn", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("FCSM_1935", HandActionType.BET, 0.1m, Street.Flop),
                               new HandAction("silas_tomkyn", HandActionType.FOLD, 0m, Street.Flop),
                               new WinningsAction("FCSM_1935", HandActionType.WINS, 0.19m, 0),             
                               new HandAction("FCSM_1935", HandActionType.UNCALLED_BET, 0.1m, Street.Showdown),                  
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActionsFoldedPreflop
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("Lesnik444", HandActionType.SMALL_BLIND, 0.50m, Street.Preflop),
                               new HandAction("Griini", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                               new HandAction("Grieche6969", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("dbhbrb", HandActionType.RAISE, 2m, Street.Preflop),
                               new HandAction("Lesnik444", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("Griini", HandActionType.FOLD, 0m, Street.Preflop),
                               new WinningsAction("dbhbrb", HandActionType.WINS, 2.50m, 0),          
                               new HandAction("dbhbrb", HandActionType.UNCALLED_BET, 1m, Street.Showdown), // 2 - 1  ( BB ) 
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActions3BetHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("littlez33", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                               new HandAction("AceOfSpace87", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                               new HandAction("M00700", HandActionType.CALL, 1m, Street.Preflop),
                               new HandAction("cashgamer15", HandActionType.RAISE, 2m, Street.Preflop),
                               new HandAction("Jack3bet", HandActionType.RAISE, 6.37m, Street.Preflop),
                               new HandAction("Krautsturm", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("littlez33", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("AceOfSpace87", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("M00700", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("cashgamer15", HandActionType.CALL,4.37m, Street.Preflop),
                               new HandAction("cashgamer15", HandActionType.CHECK, 0, Street.Flop),
                               new HandAction("Jack3bet", HandActionType.BET, 10.43m, Street.Flop),
                               new HandAction("cashgamer15", HandActionType.CALL, 10.43m, Street.Flop),
                               new HandAction("cashgamer15", HandActionType.CHECK, 0m, Street.Turn),
                               new HandAction("Jack3bet", HandActionType.CHECK, 0m, Street.Turn),
                               new HandAction("cashgamer15", HandActionType.BET, 36.10m, Street.River),
                               new HandAction("Jack3bet", HandActionType.FOLD, 0m, Street.River),
                               new WinningsAction("cashgamer15", HandActionType.WINS, 34.30m, 0),     
                               new HandAction("cashgamer15", HandActionType.UNCALLED_BET, 36.10m, Street.Showdown),                          
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActionsAllInHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("zvony_tango7", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                               new HandAction("kiss014", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                               new HandAction("dynamoz6", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                               new HandAction("dynamoz6", HandActionType.CHECK, 0m, Street.Preflop),
                               new HandAction("qprcuz", HandActionType.CALL, 1m, Street.Preflop),
                               new HandAction("FishPredator", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("zvony_tango7", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("kiss014", HandActionType.CHECK, 0, Street.Preflop),
                               new HandAction("kiss014", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("dynamoz6", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("qprcuz", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("kiss014", HandActionType.BET, 2.62m, Street.Turn),
                               new HandAction("dynamoz6", HandActionType.FOLD, 0, Street.Turn),
                               new HandAction("qprcuz", HandActionType.CALL, 2.62m, Street.Turn),
                               new HandAction("kiss014", HandActionType.BET, 6.55m, Street.River),
                               new HandAction("qprcuz", HandActionType.RAISE, 101.66m, Street.River,true),
                               new HandAction("kiss014", HandActionType.CALL, 89.83m, Street.River,true),
                               new HandAction("qprcuz", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("kiss014", HandActionType.SHOW, 0, Street.Showdown),
                               new WinningsAction("qprcuz", HandActionType.WINS, 197.50m, 0),        
                               new HandAction("qprcuz", HandActionType.UNCALLED_BET, 5.28m, Street.Showdown),  // 101.66 - 6.55 - 79.83               
                           };
             }
         }

         protected override List<HandAction> ExpectedOmahaHiLoHand
         {
             get { throw new NotImplementedException(); }
         }
    }
}
