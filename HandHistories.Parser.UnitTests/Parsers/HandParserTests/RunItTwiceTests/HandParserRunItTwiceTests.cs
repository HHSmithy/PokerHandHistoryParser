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
    class HandParserRunItTwiceTests : HandHistoryParserBaseTests 
    {
        public HandParserRunItTwiceTests(string site)
            : base(site)
        {
        }

        protected void RunItTwiceTest(List<HandAction> expectedActions_Run1, List<HandAction> expectedActions_Run2, string expectedBoardString, string name)
        {
            BoardCards expectedBoard = BoardCards.FromCards(expectedBoardString);

            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "RunItTwiceTests", name);

            HandHistory actualHand = GetParser().ParseFullHandHistory(handText, true);

            var Run1ShowdownActions = actualHand.HandActions.Street(Street.Showdown).ToList();

            Assert.AreEqual(expectedBoard, actualHand.RunItTwiceData.Board);
            Assert.AreEqual(expectedActions_Run1, Run1ShowdownActions);
            Assert.AreEqual(expectedActions_Run2, actualHand.RunItTwiceData.Actions);
        }
    }
}
