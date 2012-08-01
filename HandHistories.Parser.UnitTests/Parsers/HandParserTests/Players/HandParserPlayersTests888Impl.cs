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
    class HandParserPlayersTests888Impl : HandParserPlayersTests
    {
        public HandParserPlayersTests888Impl() : base("Pacific")
        {

        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("playedto", 6001m, 6),
                               new Player("K_E_N_G", 160.90m, 7)
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Velmonio", 1.05m, 1),                                   
                               new Player("bobash14",1.90m, 4),                                   
                               new Player("slyguyone2", 3.27m, 7)
                               {
                                       HoleCards = HoleCards.FromCards("JdAs")
                                   },                               
                               new Player("pervo", 5.08m, 9){
                                       HoleCards = HoleCards.FromCards("9cTd")
                                   },                                                                
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override PlayerList ExpectedOmahaShowdownPlayers
        {
            get { throw new NotImplementedException(); }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
