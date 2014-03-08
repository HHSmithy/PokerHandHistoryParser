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
    }
}
