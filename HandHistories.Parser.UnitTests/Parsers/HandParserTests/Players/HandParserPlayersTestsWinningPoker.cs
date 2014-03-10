using System;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    [TestFixture]
    class HandParserPlayersTestsWinningPokerImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsWinningPokerImpl()
            : base("WinningPoker")
        {
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("xx59704", 199.71m, 1),
                               new Player("Xavier2500", 86.80m, 4),
                               new Player("Carlos Danger", 445.60m, 6)
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("xx59704", 159.21m, 1)
                               {
                                   HoleCards = HoleCards.FromCards("7s4h")
                               },
                               new Player("Xavier2500", 110.40m, 4)
                               {
                                   HoleCards = HoleCards.FromCards("Th8c")
                               }
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get {
                Assert.Ignore("Hand Actions not implemented");
                throw new NotImplementedException(); 
            }
        }

        protected override PlayerList ExpectedOmahaShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("do not-call", 160m, 1)
                                   {
                                       HoleCards =  HoleCards.FromCards("3c6h7d8c")
                                   },  
                               new Player("digbick30", 297m, 4)
                                   {
                                       HoleCards =  HoleCards.FromCards("QhKcKs4d")
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
