using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.CommunityCardTests
{
    [TestFixture("PartyPoker")]
    [TestFixture("PokerStars")]
    [TestFixture("OnGame")]
    [TestFixture("IPoker")]
    [TestFixture("Pacific")]
    [TestFixture("Merge")]
    [TestFixture("Entraction")]
    [TestFixture("FullTilt")]
    [TestFixture("MicroGaming")]
    [TestFixture("Winamax")]
    [TestFixture("WinningPoker")]
    [TestFixture("WinningPoker", 2, false, false, false, true)]
    [TestFixture("BossMedia")]
    [TestFixture("IGT")]
    internal class HandParserCommunityCardTests : HandHistoryParserBaseTests
    {
        int testNumber;
        bool testPreflop = true;
        bool testFlop = true;
        bool testTurn = true;
        bool testRiver = true;

        public HandParserCommunityCardTests(string site)
            : base(site)
        {
            this.testNumber = 1;
        }

        public HandParserCommunityCardTests(string site, int testNumber, bool preflop, bool flop, bool turn, bool river)
            : base(site)
        {
            this.testNumber = testNumber;
            this.testPreflop = preflop;
            this.testFlop = flop;
            this.testTurn = turn;
            this.testRiver = river;
        }

        private void TestBoard(BoardCards expectedBoard)
        {
            string handText = SampleHandHistoryRepository.GetCommunityCardsHandHistoryText(PokerFormat.CashGame, Site, expectedBoard.Street, testNumber);
           
            Assert.AreEqual(expectedBoard, GetParser().ParseCommunityCards(handText));
        }

        [Test]
        public void ParseCommunityCards_Preflop()
        {
            if (!testPreflop)
            {
                return;
            }
            TestBoard(BoardCards.ForPreflop());
        }

        [Test]
        public void ParseCommunityCards_Flop()
        {
            if (!testFlop)
            {
                return;
            }
            TestBoard(BoardCards.ForFlop(Card.Parse("7h"), Card.Parse("Qs"), Card.Parse("3c")));
        }

        [Test]
        public void ParseCommunityCards_Turn()
        {
            if (!testTurn)
            {
                return;
            }
            TestBoard(BoardCards.ForTurn(Card.Parse("Kc"), Card.Parse("Ah"), Card.Parse("7c"), Card.Parse("3d")));
        }

        [Test]
        public void ParseCommunityCards_River()
        {
            if (!testRiver)
            {
                return;
            }
            TestBoard(BoardCards.ForRiver(Card.Parse("5d"), Card.Parse("Ks"), Card.Parse("7c"), Card.Parse("Jc"), Card.Parse("7d")));
        }
    }
}
