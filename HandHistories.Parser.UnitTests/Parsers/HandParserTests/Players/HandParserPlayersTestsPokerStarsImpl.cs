using System;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    [TestFixture]
    class HandParserPlayersTestsPokerStarsImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsPokerStarsImpl() : base("PokerStars")
        {
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
