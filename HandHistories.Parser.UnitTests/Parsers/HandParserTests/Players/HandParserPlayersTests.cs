using System.Collections.Generic;
using System.Linq;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    abstract internal class HandParserPlayersTests : HandHistoryParserBaseTests
    {
        protected HandParserPlayersTests(string site) : base(site)
        {
        }

        protected void TestParsePlayers(string fileName, PlayerList expectedPlayers)
        {
            string handText = SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "PlayerTests", fileName);

            PlayerList playerList = GetParser().ParsePlayers(handText);

            Assert.AreEqual(expectedPlayers.Count, playerList.Count, "Player List Count");
            Assert.AreEqual(string.Join(",", expectedPlayers), string.Join(",", playerList));
        }

        protected abstract PlayerList ExpectedNoHoleCardsPlayers { get; }
        protected abstract PlayerList ExpectedWithShowdownPlayers { get; }
        protected abstract PlayerList ExpectedWithSittingOutPlayers { get; }
        protected abstract PlayerList ExpectedOmahaShowdownPlayers { get; }
        protected abstract PlayerList ExpectedOmahaHiLoShowdownPlayers { get; }
            
        [Test]
        public void ParsePlayers_NoHoleCards()
        {
            TestParsePlayers("NoHoleCards", ExpectedNoHoleCardsPlayers);
        }

        [Test]
        public void ParsePlayers_WithHoleCards()
        {
            TestParsePlayers("WithShowdown", ExpectedWithShowdownPlayers);
        }

        [Test]
        public void ParsePlayers_SittingOutPlayers()
        {
            switch (Site)
            {
                case SiteName.Winamax:
                case SiteName.OnGame:
                case SiteName.Pacific:
                case SiteName.Entraction:
                case SiteName.PartyPoker:
                case SiteName.PokerStars:
                    Assert.Ignore("No sitting out examples for " + Site);
                    break;
            }

            TestParsePlayers("WithSittingOut", ExpectedWithSittingOutPlayers);
        }

        [Test]
        public void ParsePlayers_OmahaShowdown()
        {
            switch (Site)
            {            
                case SiteName.Pacific:
                    Assert.Ignore("No omaha examples for " + Site);
                    break;
            }

            TestParsePlayers("OmahaShowdown", ExpectedOmahaShowdownPlayers);
        }

        [Test]
        public void ParsePlayers_OmahaHiLoShowdown()
        {
            switch (Site)
            {
                case SiteName.Winamax:
                case SiteName.Merge:
                case SiteName.IPoker:
                case SiteName.Pacific:
                    Assert.Ignore("No Hi-Lo examples for " + Site);
                    break;
            }

            TestParsePlayers("OmahaHiLoShowdown", ExpectedOmahaHiLoShowdownPlayers);
        }
    }
}
