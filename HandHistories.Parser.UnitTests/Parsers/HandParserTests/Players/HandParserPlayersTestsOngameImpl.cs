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
    class HandParserPlayersTestsOngameImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsOngameImpl()
            : base("OnGame")
        {
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("mr dark hor", 58.62m, 4),
                               new Player("il_conta", 16m, 7),
                               new Player("Nashpan", 17.50m, 8),                               
                               new Player("---ich---", 49.50m, 9),
                               new Player("Incubus633", 45.50m, 10),
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Pan Sytuacji", 20.68m, 3),
                               new Player("mr dark hor", 53.12m, 4)
                                   {
                                       HoleCards = HoleCards.FromCards("Ts3s")
                                   },                               
                               new Player("il_conta", 15.25m, 7)
                                   {
                                       HoleCards = HoleCards.FromCards("QhQs")
                                   },                               
                               new Player("Nashpan", 17.50m, 8),                               
                               new Player("RedStar72", 48.75m, 9),
                               new Player("Incubus633", 54.60m, 10),
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
                               new Player("zatli74", 19.68m, 2)
                                   {
                                       HoleCards = HoleCards.FromCards("9c6s3sAc")
                                   },      
                               new Player("Max Power s", 9.66m, 8)
                                   {
                                       HoleCards = HoleCards.FromCards("4c3c8d6c")
                                   },                               
                               new Player("EvilJihnny99", 28.36m, 9)
                                   {
                                       HoleCards = HoleCards.FromCards("AsKcJh5c")
                                   },                                                             
                           };
            }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
