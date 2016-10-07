using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Parser.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Utils.IntegrityTests
{
    [TestFixture]
    class StreetIntegrityTests : HandIntegrityBaseTests
    {
        public StreetIntegrityTests()
            : base(ValidationChecks.STREET_ORDER)
        {
        }

        [TestCase]
        public void TestInvalidStreet_1()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Preflop),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Flop),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Turn),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Showdown),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.River),
            };

            TestIntegrity(actions, false);
        }

        [TestCase]
        public void TestInvalidStreet_2()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Preflop),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Flop),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.River),
            };

            TestIntegrity(actions, false);
        }

        [TestCase]
        public void TestInvalidStreet_3()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Preflop),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Turn),
            };

            TestIntegrity(actions, false);
        }

        [TestCase]
        public void TestValidStreet_1()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Preflop),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Flop),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Turn),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.River),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Showdown),
            };

            TestIntegrity(actions, true);
        }

        [TestCase]
        public void TestValidStreet_2()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Preflop),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Flop),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Turn),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Showdown),
            };

            TestIntegrity(actions, true);
        }

        [TestCase]
        public void TestValidStreet_3()
        {
            List<HandAction> actions = new List<HandAction>()
            {
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Preflop),
                new HandAction("", HandActionType.UNKNOWN, 0, Street.Showdown),
            };

            TestIntegrity(actions, true);
        }
    }
}
