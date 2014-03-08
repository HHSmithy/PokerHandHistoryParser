using System.Collections.Generic;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    [TestFixture]
    class HandParserPlayersTestsFullTiltImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsFullTiltImpl()
            : base("FullTilt")
        {
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("jacksparrow8119", 86.55m, 1),
                               new Player("Steamkid79", 65.65m, 2)
                               {
                                   IsSittingOut = true
                               },
                               new Player("kukuruzkuckuck", 42.70m, 3),                               
                               new Player("erki1964", 143.05m, 4),
                               new Player("Jarmuli", 40m, 5),
                               new Player("M_Mathers", 46.55m, 6),
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("burjennio", 30.06m, 1),
                               new Player("Haklot", 17.32m, 2),
                               new Player("14grant", 10.15m, 3)
                                   {
                                       HoleCards = HoleCards.FromCards("KhKs")
                                   },
                               new Player("Caribou0", 9.78m, 4),
                               new Player("Corvus81", 10m, 5),
                               new Player("MUFTI76", 16.67m, 6),
                               new Player("luka-poker", 15.58m, 7)
                                   {
                                       IsSittingOut = true
                                   },
                               new Player("ilya1681", 10m, 8),
                               new Player("OMGStoneAiken", 35.75m, 9),
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("jacksparrow8119", 86.55m, 1),
                               new Player("Steamkid79", 65.65m, 2)
                               {
                                   IsSittingOut = true
                               },
                               new Player("kukuruzkuckuck", 42.70m, 3),                               
                               new Player("erki1964", 143.05m, 4),
                               new Player("Jarmuli", 40m, 5),
                               new Player("M_Mathers", 46.55m, 6),
                           };
            }
        }

        protected override PlayerList ExpectedOmahaShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("VoitikD", 215.35m, 1)
                                   {
                                       HoleCards = HoleCards.FromCards("Qc7d9c7h")
                                   },
                               new Player("Mr_Turban", 38.90m, 2)
                                   {
                                       HoleCards = HoleCards.FromCards("2cJh2s8d")
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
                               new Player("WTFseeB8", 114.45m, 5),
                               new Player("pupsaa", 60.80m, 6)
                               {
                                       HoleCards = HoleCards.FromCards("9s6d7s4c")
                               },
                               new Player("ReydelMundo", 100m, 7)
                               {
                                       HoleCards = HoleCards.FromCards("TdAcAd4d")
                               },
                           };
            }
        }
    }
}