using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    [TestFixture]
    internal class HandParserPlayersTestsIPokerImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsIPokerImpl()
            : base("IPoker")
        {
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("WWR141388412", 10.18m, 4),
                               new Player("keepfishing68", 9.61m, 5),
                               new Player("chcake515151", 10m, 6)
                                   {
                                       IsSittingOut = true
                                   }
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("dunny53", 10m, 1)
                                   {
                                       HoleCards = HoleCards.FromCards("KhTs")
                                   },
                               new Player("chcake515151", 12.49m, 6)
                                   {
                                       HoleCards = HoleCards.FromCards("Jc8h")
                                   },                               
                               new Player("gogz8219", 2.87m, 8),                               
                               new Player("dondigidon79", 7.85m, 9),                               
                               new Player("BeenToHell", 9.67m, 10)
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("WWR141388412", 10.18m, 4),
                               new Player("keepfishing68", 9.61m, 5),
                               new Player("chcake515151", 10m, 6)
                                   {
                                       IsSittingOut = true
                                   }
                           };
            }
        }

        protected override PlayerList ExpectedOmahaShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("wonkar1955", 42.10m, 3)
                                   {
                                       HoleCards = HoleCards.FromCards("Ks6d7hQd")
                                   },
                               new Player("IAmJaredTendler", 55.25m, 8)
                                   {
                                       HoleCards = HoleCards.FromCards("TsAh6sQc")
                                   }
                           
                           };
            }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get { throw new NotImplementedException(); }
        }

        [Test]
        public void ParsePlayers_WithShowdown2()
        {
            var expected = new PlayerList(new List<Player>()
            {
                new Player("Player1", 1000, 1) ,
                new Player("Player3", 573.100m, 3)
                {
                    HoleCards = HoleCards.FromCards("6s4d9d8s")
                },
                new Player("Player5", 1000m, 5),
                new Player("Player6", 1000m, 6),
                new Player("Player8", 1275.70m, 8)
                {
                    HoleCards = HoleCards.FromCards("QsAsKcKs")
                },
                new Player("Player10", 667m, 10)
                {
                    HoleCards = HoleCards.FromCards("3dAhAcQh")
                },
            });

            TestParsePlayers("WithShowdown2", expected);
        }
    }
}
