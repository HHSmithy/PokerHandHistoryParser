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

         protected override List<HandAction> ExpectedHandActionsBasicHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("3", HandActionType.SMALL_BLIND, 0.01m, Street.Preflop),
                               new HandAction("5", HandActionType.BIG_BLIND, 0.02m, Street.Preflop),
                               new HandAction("6", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("1", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("3", HandActionType.CALL, 0.01m, Street.Preflop),
                               new HandAction("5", HandActionType.BET, 0.12m, Street.Preflop),
                               new HandAction("3", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("5", HandActionType.MUCKS, 0m, Street.Preflop),
                               new WinningsAction("_joker_", HandActionType.WINS, 0.02m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActionsFoldedPreflop
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("1", HandActionType.SMALL_BLIND, 0.01m, Street.Preflop),
                               new HandAction("3", HandActionType.BIG_BLIND, 0.02m, Street.Preflop),
                               new HandAction("5", HandActionType.RAISE, 0.05m, Street.Preflop),
                               new HandAction("6", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("1", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("3", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("5", HandActionType.MUCKS, 0m, Street.Preflop),
                               new WinningsAction("_joker_", HandActionType.WINS, 0.03m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActions3BetHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("1", HandActionType.SMALL_BLIND, 0.01m, Street.Preflop),
                               new HandAction("2", HandActionType.BIG_BLIND, 0.02m, Street.Preflop),
                               new HandAction("3", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("4", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("5", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("6", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("1", HandActionType.RAISE, 0.06m, Street.Preflop),
                               new HandAction("2", HandActionType.RAISE, 0.14m, Street.Preflop),
                               new HandAction("1", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("2", HandActionType.MUCKS, 0, Street.Preflop),
                               new WinningsAction("Muszkliii", HandActionType.WINS, 0.07m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedHandActionsAllInHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("3", HandActionType.SMALL_BLIND, 0.01m, Street.Preflop),
                               new HandAction("4", HandActionType.BIG_BLIND, 0.02m, Street.Preflop),
                               new HandAction("5", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("6", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("1", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("2", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("3", HandActionType.CALL, 0.01m, Street.Preflop),
                               new HandAction("4", HandActionType.BET, 0.12m, Street.Preflop),
                               new HandAction("3", HandActionType.CALL, 0.12m, Street.Preflop),
                               new HandAction("3", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("4", HandActionType.ALL_IN, 2.02m, Street.Flop),
                               new HandAction("3", HandActionType.ALL_IN, 0.42m, Street.Flop),
                               new HandAction("4", HandActionType.UNCALLED_BET, 1.60m, Street.Flop),
                               new HandAction("jugins", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("MrJohnCarter", HandActionType.SHOW, 0, Street.Showdown),
                               new WinningsAction("jugins", HandActionType.WINS, 0.46m, 0),                               
                           };
             }
         }

         protected override List<HandAction> ExpectedOmahaHiLoHand
         {
             get
             {
                 return new List<HandAction>()
                           {
                               new HandAction("6", HandActionType.SMALL_BLIND, 0.01m, Street.Preflop),
                               new HandAction("3", HandActionType.BIG_BLIND, 0.02m, Street.Preflop),
                               new HandAction("4", HandActionType.CALL, 0.02m, Street.Preflop),
                               new HandAction("5", HandActionType.CALL, 0.02m, Street.Preflop),
                               new HandAction("6", HandActionType.CALL, 0.01m, Street.Preflop),
                               new HandAction("3", HandActionType.CHECK, 0, Street.Preflop),

                               new HandAction("6", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("3", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("4", HandActionType.CHECK, 0m, Street.Flop),
                               new HandAction("5", HandActionType.BET, 0.08m, Street.Flop),
                               new HandAction("6", HandActionType.RAISE, 0.32m, Street.Flop),
                               new HandAction("3", HandActionType.FOLD, 0m, Street.Flop),
                               new HandAction("4", HandActionType.FOLD, 0m, Street.Flop),
                               new HandAction("5", HandActionType.CALL, 0.24m, Street.Flop),

                               new HandAction("6", HandActionType.BET, 0.72m, Street.Turn),
                               new HandAction("5", HandActionType.CALL, 0.72m, Street.Turn),
                               
                               new HandAction("6", HandActionType.ALL_IN, 0.70m, Street.River),
                               new HandAction("5", HandActionType.CALL, 0.70m, Street.River),

                               new HandAction("theweman", HandActionType.SHOW, 0, Street.Showdown),
                               new HandAction("hoop", HandActionType.SHOW, 0, Street.Showdown)
                           };
             }
         }
    }
}
