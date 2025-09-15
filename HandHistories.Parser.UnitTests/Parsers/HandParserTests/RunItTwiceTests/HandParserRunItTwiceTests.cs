using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.RunItTwiceTests
{
    class HandParserRunItTwiceTests : HandHistoryParserBaseTests 
    {
        public HandParserRunItTwiceTests(string site)
            : base(site)
        {
        }

        protected void RunItTwiceTest(List<HandAction> expectedActions, string expectedBoardString, string name)
        {
            BoardCards expectedBoard = BoardCards.FromCards(expectedBoardString);

            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "RunItTwiceTests", name);

            HandHistory actualHand = GetParser().ParseFullHandHistory(handText, true);

            ClassicAssert.AreEqual(expectedBoard, actualHand.RunItTwiceData.Board);
            ClassicAssert.AreEqual(expectedActions, actualHand.RunItTwiceData.Actions);
        }
    }
}
