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
    internal class HandParserCommunityCardTests : HandHistoryParserBaseTests
    {
        public HandParserCommunityCardTests(string site)
            : base(site)
        {

        }

        private void TestBoard(BoardCards expectedBoard)
        {
            string handText = SampleHandHistoryRepository.GetCommunityCardsHandHistoryText(PokerFormat.CashGame, Site, expectedBoard.Street);
           
            Assert.AreEqual(expectedBoard, GetParser().ParseCommunityCards(handText));
        }

        [Test]
        public void ParseCommunityCards_Preflop()
        {
            TestBoard(BoardCards.ForPreflop());
        }

        [Test]
        public void ParseCommunityCards_Flop()
        {
            TestBoard(BoardCards.ForFlop(Card.Parse("7h"), Card.Parse("Qs"), Card.Parse("3c")));
        }

        [Test]
        public void ParseCommunityCards_Turn()
        {
            TestBoard(BoardCards.ForTurn(Card.Parse("Kc"), Card.Parse("Ah"), Card.Parse("7c"), Card.Parse("3d")));
        }

        [Test]
        public void ParseCommunityCards_River()
        {
            TestBoard(BoardCards.ForRiver(Card.Parse("5d"), Card.Parse("Ks"), Card.Parse("7c"), Card.Parse("Jc"), Card.Parse("7d")));
        }
    }
}
