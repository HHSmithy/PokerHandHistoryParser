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
                               new Player("kattiza", 94.10m, 2),
                               new Player("adamsin1", 108.75m, 8),
                               new Player("R1verM0nster", 100m, 9)   
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("kattiza", 107.38m, 2)
                                   {
                                       HoleCards = HoleCards.FromCards("Ad9c")
                                   },     
                               new Player("spir12.", 35.73m, 3),
                                                             
                               new Player("adamsin1", 102m, 8)
                                   {
                                       HoleCards = HoleCards.FromCards("Ah8s")
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
                               new Player("Cockie", 454.05m, 3)
                                   {
                                       HoleCards = HoleCards.FromCards("JdTsTdKd")
                                   },      
                               new Player("zoo0ega", 1666.40m, 4),   
                               new Player("Ronstorm", 498m, 5),    
                               new Player("LagTard_x", 742.05m, 6),     
                               new Player("the_mino", 129.50m, 8)
                                   {
                                       HoleCards = HoleCards.FromCards("Ad4c4dJc")
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
                               new Player("alikator21", 332m, 1)
                                   {
                                       HoleCards = HoleCards.FromCards("AdJs7dAh")
                                   },      
                               new Player("McCall901", 531m, 2)
                                   {
                                       HoleCards = HoleCards.FromCards("2h3cAs9h")
                                   }                                                            
                           };
            }
        }
    }
}
