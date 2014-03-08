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
                               new Player("alexhind1980", 6m, 1),
                               new Player("expos299", 64.84m, 2),
                               new Player("FLopDonkey", 55.40m, 9)
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {                                
                               new Player("ManakaReg", 128.38m, 2)
                               {
                                       HoleCards = HoleCards.FromCards("7s8c")
                                   },                               
                               new Player("Akisto", 267.90m, 6){
                                       HoleCards = HoleCards.FromCards("AcTc")
                                   },                                                                
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get
            {
                return new PlayerList()
                           {                                
                               new Player("clarefrush", 2.46m, 5)
                               {
                                       HoleCards = HoleCards.FromCards("6s6hKc7h")
                                   },                               
                               new Player("IbetIgotYou", 1.50m, 6){
                                       HoleCards = HoleCards.FromCards("4hTh6dQc")
                                   },                                                                
                           };
            }
        }

        protected override PlayerList ExpectedOmahaShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {    
                               new Player("v0rtexx", 100m, 1),
                               new Player("AndyGlaeser", 121.13m, 2)
                                   {
                                       HoleCards = HoleCards.FromCards("9hQhTs9h")
                                   },
                               new Player("777set ", 74.99m, 4)
                                   {
                                       HoleCards = HoleCards.FromCards("Kc3c7hKs")
                                   },
                               new Player("Silbercherry ", 117m, 6),
                               new Player("T.R.N", 83.49m, 7),               
                               new Player("Borkot091 ", 100m, 8)                                    
                           };
            }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
