using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HandHistories.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.UnitTests.Utils.Serialization
{
    class StreetUtilityTests
    {
        [Test]
        public void Utilities_GetFirstVPIP()
        {
            HandAction HA = StreetUtility.GetFirstVPIPAction(PotUtilityTest.TestActions1);
            Assert.AreEqual(HA.PlayerName, "igalo1979");
        }

        [Test]
        public void Utilities_GetFirstFlop()
        {
            HandAction HA = StreetUtility.GetFirstVPIPAction(StreetUtility.GetStreetActions(PotUtilityTest.TestActions1, Street.Flop));
            Assert.AreEqual(HA.PlayerName, "lillil32");
        }

        [Test]
        public void Utilities_GetFirstTurn()
        {
            HandAction HA = StreetUtility.GetFirstVPIPAction(StreetUtility.GetStreetActions(PotUtilityTest.TestActions1, Street.Turn));
            Assert.AreEqual(HA, null);
        }

        [Test]
        public void Utilities_GetFirstRiver()
        {
            HandAction HA = StreetUtility.GetFirstVPIPAction(StreetUtility.GetStreetActions(PotUtilityTest.TestActions1, Street.River));
            Assert.AreEqual(HA.PlayerName, "lillil32");
        }

        [Test]
        public void Utilities_PlayersOnStreet()
        {
            Assert.AreEqual(StreetUtility.PlayersOnStreet(PotUtilityTest.TestActions1, Street.Preflop), 6);
            Assert.AreEqual(StreetUtility.PlayersOnStreet(PotUtilityTest.TestActions1, Street.Flop), 3);
            Assert.AreEqual(StreetUtility.PlayersOnStreet(PotUtilityTest.TestActions1, Street.Turn), 2);
            Assert.AreEqual(StreetUtility.PlayersOnStreet(PotUtilityTest.TestActions1, Street.River), 2);
        }

        [Test]
        public void Utilities_WentToShowdown()
        {
            HandHistory HH = new Hand.HandHistory() { HandActions = PotUtilityTest.TestActions1 };
            Assert.AreEqual(StreetUtility.HandWentToShowdown(HH), true);

            HH.HandActions = new List<HandAction>{ 
                new HandAction("yrrrhh33", HandActionType.SMALL_BLIND, 1m, Street.Preflop, 0),
                new HandAction("lhjynfobn", HandActionType.BIG_BLIND, 2m, Street.Preflop, 1),
                new HandAction("igalo1979", HandActionType.CALL, 2m, Street.Preflop, 2),
                new HandAction("YienRang", HandActionType.FOLD, 0, Street.Preflop, 3)
            };
            Assert.AreEqual(StreetUtility.HandWentToShowdown(HH), false);
        }

        [Test]
        public void StreeUtilities_GetNextVPIPAction()
        {
            Assert.AreEqual(StreetUtility.GetNextVPIPAction(PotUtilityTest.TestActions1, 0).HandActionType == HandActionType.CALL, true);
            Assert.AreEqual(StreetUtility.GetNextVPIPAction(PotUtilityTest.TestActions1, 4).HandActionType == HandActionType.CALL, true);
            Assert.AreEqual(StreetUtility.GetNextVPIPAction(PotUtilityTest.TestActions1, 9).HandActionType == HandActionType.BET, true);
            Assert.AreEqual(StreetUtility.GetNextVPIPAction(PotUtilityTest.TestActions1, 19), null);
        }

        [Test]
        public void StreeUtilities_FirstAction()
        {
            Assert.AreEqual(StreetUtility.FirstAction(PotUtilityTest.TestActions1, Street.Preflop, "yrrrhh33").HandActionType == HandActionType.SMALL_BLIND, true);
            Assert.AreEqual(StreetUtility.FirstAction(PotUtilityTest.TestActions1, Street.Flop, "yrrrhh33").HandActionType == HandActionType.CHECK, true);
            Assert.AreEqual(StreetUtility.FirstAction(PotUtilityTest.TestActions1, Street.Turn, "yrrrhh33") == null, true);
            Assert.AreEqual(StreetUtility.FirstAction(PotUtilityTest.TestActions1, Street.River, "igalo1979").HandActionType == HandActionType.CHECK, true);
        }

        [Test]
        public void StreeUtilities_GetNextHandActionNumber()
        {
            Assert.AreEqual(StreetUtility.GetNextHandActionNumber(PotUtilityTest.TestActions1, 0).HandActionType == HandActionType.BIG_BLIND, true);
            Assert.AreEqual(StreetUtility.GetNextHandActionNumber(PotUtilityTest.TestActions1, 6).HandActionType == HandActionType.FOLD, true);
            Assert.AreEqual(StreetUtility.GetNextHandActionNumber(PotUtilityTest.TestActions1, 9).HandActionType == HandActionType.CHECK, true);
            Assert.AreEqual(StreetUtility.GetNextHandActionNumber(PotUtilityTest.TestActions1, 16).HandActionType == HandActionType.BET, true);
            Assert.AreEqual(StreetUtility.GetNextHandActionNumber(PotUtilityTest.TestActions1, 19) == null, true);
        }

        [Test]
        public void StreeUtilities_GetNextHandAction()
        {
            Assert.AreEqual(StreetUtility.GetNextHandAction(PotUtilityTest.TestActions1, 0).HandActionType == HandActionType.BIG_BLIND, true);
            Assert.AreEqual(StreetUtility.GetNextHandAction(PotUtilityTest.TestActions1, 6).HandActionType == HandActionType.FOLD, true);
            Assert.AreEqual(StreetUtility.GetNextHandAction(PotUtilityTest.TestActions1, 9).HandActionType == HandActionType.CHECK, true);
            Assert.AreEqual(StreetUtility.GetNextHandAction(PotUtilityTest.TestActions1, 16).HandActionType == HandActionType.BET, true);
            Assert.AreEqual(StreetUtility.GetNextHandAction(PotUtilityTest.TestActions1, 19) == null, true);
        }
    }
}
