using System;
using NUnit.Framework;
using HandHistories.Objects.Actions;
using System.Collections.Generic;
using HandHistories.Objects.Cards;
using HandHistories.Utilities;
using HandHistories.Objects.Hand;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;

namespace HandHistories.Objects.UnitTests.Utils
{
    [TestFixture]
    public class PotUtilityTest
    {
        public static List<HandAction> TestActions1 = 
            new List<HandAction>{ 
                new HandAction("yrrrhh33", HandActionType.SMALL_BLIND, 1m, Street.Preflop, 0),
                new HandAction("lhjynfobn", HandActionType.BIG_BLIND, 2m, Street.Preflop, 1),
                new HandAction("igalo1979", HandActionType.CALL, 2m, Street.Preflop, 2),
                new HandAction("YienRang", HandActionType.FOLD, 0, Street.Preflop, 3),
                new HandAction("lillil32", HandActionType.RAISE, 6m, Street.Preflop, 4),
                new HandAction("pepealas5", HandActionType.FOLD, 0, Street.Preflop, 5),
                new HandAction("yrrrhh33", HandActionType.CALL, 5m, Street.Preflop, 6),
                new HandAction("lhjynfobn", HandActionType.FOLD, 0, Street.Preflop, 7),
                new HandAction("igalo1979", HandActionType.CALL, 4m, Street.Preflop, 8),
                new HandAction("yrrrhh33", HandActionType.CHECK, 0m, Street.Flop, 9),
                new HandAction("igalo1979", HandActionType.CHECK, 0m, Street.Flop, 10),
                new HandAction("lillil32", HandActionType.BET, 18m, Street.Flop, 11),
                new HandAction("yrrrhh33", HandActionType.FOLD, 0m, Street.Flop, 12),
                new HandAction("igalo1979", HandActionType.CALL, 18m, Street.Flop, 13),
                new HandAction("igalo1979", HandActionType.CHECK, 0m, Street.Turn, 14),
                new HandAction("lillil32", HandActionType.CHECK, 0m, Street.Turn, 15),
                new HandAction("igalo1979", HandActionType.CHECK, 0m, Street.River, 16),
                new HandAction("lillil32", HandActionType.BET, 30m, Street.River, 17),
                new HandAction("igalo1979", HandActionType.CALL, 30m, Street.River, 18),
                new HandAction("lillil32", HandActionType.SHOW, 0, Street.Showdown, 19),
                new WinningsAction("lillil32", HandActionType.WINS, 113m, 0, 20),     
            };

        [Test]
        public void Utilities_GetPot()
        {
            Assert.AreEqual(3M, PotUtility.GetPot(TestActions1, 2, false));
            Assert.AreEqual(5M, PotUtility.GetPot(TestActions1, 2, true));
            Assert.AreEqual(16M, PotUtility.GetPot(TestActions1, 7, false));
            Assert.AreEqual(16M, PotUtility.GetPot(TestActions1, 7, true));
            Assert.AreEqual(16M, PotUtility.GetPot(TestActions1, 7, false));
            Assert.AreEqual(16M, PotUtility.GetPot(TestActions1, 7, true));
            Assert.AreEqual(56M, PotUtility.GetPot(TestActions1, 17, false));
            Assert.AreEqual(86M, PotUtility.GetPot(TestActions1, 17, true));
            Assert.AreEqual(116M, PotUtility.GetPot(TestActions1, 20, false));
            Assert.AreEqual(116M, PotUtility.GetPot(TestActions1, 20, true));
        }

        [Test]
        public void Utilities_PotSizeInBBs()
        {
            Limit HH = Limit.FromSmallBlindBigBlind(1M, 2M, Currency.USD);
            Assert.AreEqual(PotUtility.StackSizeInBBs(HH, new Player("", 200M, 2)), 100M);
        }

        [Test]
        public void Utilities_GetActionPotSize()
        {
            Limit HH = Limit.FromSmallBlindBigBlind(1M, 2M, Currency.USD);
            Assert.AreEqual(Math.Round(PotUtility.GetActionPotSize(TestActions1, TestActions1[17]) * 100, 2), 53.57M);
            Assert.AreEqual(Math.Round(PotUtility.GetActionPotSize(TestActions1, TestActions1[18]) * 100, 2), 34.88M);
        }
    }
}