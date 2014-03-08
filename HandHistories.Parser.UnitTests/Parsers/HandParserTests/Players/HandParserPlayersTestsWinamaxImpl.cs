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
    class HandParserPlayersTestsWinamaxImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsWinamaxImpl()
            : base("Winamax")
        {

        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Booobisar", 2030.75m, 1),
                               new Player("LASCONI4", 1010, 2),
                               new Player("I_M_ALL_IN", 1046.14m, 3),
                               new Player("justlucky", 2723.25m, 4),
                               new Player("arthurxxx", 1238.50m, 5)
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("PornstarX", 67.80m, 1),
                               new Player("nico86190", 38.52m, 2)
                                   {
                                       HoleCards = HoleCards.FromCards("QhQc")
                                   },
                               new Player("-LePianiste-", 53.65m, 3),
                               new Player("LEROISALO", 22.85m, 4),
                               new Player("Matthieu_59_", 42.59m, 5)
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
                               new Player("scyllah374", 30, 1),
                               new Player("RICO97133", 80.89m, 2),
                               new Player("AtomeXxX", 18.85m, 3),
                               new Player("trasto51", 30.77m, 4),
                               new Player("sharon59221", 30.74m, 5)
                                   {
                                       HoleCards = HoleCards.FromCards("AhAs4cTd")
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
