using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
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
    }
}
