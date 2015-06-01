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
                               new HandAction("BONUS 1OOO", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                               new HandAction("fyabcf", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                               new HandAction("kliketiklok", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Dbcee89", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("BlackH0L3", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Cockatrice", HandActionType.FOLD, 0, Street.Preflop),                        
                               new HandAction("BONUS 1OOO", HandActionType.RAISE, 1.50m, Street.Preflop),
                               new HandAction("fyabcf", HandActionType.CALL, 1m, Street.Preflop),
                              
                               new HandAction("BONUS 1OOO", HandActionType.BET, 2.5m, Street.Flop),
                               new HandAction("fyabcf", HandActionType.FOLD, 0, Street.Flop),

                               new WinningsAction("BONUS 1OOO", HandActionType.WINS, 3.80m, 0),
                               new HandAction("BONUS 1OOO", HandActionType.UNCALLED_BET, 2.5m, Street.Showdown),
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("fyabcf", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                               new HandAction("kliketiklok", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                               new HandAction("Dbcee89", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("BlackH0L3", HandActionType.FOLD, 0, Street.Preflop),
                               new HandAction("Cockatrice", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("BONUS 1OOO", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("fyabcf", HandActionType.FOLD, 0m, Street.Preflop),
                               new WinningsAction("kliketiklok", HandActionType.WINS, 1, 0),
                               new HandAction("kliketiklok", HandActionType.UNCALLED_BET,0.5m, Street.Showdown), // 1 ( BB ) - 0.5 ( SB )
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("BONUS 1OOO", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                               new HandAction("fyabcf", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                               new HandAction("kliketiklok", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("Dbcee89", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("BlackH0L3", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("Cockatrice", HandActionType.FOLD, 0m, Street.Preflop),                        
                               new HandAction("BONUS 1OOO", HandActionType.RAISE, 1.50m, Street.Preflop),                              
                               new HandAction("fyabcf", HandActionType.RAISE, 5m, Street.Preflop),                              
                               new HandAction("BONUS 1OOO", HandActionType.FOLD, 0, Street.Preflop),
                               new WinningsAction("fyabcf", HandActionType.WINS, 4m, 0),                           
                               new HandAction("fyabcf", HandActionType.UNCALLED_BET, 4m, Street.Showdown), // 5 + 1 (BB) - 1.5 - 0.5(SB)          
                           };
            }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("kliketiklok", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                               new HandAction("Dbcee89", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                               new HandAction("BlackH0L3", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("Cockatrice", HandActionType.RAISE, 3m, Street.Preflop),
                               new HandAction("BONUS 1OOO", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("fyabcf", HandActionType.CALL, 3m, Street.Preflop),                        
                               new HandAction("kliketiklok", HandActionType.RAISE,10m, Street.Preflop),
                               new HandAction("Dbcee89", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("Cockatrice", HandActionType.FOLD, 0m, Street.Preflop),
                               new HandAction("fyabcf", HandActionType.RAISE, 23m, Street.Preflop),    
                               new HandAction("kliketiklok", HandActionType.CALL,  15.5m, Street.Preflop),

                               new HandAction("kliketiklok", HandActionType.CHECK,  0m, Street.Flop),
                               new HandAction("fyabcf", HandActionType.BET, 18m, Street.Flop),    
                               new HandAction("kliketiklok", HandActionType.RAISE,  36m, Street.Flop),
                               new AllInAction("fyabcf", 192.15m, Street.Flop,true),    
                               new AllInAction("kliketiklok", 50.91m, Street.Flop,false),    
                               
                               new WinningsAction("fyabcf", HandActionType.WINS, 226.82m, 0),
                               new HandAction("fyabcf", HandActionType.UNCALLED_BET,  123.24m, Street.Showdown), // 18 + 192.15 - 36 - 50.91
                           };
            }
        }

        protected override List<HandAction> ExpectedOmahaHiLoHand
        {
            get
            {
                return new List<HandAction>()
                           {
                               new HandAction("McCall901", HandActionType.SMALL_BLIND, 5m, Street.Preflop),
                               new HandAction("alikator21", HandActionType.BIG_BLIND, 5m, Street.Preflop),
                               new HandAction("McCall901", HandActionType.RAISE, 5m, Street.Preflop),
                               new HandAction("alikator21", HandActionType.RAISE, 25m, Street.Preflop),
                               new HandAction("McCall901", HandActionType.CALL, 20m, Street.Preflop),

                               new HandAction("alikator21", HandActionType.BET, 60, Street.Flop),                        
                               new HandAction("McCall901",  HandActionType.RAISE, 240, Street.Flop),
                               new AllInAction("alikator21",  265, Street.Flop, true),
                               new HandAction("McCall901", HandActionType.CALL, 85, Street.Flop),

                               new WinningsAction("alikator21", HandActionType.WINS, 354.50m, 0),                               
                               new WinningsAction("McCall901", HandActionType.WINS, 354.50m, 0),                               
                           };
            }
        }

        [Test]
        public void PlayerNameWithDashes_Works()
        {
            List<HandAction> expectedActions = new List<HandAction>()
                                    {
                                        new HandAction("Dbcee89", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop),
                                        new HandAction("BlackH0L3", HandActionType.BIG_BLIND, 1m, Street.Preflop),
                                        new HandAction("---Cockatrice---", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("BONUS 1OOO", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("fyabcf", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("kliketiklok", HandActionType.RAISE, 2m, Street.Preflop),
                                        new HandAction("Dbcee89", HandActionType.FOLD, 0, Street.Preflop),
                                        new HandAction("BlackH0L3", HandActionType.FOLD, 0, Street.Preflop),
                                        new WinningsAction("kliketiklok", HandActionType.WINS, 2.5m, 0),
                                        new HandAction("kliketiklok", HandActionType.UNCALLED_BET, 1m, Street.Showdown), // 2 - 1 ( BB )
                                    };

            TestParseActions("NameWithDashes", expectedActions);
        }
    }
}
