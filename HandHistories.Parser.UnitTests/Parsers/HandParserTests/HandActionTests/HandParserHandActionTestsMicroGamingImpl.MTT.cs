using System;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using NUnit.Framework;
using HandHistories.Objects.GameDescription;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.HandActionTests
{
    [TestFixture]
    class HandParserHandActionTestsMicroGamingImpl_MTT : HandParserHandActionTests
    {

        public HandParserHandActionTestsMicroGamingImpl_MTT()
             : base(PokerFormat.MultiTableTournament, "MicroGaming")
        {
        }

        [Test]
        public void TestActions_SBisAllin()
        {
            var actions = new List<HandAction>()
            {
                new HandAction("Player7", HandActionType.SMALL_BLIND, 10m, Street.Preflop, true),
                new HandAction("Player8", HandActionType.BIG_BLIND, 20m, Street.Preflop),
                new HandAction("Player9", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("Player1", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("Player2", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("Player3", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("Player4", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("HERO", HandActionType.RAISE, 40, Street.Preflop),
                new HandAction("Player8", HandActionType.FOLD, 0, Street.Preflop),
                new HandAction("HERO", HandActionType.UNCALLED_BET, 20, Street.Preflop),
                new HandAction("HERO", HandActionType.SHOW, 0, Street.Showdown),
                new HandAction("Player7", HandActionType.SHOW, 0, Street.Showdown),
                new HandAction("HERO", HandActionType.SHOW, 0, Street.Showdown),
                new WinningsAction("HERO", HandActionType.WINS, 20m, 0),
                new WinningsAction("Player7", HandActionType.WINS, 30m, 0), 
            };

            TestParseActions("SBisAllin", actions);
        }

         protected override List<HandAction> ExpectedHandActionsBasicHand
         {
             get
             {
                 Assert.Ignore();
                 throw new NotImplementedException();
             }
         }

         protected override List<HandAction> ExpectedHandActionsFoldedPreflop
         {
             get
             {
                 Assert.Ignore();
                 throw new NotImplementedException();
             }
         }

         protected override List<HandAction> ExpectedHandActions3BetHand
         {
             get
             {
                 Assert.Ignore();
                 throw new NotImplementedException();
             }
         }

         protected override List<HandAction> ExpectedHandActionsAllInHand
         {
             get
             {
                 Assert.Ignore();
                 throw new NotImplementedException();
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

         protected override List<HandAction> ExpectedOmahaHiLoHand
         {
             get
             {
                 Assert.Ignore();
                 throw new NotImplementedException();
             }
         }
    }
}
