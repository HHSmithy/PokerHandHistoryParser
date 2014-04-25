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

        [Test]
        public void ParsePlayers_WaitingBB()
        {
            //Seat 1: Garzvorgh (12.79).
            //Seat 2: Cellar door (16.60).
            //Seat 3: Tr0m (25.35).
            //Seat 4: bubblebubble (25.80).
            //Seat 5: Dutch1066 (10.52).
            //Seat 6: ButtonSmasher (10.14).
            //Player ButtonSmasher has small blind (0.10)
            //Player Garzvorgh has big blind (0.25)
            //Player bubblebubble is timed out.
            //Player bubblebubble wait BB
            PlayerList players = new PlayerList()
                           {
                               new Player("Garzvorgh", 12.79m, 1),
                               new Player("Cellar door", 16.60m, 2),
                               new Player("Tr0m", 25.35m, 3),
                               new Player("bubblebubble", 25.80m, 4)
                               {
                                   IsSittingOut = true
                               },
                               new Player("Dutch1066", 10.52m, 5),
                               new Player("ButtonSmasher", 10.14m, 6)
                           };
            TestParsePlayers("WaitingBB", players);
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
                return new PlayerList()
                {
                    new Player("Ra1syDa1sy", 182.22m, 1),
                    new Player("sweetdough", 738.20m, 2),
                    new Player("NoahSDsDad", 427.75m, 3),
                    new Player("xx45809", 445.54m, 4)
                    {
                        IsSittingOut = true
                    },
                    new Player("megadouche", 397.56m, 5)
                    {
                        IsSittingOut = true
                    },
                    new Player("KiWiKaKi", 844.29m, 6),
                };
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
