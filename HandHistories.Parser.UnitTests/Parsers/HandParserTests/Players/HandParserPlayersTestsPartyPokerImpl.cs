using System;
using System.Collections.Generic;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    [TestFixture]
    class HandParserPlayersTestsPartyPokerImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsPartyPokerImpl() : base("PartyPoker")
        {
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Kelevra_91", 24.49m, 2),
                               new Player("hulkhoden1969", 10m, 5)
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("poket Ass", 8.54m, 1)
                                   {
                                       HoleCards = HoleCards.FromCards("Ks6d")
                                   },
                               new Player("stal1969", 11m, 5)
                                   {
                                       HoleCards = HoleCards.FromCards("7h3c")
                                   }
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
                               new Player("lunaemma", 108.86m, 1)
                                   {
                                       HoleCards = HoleCards.FromCards("3d9c2d6h")
                                   },
                               new Player("pexiM", 100m, 2)
                                   {
                                       HoleCards = HoleCards.FromCards("3hAsQc5s")
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
                               new Player("DrunkSon88", 10m, 2)
                               {
                                       HoleCards = HoleCards.FromCards("2cQs8sKd")
                               },
                               new Player("Spuel2", 3.59m, 1)
                               {
                                       HoleCards = HoleCards.FromCards("JdTs8h5d")
                               }
                           };
            }
        }
    }
}