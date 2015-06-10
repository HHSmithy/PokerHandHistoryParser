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

        protected void RunItTwiceTest(List<HandAction> expectedActions_Run1, List<HandAction> expectedActions_Run2, string expectedBoardString_1, string expectedBoardString_2, string name)
        {
            BoardCards expectedBoard1 = BoardCards.FromCards(expectedBoardString_1);
            BoardCards expectedBoard2 = BoardCards.FromCards(expectedBoardString_2);

            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "RunItTwiceTests", name);

            HandHistory actualHand = GetParser().ParseFullHandHistory(handText, true);

            var Run1ShowdownActions = actualHand.HandActions.Street(Street.Showdown).ToList();

            Assert.AreEqual(expectedBoard1, actualHand.ComumnityCards);
            Assert.AreEqual(expectedBoard2, actualHand.RunItTwiceData.Board);

            Assert.AreEqual(expectedActions_Run1, Run1ShowdownActions);
            Assert.AreEqual(expectedActions_Run2, actualHand.RunItTwiceData.Actions);
        }
    }
}
