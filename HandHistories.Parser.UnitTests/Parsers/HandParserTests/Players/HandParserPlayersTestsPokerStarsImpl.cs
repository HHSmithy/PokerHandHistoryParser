using System;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using NUnit.Framework;
using System.Collections.Generic;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    [TestFixture]
    class HandParserPlayersTestsPokerStarsImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsPokerStarsImpl() : base("PokerStars")
        {
        }

        [Test]
        public void ParsePlayers_FiveCardOmaha()
        {
            var expected = new PlayerList(new List<Player>()
            {
                new Player("madmax84", 600m, 3)
                {
                    HoleCards = HoleCards.FromCards("As5h4sAdJd")
                },
                new Player("manics16", 250m, 4)
                {
                    HoleCards = HoleCards.FromCards("9s7cTcAc3h")
                },
            });

            TestParsePlayers("FiveCardOmaha", expected);
        }

        [Test]
        public void ParsePlayers_ShowWithoutShowdown()
        {
            var expected = new PlayerList(new List<Player>()
            {
                new Player("TheHoboKing", 187.76m, 1),
                new Player("gio_pot7", 1312.61m, 2),
                new Player("princepemega", 237.45m, 3),
                new Player("Chip-phage", 400m, 4)
                {
                    HoleCards = HoleCards.ForOmaha(
                    new Card('2', 'c'), 
                    new Card('7', 'd'),
                    new Card('8', 'c'),
                    new Card('Q', 'h'))
                },
                new Player("YoungAKGun", 657.14m, 5),
                new Player("Zsipali", 432.12m, 6),
            });

            TestParsePlayers("ShowWithoutShowdown", expected);
        }

        [Test]
        public void ParsePlayers_RunItTwice()
        {
            var expected = new PlayerList(new List<Player>()
            {
                new Player("Player1", 1000, 1)
                {
                    HoleCards = HoleCards.FromCards("Qh3cKs7c")
                },
                new Player("Player2", 1859.80m, 2),
                new Player("Player3", 2000m, 3)
                {
                    HoleCards = HoleCards.FromCards("JdTsQdQs")
                },
                new Player("Player4", 1000m, 4)
                {
                    HoleCards = HoleCards.FromCards("7s9s7hJh")
                },
                new Player("Player5", 2000m, 5),
                new Player("Player6", 2000m, 6),
            });

            TestParsePlayers("RunItTwice", expected);
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("redskruf7", 1136.30m, 1),
                               new Player("BoomDoon", 1685m, 2),
                               new Player("Zockermicha", 1657m, 4),                               
                               new Player("postler812", 535.40m, 5),
                               new Player("jimmyhoo", 2100m, 6),
                               new Player("filushh", 907m, 8),
                               new Player("mecha0117", 1000m, 9),
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("redskruf7", 1151.30m, 1),
                               new Player("BoomDoon", 1695m, 2),
                               new Player("Carster", 1048.82m, 3),  
                               new Player("Zockermicha", 1642m, 4),                               
                               new Player("postler812", 620.40m, 5),
                               new Player("jimmyhoo", 1693.92m, 6)
                                   {
                                       HoleCards = HoleCards.FromCards("7h6h")
                                   },
                               new Player("EASSA", 719.36m, 7),
                               new Player("filushh", 932m, 8),
                               new Player("mecha0117", 1000m, 9),
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get { throw new NotImplementedException(); }
        }

        protected override PlayerList ExpectedOmahaShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("MajinVeta", 108.23m, 1)
                                   {
                                       HoleCards =  HoleCards.FromCards("7d6s9s7h")
                                   },  
                               new Player("Dracospinner", 434.77m, 2)
                                   {
                                       HoleCards =  HoleCards.FromCards("3d5s6hKd")
                                   }                             
                           };
            }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("besteffect+", 7.06m, 1),
                               new Player("soma-ruse", 41.16m, 2),                                   
                               new Player("topspud", 10.05m, 3),
                               new Player("HELVER4728", 9.65m, 5)
                                   {
                                       HoleCards = HoleCards.FromCards("3s9dKdQs")
                                   },                               
                               new Player("immo 1000", 8.86m, 6)
                                   {
                                       HoleCards = HoleCards.FromCards("QcQh9hTs")
                                   },                                                           
                           };
            }
        }
    }
}
