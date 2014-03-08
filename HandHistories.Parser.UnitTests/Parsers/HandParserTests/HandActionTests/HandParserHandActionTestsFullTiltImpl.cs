using System;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.HandActionTests
{
    [TestFixture]
    class HandParserHandActionTestsFullTiltImpl : HandParserHandActionTests
    {
        public HandParserHandActionTestsFullTiltImpl()
            : base("FullTilt")
        {
        }

        protected override List<HandAction> ExpectedHandActionsBasicHand
        {
            get
            {
                Assert.Ignore("Hand Actions not implemented");
                throw new NotImplementedException();
            }
        }

        protected override List<HandAction> ExpectedHandActionsFoldedPreflop
        {
            get
            {
                Assert.Ignore("Hand Actions not implemented");
                throw new NotImplementedException();
            }
        }

        protected override List<HandAction> ExpectedHandActions3BetHand
        {
            get
            {
                Assert.Ignore("Hand Actions not implemented");
                throw new NotImplementedException();
            }
        }

        protected override List<HandAction> ExpectedHandActionsAllInHand
        {
            get
            {
                Assert.Ignore("Hand Actions not implemented");
                throw new NotImplementedException();
            }
        }

        protected override List<HandAction> ExpectedOmahaHiLoHand
        {
            get
            {
                Assert.Ignore("Hand Actions not implemented");
                throw new NotImplementedException();
            }
        }
    }
}