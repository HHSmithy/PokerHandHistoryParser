using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.MicroGaming
{
    class MicroGamingFastParserExtraTests : HandHistoryParserBaseTests 
    {
        public MicroGamingFastParserExtraTests()
            : base("MicroGaming")
        {
        }

        [Test]
        public void TestCancelledHand()
        {
            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "ExtraHands", "CancelledHand");

            HandHistory actualHand = GetParser().ParseFullHandHistory(handText, true);
        }
    }
}
