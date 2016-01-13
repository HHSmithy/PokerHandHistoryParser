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
    class HandParserPlayersTestsMicroGamingImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsMicroGamingImpl() : base("MicroGaming")
        {

        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Crystalised", 498.30m, 1),
                               new Player("skjoldfz", 438m, 2),
                               new Player("Nnnnn1", 418.00m, 3),
                               new Player("kodeyoung", 161.92m, 4),
                               new Player("ksspara", 439.00m, 5),
                               new Player("BubbleGum2", 406.00m, 6)
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("DuckGhoul", 2.03m, 1)
                                   {
                                       IsSittingOut = true
                                   },
                               new Player("dogugiev", 2.01m, 2),
                               new Player("creys", 0.61m, 3)
                                   {
                                       // KcKh
                                       HoleCards = HoleCards.FromCards("KcKh")
                                   },
                               new Player("4WHawkft", 2.01m, 4),
                               new Player("_joker_", 2.00m, 5),
                               new Player("Jeesuslaps", 2.00m, 6)
                                   {
                                       // 5h5c
                                       HoleCards = HoleCards.FromCards("5h5c")
                                   }                            
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("DuckGhoul", 2.00m, 1),
                               new Player("Str16b8", 2.49m, 2)
                                   {
                                       IsSittingOut = true
                                   },
                               new Player("creys", 0.40m, 3),
                               new Player("4WHawkft", 2.04m, 4),
                               new Player("_joker_", 2.04m, 5),
                               new Player("Jeesuslaps", 1.98m, 6)
                           };
            }
        }

        protected override PlayerList ExpectedOmahaShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Srajdanek", 22.50m, 1),
                               new Player("edisheri", 156.52m, 2),
                               new Player("Justin2010", 153.73m, 3)
                                   {
                                       HoleCards = HoleCards.FromCards("5c3cKsQs")
                                   },
                               new Player("WTFareU", 129.96m, 4),
                               new Player("NisshinMaru", 100.00m, 5),
                               new Player("BunkerMental", 104.02m, 6)
                           };
            }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("uk_munchkin", 1.68m, 3)
                                   {
                                       HoleCards = HoleCards.FromCards("Jc4d2hTc")
                                   },
                               new Player("tigersue",1.98m, 4)
                                   {
                                       HoleCards = HoleCards.FromCards("3s3h5h4c")
                                   },
                               new Player("theweman", 2.74m, 5),
                               new Player("hoop", 0.66m, 6)
                           };
            }
        }

        [Test]
        public void ParsePlayers_Hero()
        {
            var expected = new PlayerList(new List<Player>()
            {
                new Player("Player1", 128.80m, 1),
                new Player("Player2", 532.00m, 2),
                new Player("Hero", 266.40m, 3)
                {
                    HoleCards = HoleCards.FromCards("Qd8hAh6c")
                },
                new Player("Player6", 426.00m, 6)
                {
                    IsSittingOut = true
                },
            });

            TestParsePlayers("Hero", expected);
        }
    }
}
