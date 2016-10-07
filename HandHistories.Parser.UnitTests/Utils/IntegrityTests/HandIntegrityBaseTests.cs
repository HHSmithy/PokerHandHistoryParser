using HandHistories.Objects.Actions;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Utils.IntegrityTests
{
    abstract class HandIntegrityBaseTests
    {
        ValidationChecks checks;
        protected HandIntegrityBaseTests(ValidationChecks checks)
        {
            this.checks = checks;
        }

        protected void TestIntegrity(HandHistory hand,  bool valid)
        {
            string reason;
            Assert.AreEqual(valid, HandIntegrity.Check(hand, checks, out reason));
        }

        protected void TestIntegrity(List<HandAction> actions, bool valid)
        {
            var hand = new HandHistory()
            {
                HandActions = actions
            };

            TestIntegrity(hand, valid);
        }
    }
}
