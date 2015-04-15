using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.ThreeStateParserTests
{
    class ThreeStateParserTestsFullTilt : ThreeStateParserTests
    {
        public ThreeStateParserTestsFullTilt()
            : base("FullTilt")
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

        protected override bool BlindChatEndingWithNumberTestable
        {
            get
            {
                return true;
            }
        }

        protected override List<HandAction> ExpectedShowDownActions_Wins
        {
            get
            {
                return new List<HandAction>()
                    {
                        new HandAction("jobetzu", HandActionType.SHOW, Street.Showdown),
                        new HandAction("theking881", HandActionType.MUCKS, Street.Showdown),
                        new WinningsAction("jobetzu", HandActionType.WINS, 615.5m, 0),
                    };
            }
        }
    }
}
