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
    class HandParserPlayersTestsEntractionImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsEntractionImpl() : base("Entraction")
        {
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("GrossoGatto", 100, 4),
                               new Player("ORFEO", 48.50m, 5),
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Lechmajstr", 118.62m, 1),
                               new Player("hellowkit", 68.67m, 3)
                                   {
                                       HoleCards = HoleCards.FromCards("3hAd")
                                   },
                               new Player("Kangorooz", 148.50m, 4),
                               new Player("IFeelFree", 100m, 5)
                                   {
                                       HoleCards = HoleCards.FromCards("KhTc")
                                   },
                               new Player("Thx4thatM8", 276.26m, 6),
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
                               new Player("Ballyhoo", 3999.25m, 1)
                                   {
                                       HoleCards = HoleCards.FromCards("JdQh6sAc")
                                   },
                               new Player("YoOyYo", 10377.37m, 2)
                                   {
                                       HoleCards = HoleCards.FromCards("9d5dAd6h")
                                   },
                           };
            }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Ballyhoo", 3199.25m, 1)
                                   {
                                       HoleCards = HoleCards.FromCards("Js5c6h6s")
                                   },
                               new Player("YoOyYo", 11175.87m, 2)
                                   {
                                       HoleCards = HoleCards.FromCards("7d3h4d9c")
                                   },
                           };
            }
        }
    }
}
