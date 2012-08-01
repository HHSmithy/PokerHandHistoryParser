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
                               new Player("thaiJhonny", 16.08m, 1),
                               new Player("Kiko991", 15.60m, 2),
                               new Player("Netolip", 16.64m, 3),                               
                               new Player("Eva45", 16m, 4),
                               new Player("miumer", 16.08m, 5),
                               new Player("Sascha9574", 16m, 6),
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Marcotjow", 4384.20m, 3)
                                   {
                                    HoleCards = HoleCards.ForHoldem(Card.Parse("Kh"), Card.Parse("Qh"))
                                   },
                               new Player("WizardOfAhhs", 2692.80m, 4)
                                   {
                                       HoleCards = HoleCards.ForHoldem(Card.Parse("Ad"), Card.Parse("Kd"))
                                   }
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get
            {
                // Stars doesn't say what seat some-one who is sitting out at is
                return new PlayerList()
                           {
                               new Player("neverJa(1)ger", 22.07m, 1),
                               new Player("s1N91", 16.56m, 2),
                               new Player("Blahdi:42eblah", 22.99m, 4),                               
                               new Player("antol34", 16m, 6),                               
                           };
            }
        }

        protected override PlayerList ExpectedOmahaShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("chico134", 736.54m, 1),
                               new Player("jippo79", 600m, 2)
                                   {
                                       HoleCards =  HoleCards.FromCards("4hAs5s7h")
                                   },
                               new Player("cuartito76", 355m, 3)
                                   {
                                       HoleCards =  HoleCards.FromCards("Ac8cJhKh")
                                   },  
                               new Player("Gakn29", 600m, 4),                               
                               new Player("IMALLIN723", 625.24m, 5),                               
                               new Player("foldngst8n", 600m, 6),                               
                           };
            }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("DOT19", 20.23m, 1)
                                   {
                                       HoleCards = HoleCards.FromCards("As8hAcKd")
                                   },
                               new Player("Guffings", 8.79m, 2),                                   
                               new Player("HELVER4728", 10.25m, 3),
                               new Player("mickeyr777", 6.97m, 5),                               
                               new Player("JokerTKD", 12.05m, 6)
                                   {
                                       HoleCards = HoleCards.FromCards("2s3sAhKh")
                                   },                                                           
                           };
            }
        }
    }
}
