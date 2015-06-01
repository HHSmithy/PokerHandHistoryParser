using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Utils.Pot;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Utils.Pot
{
    [TestFixture]
    public class PotCalculatorTests
    {
        void TestPotCalculator(decimal ExpectedPot, HandHistory hand)
        {
            var calculatedPot = PotCalculator.CalculateTotalPot(hand);
            Assert.AreEqual(ExpectedPot, calculatedPot);
        }

        [TestCase]
        public void PotCalculatorTest_1()
        {
            HandHistory hand = new HandHistory();

            hand.HandActions = new List<HandAction>
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 0.1m, Objects.Cards.Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 0.2m, Objects.Cards.Street.Preflop),
                new HandAction("P1", HandActionType.CALL, 0.1m, Objects.Cards.Street.Preflop),
                new HandAction("P2", HandActionType.CHECK, 0m, Objects.Cards.Street.Preflop),

                new HandAction("P1", HandActionType.CHECK, 0m, Objects.Cards.Street.Flop),
                new HandAction("P2", HandActionType.BET, 0.1m, Objects.Cards.Street.Flop),
                new HandAction("P1", HandActionType.FOLD, 0m, Objects.Cards.Street.Flop),
                new HandAction("P2", HandActionType.UNCALLED_BET, 0.1m, Objects.Cards.Street.Flop),
            };

            TestPotCalculator(0.4m, hand);
        }

        [TestCase]
        public void PotCalculatorTest_2()
        {
            HandHistory hand = new HandHistory();

            hand.HandActions = new List<HandAction>
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 0.1m, Objects.Cards.Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 0.2m, Objects.Cards.Street.Preflop),
                new HandAction("P1", HandActionType.CALL, 0.1m, Objects.Cards.Street.Preflop),
                new HandAction("P2", HandActionType.CHECK, 0m, Objects.Cards.Street.Preflop),

                new HandAction("P1", HandActionType.CHECK, 0m, Objects.Cards.Street.Flop),
                new HandAction("P2", HandActionType.CHECK, 0m, Objects.Cards.Street.Flop),

                new HandAction("P1", HandActionType.BET, 0.2m, Objects.Cards.Street.Turn),
                new HandAction("P2", HandActionType.CALL, 0.2m, Objects.Cards.Street.Turn),

                new HandAction("P1", HandActionType.CHECK, 0m, Objects.Cards.Street.River),
                new HandAction("P2", HandActionType.CHECK, 0m, Objects.Cards.Street.River),

                new WinningsAction("P1", HandActionType.WINS, 0.8m, 0),
            };

            TestPotCalculator(0.8m, hand);
        }

        [TestCase]
        public void PotCalculatorTest_DonkFold()
        {
            HandHistory hand = new HandHistory();

            hand.HandActions = new List<HandAction>
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 0.1m, Objects.Cards.Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 0.2m, Objects.Cards.Street.Preflop),
                new HandAction("P1", HandActionType.CALL, 0.1m, Objects.Cards.Street.Preflop),
                new HandAction("P2", HandActionType.CHECK, 0m, Objects.Cards.Street.Preflop),

                new HandAction("P1", HandActionType.FOLD, 0m, Objects.Cards.Street.Flop),

                new WinningsAction("P2", HandActionType.WINS, 0.4m, 0),
            };

            TestPotCalculator(0.4m, hand);
        }

        [TestCase]
        public void PotCalculatorTest_CallAllIn()
        {
            HandHistory hand = new HandHistory();

            hand.HandActions = new List<HandAction>
            {
                new HandAction("P1", HandActionType.SMALL_BLIND, 1m, Street.Preflop),
                new HandAction("P2", HandActionType.BIG_BLIND, 2m, Street.Preflop),
                new HandAction("P1", HandActionType.RAISE, 10m, Street.Preflop),
                new HandAction("P2", HandActionType.RAISE, 60m, Street.Preflop, true),
                new HandAction("P1", HandActionType.CALL, 29m, Street.Preflop, true),

                new WinningsAction("P2", HandActionType.WINS, 80m, 0),
            };

            TestPotCalculator(80m, hand);
        }
    }
}
