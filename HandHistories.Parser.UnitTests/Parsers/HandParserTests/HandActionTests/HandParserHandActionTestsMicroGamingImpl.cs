using System;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.HandActionTests
{
    [TestFixture]
    class HandParserHandActionTestsMicroGamingImpl : HandParserHandActionTests
    {

        public HandParserHandActionTestsMicroGamingImpl()
             : base("MicroGaming")
        {
        }

        [Test]
        public void TestDisconnectedActions()
        {
            var actions = new List<HandAction>()
            {
                new HandAction("Giddy_Goat", HandActionType.SMALL_BLIND, 1m, Street.Preflop),
                new HandAction("JoakimAF", HandActionType.BIG_BLIND, 2m, Street.Preflop),
                new HandAction("tuffgong", HandActionType.JACKPOTCONTRIBUTION, 0.02m, Street.Preflop),
                new HandAction("tuffgong", HandActionType.RAISE, 6, Street.Preflop),
                new HandAction("CptStupid", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("ConorIsKing", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("Iordanes", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("BigLuigy", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("spidercat", HandActionType.RAISE, 22, Street.Preflop),
                new HandAction("Giddy_Goat", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("JoakimAF", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("tuffgong", HandActionType.RAISE, 68, Street.Preflop),
                new HandAction("spidercat", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("tuffgong", HandActionType.MUCKS, 0m, Street.Preflop),
                new WinningsAction("tuffgong", HandActionType.WINS, 99m, 0),    
            };

            TestParseActions("Disconnected", actions);
        }

        [Test]
        public void AllInWithUncalledBet_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
            {
                new HandAction("Player1", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                new HandAction("Player2", HandActionType.BIG_BLIND, 10m, Street.Preflop),

                new HandAction("Player3", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("Player4", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("Player6", HandActionType.RAISE, 35m, Street.Preflop),
                new HandAction("Player1", HandActionType.RAISE, 110m, Street.Preflop),
                new HandAction("Player2", HandActionType.FOLD, 0, Street.Preflop),

                new HandAction("Player6", HandActionType.RAISE, 320m, Street.Preflop),
                new HandAction("Player1", HandActionType.CALL, 122m, Street.Preflop, true),
                new HandAction("Player6", HandActionType.UNCALLED_BET, 117, Street.Preflop),
                new HandAction("Player1", HandActionType.SHOW, 0m, Street.Showdown),
                new HandAction("Player6", HandActionType.SHOW, 0m, Street.Showdown),
                new WinningsAction("Player1", HandActionType.WINS, 481.5m, 0),
            };

            TestParseActions("AllInHandWithUncalledBet", expectedActions);
        }

         protected override List<HandAction> ExpectedHandActionsBasicHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("creys", HandActionType.SMALL_BLIND, 0.01m, Street.Preflop),
                               new HandAction("_joker_", HandActionType.BIG_BLIND, 0.02m, Street.Preflop),
                               new HandAction("Jeesuslaps", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("DuckGhoul", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("creys", HandActionType.CALL, 0.01m, Street.Preflop),
                               new HandAction("_joker_", HandActionType.BET, 0.12m, Street.Preflop),
                               new HandAction("creys", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("_joker_", HandActionType.MUCKS, 0m, Street.Preflop),
                               new WinningsAction("_joker_", HandActionType.WINS, 0.16m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActionsFoldedPreflop
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("DuckGhoul", HandActionType.SMALL_BLIND, 0.01m, Street.Preflop),
                               new HandAction("creys", HandActionType.BIG_BLIND, 0.02m, Street.Preflop),
                               new HandAction("_joker_", HandActionType.RAISE, 0.05m, Street.Preflop),
                               new HandAction("Jeesuslaps", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("DuckGhoul", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("creys", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("_joker_", HandActionType.MUCKS, 0m, Street.Preflop),
                               new WinningsAction("_joker_", HandActionType.WINS, 0.08m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActions3BetHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("CrzyVndl", HandActionType.SMALL_BLIND, 0.01m, Street.Preflop),
                               new HandAction("Muszkliii", HandActionType.BIG_BLIND, 0.02m, Street.Preflop),
                               new HandAction("jugins", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("MrJohnCarter", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("gabrieliso", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("SinjkV", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("CrzyVndl", HandActionType.RAISE, 0.06m, Street.Preflop),
                               new HandAction("Muszkliii", HandActionType.RAISE, 0.14m, Street.Preflop),
                               new HandAction("CrzyVndl", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("Muszkliii", HandActionType.MUCKS, 0, Street.Preflop),
                               new WinningsAction("Muszkliii", HandActionType.WINS, 0.23m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActionsAllInHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("jugins", HandActionType.SMALL_BLIND, 0.01m, Street.Preflop),
                               new HandAction("MrJohnCarter", HandActionType.BIG_BLIND, 0.02m, Street.Preflop),
                               new HandAction("gabrieliso", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("SinjkV", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("CrzyVndl", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("Muszkliii", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("jugins", HandActionType.CALL, 0.01m, Street.Preflop),
                               new HandAction("MrJohnCarter", HandActionType.BET, 0.12m, Street.Preflop),
                               new HandAction("jugins", HandActionType.CALL, 0.12m, Street.Preflop),
                               new HandAction("jugins", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("MrJohnCarter", HandActionType.BET, 2.02m, Street.Flop, true),
                               new HandAction("jugins", HandActionType.CALL, 0.42m, Street.Flop, true),
                               new HandAction("MrJohnCarter", HandActionType.UNCALLED_BET, 1.60m, Street.Flop),
                               new HandAction("jugins", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("MrJohnCarter", HandActionType.SHOW, 0, Street.Showdown),
                               new WinningsAction("jugins", HandActionType.WINS, 1.02m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedOmahaHiLoHand
         {
             get
             {
                 return new List<HandAction>()
                    {
                        new HandAction("hoop", HandActionType.SMALL_BLIND, 0.01m, Street.Preflop),
                        new HandAction("makkis__", HandActionType.BIG_BLIND, 0.02m, Street.Preflop),
                        new HandAction("tigersue", HandActionType.CALL, 0.02m, Street.Preflop),
                        new HandAction("theweman", HandActionType.CALL, 0.02m, Street.Preflop),
                        new HandAction("hoop", HandActionType.CALL, 0.01m, Street.Preflop),
                        new HandAction("makkis__", HandActionType.CHECK, 0, Street.Preflop),

                        new HandAction("hoop", HandActionType.CHECK, 0m, Street.Flop),
                        new HandAction("makkis__", HandActionType.CHECK, 0m, Street.Flop),
                        new HandAction("tigersue", HandActionType.CHECK, 0m, Street.Flop),
                        new HandAction("theweman", HandActionType.BET, 0.08m, Street.Flop),
                        new HandAction("hoop", HandActionType.RAISE, 0.32m, Street.Flop),
                        new HandAction("makkis__", HandActionType.FOLD, 0m, Street.Flop),
                        new HandAction("tigersue", HandActionType.FOLD, 0m, Street.Flop),
                        new HandAction("theweman", HandActionType.CALL, 0.24m, Street.Flop),

                        new HandAction("hoop", HandActionType.BET, 0.72m, Street.Turn),
                        new HandAction("theweman", HandActionType.CALL, 0.72m, Street.Turn),
                               
                        new HandAction("hoop", HandActionType.BET, 0.70m, Street.River, true),
                        new HandAction("theweman", HandActionType.CALL, 0.70m, Street.River),

                        new HandAction("theweman", HandActionType.SHOW, 0, Street.Showdown),
                        new HandAction("hoop", HandActionType.SHOW, 0, Street.Showdown),

                        new WinningsAction("theweman", HandActionType.WINS, 1.73m, 0),
                        new WinningsAction("hoop", HandActionType.WINS, 1.73m, 0)
                    };
             }
         }
    }
}
