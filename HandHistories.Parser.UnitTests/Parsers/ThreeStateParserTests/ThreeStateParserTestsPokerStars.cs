using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using NUnit.Framework;
using HandHistories.Parser.Parsers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.ThreeStateParserTests
{
    class ThreeStateParserTestsPokerStars : ThreeStateParserTests
    {
        public ThreeStateParserTestsPokerStars()
            : base("PokerStars")
        {
        }

        protected override List<HandAction> ExpectedHandActionsAnte
        {
            get
            {
                return new List<HandAction>()
                    {
                        new HandAction("Player2", HandActionType.ANTE, 0.5m, Street.Preflop),
                        new HandAction("Player3", HandActionType.ANTE, 0.5m, Street.Preflop),
                        new HandAction("Player2", HandActionType.SMALL_BLIND, 1, Street.Preflop),
                        new HandAction("Player3", HandActionType.BIG_BLIND, 2, Street.Preflop),
                    };
            }
        }

        protected override List<HandAction> ExpectedShowDownActions_Wins
        {
            get
            {
                return new List<HandAction>()
                    {
                        new HandAction("numbush", HandActionType.SHOW, Street.Showdown),
                        new HandAction("matze1987", HandActionType.SHOW, Street.Showdown),
                        new WinningsAction("numbush", HandActionType.WINS, 23.57m, 0),
                    };
            }
        }

        [Test]
        public void ExpectedShowDownActions_AllInFlop()
        {
            var actions = new List<HandAction>()
                    {
                        new HandAction("danfiu", HandActionType.SHOW, Street.Showdown),
                        new HandAction("KENZA_MILOU", HandActionType.SHOW, Street.Showdown),
                        new WinningsAction("danfiu", HandActionType.WINS, 1097.32m, 0),
                    };
            TestShowDownActions("AllinFlop", actions);
        }

        public void TestRunItTwice()
        {
            Assert.Throws<RunItTwiceHandException>(RunItTwiceTest);
        }

        void RunItTwiceTest()
        {
            var hand = GetShowDownTest("RunItTwice");

             List<HandAction> actions = new List<HandAction>();
            parser.ParseShowDown(hand, ref actions, 0, Objects.GameDescription.GameType.Unknown);
        }
    }
}
