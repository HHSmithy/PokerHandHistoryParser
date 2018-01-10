﻿using System;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using NUnit.Framework;
using System.Collections.Generic;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    [TestFixture]
    class HandParserPlayersTestsBossMediaPokerImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsBossMediaPokerImpl()
            : base("BossMedia")
        {
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("mintkyss", 11111m, 1)
                               {
                                   IsSittingOut = true
                               },
                               new Player("Phyre", 13922m, 2),
                               new Player("AllinAnna", 13510m, 3),
                               new Player("ItalyToast", 10000m, 4),
                               new Player("SAMERRRR", 12745m, 5),
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("verner2", 392.06m, 2),
                               new Player("puki", 808.97m, 3),
                               new Player("Rob Whidey", 541.56m, 4)
                               {
                                   HoleCards = HoleCards.ForHoldem
                                   (
                                   new Card('7', 'c'),
                                   new Card('8', 'c')
                                   )
                               },
                               new Player("ItalyToast", 195m, 5)
                               {
                                    HoleCards = HoleCards.ForHoldem
                                    (
                                    new Card('K', 'h'),
                                    new Card('J', 'c')
                                    )
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
                               new Player("Player1", 2485.94m, 1),
                               new Player("Player2", 2491.77m, 2),
                               new Player("Player3", 7175.04m, 3),
                               new Player("Player4", 2375.79m, 4)
                               {
                                   HoleCards = HoleCards.ForOmaha
                                   (
                                   new Card('8', 's'),
                                   new Card('2', 'd'),
                                   new Card('T', 'd'),
                                   new Card('9', 's')
                                   )
                               },
                               new Player("Player5", 440.01m, 5)
                               {
                                   HoleCards = HoleCards.ForOmaha
                                   (
                                   new Card('A', 's'),
                                   new Card('Q', 's'),
                                   new Card('Q', 'c'),
                                   new Card('8', 'd')
                                   )
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
                    new Player("Player1", 56.95m, 1),
                    new Player("HERO", 116.9m, 2)
                    {
                        HoleCards = HoleCards.FromCards("8h6sQdTh")
                    },
                    new Player("Player3", 120.86m, 3)
                    {
                        IsSittingOut = true
                    },
                };
            }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get { throw new NotImplementedException(); }
        }

        [Test]
        public void ParsePlayers_STATE_RESERVED()
        {
            var expected = new PlayerList(new List<Player>()
            {
                new Player("Player1", 0m, 1)
                {
                    IsSittingOut = true,
                },
                new Player("HERO", 175.44m, 2)
                {
                    HoleCards = HoleCards.FromCards("7c2dKd4s")
                },
                new Player("Player4", 466.18m, 4)
                {
                    IsSittingOut = true,
                },
                new Player("Player5", 202.34m, 5),
            });

            TestParsePlayers("STATE_RESERVED", expected);
        }

        [Test]
        public void ParsePlayers_StrangePlayerName()
        {
            var expected = new PlayerList(new List<Player>()
            {
                new Player("&&££ÖÖ", 400m, 1),
                new Player("L'POOL", 60m, 2)
                {
                    HoleCards = HoleCards.FromCards("9s9hQhJs")
                },
                new Player("Player3", 200m, 3)
                {
                    HoleCards = HoleCards.FromCards("JdJhKc7h")
                },
                new Player("Player4", 470m, 4),
                new Player("Player5", 90m, 5),
            });

            TestParsePlayers("StrangePlayerNames", expected);
        }
    }
}