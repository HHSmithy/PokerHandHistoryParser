using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.RunItTwiceTests
{
    [TestFixture]
    class FullTiltRunItTwiceTests : HandParserRunItTwiceTests
    {
        public FullTiltRunItTwiceTests()
            : base("FullTilt")
        {
        }

        [Test]
        public void RunItTwiceTest_1()
        {
            var expectedActions = new List<HandAction>()
            {
                new WinningsAction("1mperial", HandActionType.WINS, 311m, 0),
            };

            RunItTwiceTest(expectedActions, "Td 3d 3h Qh 2h", "RunItTwice1");
        }
    }
}
