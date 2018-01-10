using System;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using NUnit.Framework;
using System.Collections.Generic;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    [TestFixture]
    class HandParserPlayersTestsIGTPokerImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsIGTPokerImpl()
            : base("IGT")
        {
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                Assert.Ignore();
                throw new NotImplementedException();
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                Assert.Ignore();
                throw new NotImplementedException();
            }
        }

        protected override PlayerList ExpectedOmahaShowdownPlayers
        {
            get
            {
                return new PlayerList()
                {
                    new Player("HERO", 19.50m, 1)
                    {
                        HoleCards = HoleCards.FromCards("Jh9d4dTc")
                    },
                    new Player("PLAYER2", 59.22m, 2),
                    new Player("PLAYER3", 134.67m, 3),
                    new Player("PLAYER4", 64.49m, 4)
                    {
                        HoleCards = HoleCards.FromCards("3s4s5sAs")
                    },
                    new Player("PLAYER5", 99.17m, 5),
                };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get 
            {
                Assert.Ignore();
                throw new NotImplementedException();
            }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get
            {
                Assert.Ignore();
                throw new NotImplementedException();
            }
        }
    }
}