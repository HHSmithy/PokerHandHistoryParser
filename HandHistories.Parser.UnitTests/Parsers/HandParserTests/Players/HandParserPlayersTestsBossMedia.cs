using System;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using NUnit.Framework;

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
                               new Player("mintkyss", 11111m, 1),
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
                               new Player("Supervic", 10985m, 1),
                               new Player("Phyre", 13427m, 2),
                               new Player("AllinAnna", 12995m, 3),
                               new Player("ItalyToast", 8635m, 4)
                               {
                                   HoleCards = HoleCards.ForOmaha
                                   (
                                   new Card('K', 'h'),
                                   new Card('Q', 's'),
                                   new Card('J', 'c'),
                                   new Card('A', 'd')
                                   )
                               },
                               new Player("SAMERRRR", 15972.51m, 5)
                               {
                                   HoleCards = HoleCards.ForOmaha
                                   (
                                   new Card('J', 's'),
                                   new Card('8', 'h'),
                                   new Card('Q', 'h'),
                                   new Card('7', 'd')
                                   )
                               }
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get { throw new NotImplementedException(); }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get { throw new NotImplementedException(); }
        }
    }
}