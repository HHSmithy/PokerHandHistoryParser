using System.Collections.Generic;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    [TestFixture]
    class HandParserPlayersTestsPartyPokerImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsPartyPokerImpl() : base("PartyPoker")
        {
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("D0GgivesHead", 200.00m, 5),
                               new Player("F1SHH11", 201.75m, 6),
                               new Player("NoMJd", 65.85m, 3),                               
                               new Player("l_ . - l - . _l", 213.63m, 4),
                               new Player("magico66666", 175.30m, 2),
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Enslaver", 200.00m, 4),
                               new Player("ImMovingOnUp", 214.40m, 6),
                               new Player("K_flip_L", 54.51m, 3)
                                   {
                                       HoleCards = HoleCards.ForHoldem(Card.Parse("Ah"), Card.Parse("7s"))
                                   },
                               new Player("l_ . - l - . _l", 214.30m, 5)
                                   {
                                       HoleCards = HoleCards.ForHoldem(Card.Parse("Qc"), Card.Parse("Kc"))
                                   },
                               new Player("bloody__9", 273.55m, 2),
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("DirkDiggIer", 5000m, 6)
                               {
                                   IsSittingOut = true
                               },
                               new Player("Kanu7", 5000m, 2),
                               new Player("beebsela", 5000m, 5),
                               new Player("don_perignon", 5000m, 4)
                               {
                                   IsSittingOut = true
                               },
                               new Player("subway96", 5000m, 3)
                               {
                                   IsSittingOut = true
                               },
                               new Player("weeee84", 5000m, 1)
                               {
                                   IsSittingOut = true
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
                               new Player("Call_911", 2186.72m, 2),
                               new Player("TightRaiseR123", 1888.64m, 5),
                               new Player("donkeykong58", 708.75m, 6)
                                   {
                                       HoleCards = HoleCards.ForOmaha(Card.Parse("Kh"), Card.Parse("Qd"), Card.Parse("Qh"), Card.Parse("Kd"))
                                   },
                               new Player("kondilon", 1000.0m, 3)
                                   {
                                       HoleCards = HoleCards.ForOmaha(Card.Parse("3d"), Card.Parse("3h"), Card.Parse("Ac"), Card.Parse("Ah"))
                                   },
                               new Player("m3str3n0va", 1372.39m, 1),
                           };
            }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Scoopydoo81", 701m, 6)
                               {
                                       HoleCards = HoleCards.ForOmaha(Card.Parse("Js"), Card.Parse("4d"), Card.Parse("As"), Card.Parse("9d"))
                               },
                               new Player("janusfaced", 448.25m, 1)
                               {
                                       HoleCards = HoleCards.ForOmaha(Card.Parse("Jh"), Card.Parse("7h"), Card.Parse("9c"), Card.Parse("6d"))
                               },
                               new Player("robertp10", 164.50m, 4),
                               new Player("wembrinator", 105m, 3)
                           };
            }
        }
    }
}