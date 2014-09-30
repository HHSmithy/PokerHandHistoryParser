using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Tools;

namespace HandHistories.Tools.UnitTests
{
    [TestFixture]
    public class StreetExtensionTest
    {
        void AssertStreetActions(List<HandAction> expected, List<HandAction> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i], "Not equal in index: " + i);
            }
        }

        List<HandAction> TestCase1 = new List<HandAction>()
                {
                    new HandAction("BiatchPeople", HandActionType.SMALL_BLIND, 150m, Street.Preflop),
                    new HandAction("Bluf_To_Much", HandActionType.BIG_BLIND, 300m, Street.Preflop),
                                        
                    new HandAction("Ravenswood13", HandActionType.FOLD, 0m, Street.Preflop),                                     
                    new HandAction("Crazy Elior", HandActionType.RAISE, 600m, Street.Preflop),
                    new HandAction("joiso", HandActionType.RAISE, 900m, Street.Preflop),
                    new HandAction("Ilari FIN", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("LewisFriend", HandActionType.CALL, 900m, Street.Preflop),
                    new HandAction("BiatchPeople", HandActionType.FOLD, 0m, Street.Preflop),
                    new HandAction("Bluf_To_Much", HandActionType.CALL, 600m, Street.Preflop),
                    new HandAction("Crazy Elior", HandActionType.CALL, 300m, Street.Preflop),

                    new HandAction("Bluf_To_Much", HandActionType.CHECK, 0m, Street.Flop),
                    new HandAction("Crazy Elior", HandActionType.CHECK, 0m, Street.Flop),
                    new HandAction("joiso", HandActionType.BET, 300m, Street.Flop),
                    new HandAction("LewisFriend", HandActionType.CALL, 300m, Street.Flop),
                    new HandAction("Bluf_To_Much", HandActionType.CALL, 300m, Street.Flop),
                    new AllInAction("Crazy Elior", 382.50m, Street.Flop, true),
                    new HandAction("joiso", HandActionType.CALL, 82.50m, Street.Flop),
                    new HandAction("LewisFriend", HandActionType.CALL, 82.50m, Street.Flop),
                    new HandAction("Bluf_To_Much", HandActionType.CALL, 82.50m, Street.Flop),
                                        
                    new HandAction("Bluf_To_Much", HandActionType.CHECK, 0m, Street.Turn),
                    new HandAction("joiso", HandActionType.BET, 600m, Street.Turn),
                    new HandAction("LewisFriend", HandActionType.CALL, 600m, Street.Turn),
                    new HandAction("Bluf_To_Much", HandActionType.CALL, 600m, Street.Turn),

                    new HandAction("Bluf_To_Much", HandActionType.BET, 600m, Street.River),
                    new HandAction("joiso", HandActionType.FOLD, 0, Street.River),
                    new HandAction("LewisFriend", HandActionType.RAISE, 1200m, Street.River),
                    new HandAction("Bluf_To_Much", HandActionType.CALL, 600m, Street.River),

                    new HandAction("LewisFriend", HandActionType.SHOW, 0, Street.Showdown),
                    new HandAction("Bluf_To_Much", HandActionType.SHOW, 0, Street.Showdown),
                    new WinningsAction("Bluf_To_Much", HandActionType.WINS_SIDE_POT, 2100, 1),
                    new WinningsAction("LewisFriend", HandActionType.WINS_SIDE_POT, 2100, 1),
                                        
                    new HandAction("Crazy Elior", HandActionType.SHOW, 0, Street.Showdown),
                    new WinningsAction("Crazy Elior", HandActionType.WINS, 2637.50m, 0),
                    new WinningsAction("LewisFriend", HandActionType.WINS, 2637.50m, 0),
                };

        [Test]
        public void GetStreetActions_Preflop()
        {
            List<HandAction> ExpectedActions = new List<HandAction>()
            {
                new HandAction("BiatchPeople", HandActionType.SMALL_BLIND, 150m, Street.Preflop),
                new HandAction("Bluf_To_Much", HandActionType.BIG_BLIND, 300m, Street.Preflop),                   
                new HandAction("Ravenswood13", HandActionType.FOLD, 0m, Street.Preflop),                                     
                new HandAction("Crazy Elior", HandActionType.RAISE, 600m, Street.Preflop),
                new HandAction("joiso", HandActionType.RAISE, 900m, Street.Preflop),
                new HandAction("Ilari FIN", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("LewisFriend", HandActionType.CALL, 900m, Street.Preflop),
                new HandAction("BiatchPeople", HandActionType.FOLD, 0m, Street.Preflop),
                new HandAction("Bluf_To_Much", HandActionType.CALL, 600m, Street.Preflop),
                new HandAction("Crazy Elior", HandActionType.CALL, 300m, Street.Preflop)
            };

            AssertStreetActions(ExpectedActions, TestCase1.Street(Street.Preflop).ToList());
        }

        [Test]
        public void GetStreetActions_Flop()
        {
            List<HandAction> ExpectedActions = new List<HandAction>()
            {
                new HandAction("Bluf_To_Much", HandActionType.CHECK, 0m, Street.Flop),
                new HandAction("Crazy Elior", HandActionType.CHECK, 0m, Street.Flop),
                new HandAction("joiso", HandActionType.BET, 300m, Street.Flop),
                new HandAction("LewisFriend", HandActionType.CALL, 300m, Street.Flop),
                new HandAction("Bluf_To_Much", HandActionType.CALL, 300m, Street.Flop),
                new AllInAction("Crazy Elior", 382.50m, Street.Flop, true),
                new HandAction("joiso", HandActionType.CALL, 82.50m, Street.Flop),
                new HandAction("LewisFriend", HandActionType.CALL, 82.50m, Street.Flop),
                new HandAction("Bluf_To_Much", HandActionType.CALL, 82.50m, Street.Flop)
            };

            AssertStreetActions(ExpectedActions, TestCase1.Street(Street.Flop).ToList());
        }

        [Test]
        public void GetStreetActions_Turn()
        {
            var hand = new HandHistory()
            {
                HandActions = TestCase1
            };

            List<HandAction> ExpectedActions = new List<HandAction>()
            {
                new HandAction("Bluf_To_Much", HandActionType.CHECK, 0m, Street.Turn),
                new HandAction("joiso", HandActionType.BET, 600m, Street.Turn),
                new HandAction("LewisFriend", HandActionType.CALL, 600m, Street.Turn),
                new HandAction("Bluf_To_Much", HandActionType.CALL, 600m, Street.Turn)
            };

            AssertStreetActions(ExpectedActions, TestCase1.Street(Street.Turn).ToList());
        }

        [Test]
        public void GetStreetActions_River()
        {
            List<HandAction> ExpectedActions = new List<HandAction>()
            {
                new HandAction("Bluf_To_Much", HandActionType.BET, 600m, Street.River),
                new HandAction("joiso", HandActionType.FOLD, 0, Street.River),
                new HandAction("LewisFriend", HandActionType.RAISE, 1200m, Street.River),
                new HandAction("Bluf_To_Much", HandActionType.CALL, 600m, Street.River),
            };

            AssertStreetActions(ExpectedActions, TestCase1.Street(Street.River).ToList());
        }
    }
}