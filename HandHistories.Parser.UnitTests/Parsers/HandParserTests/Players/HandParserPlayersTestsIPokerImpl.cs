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
    internal class HandParserPlayersTestsIPokerImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsIPokerImpl()
            : base("IPoker")
        {
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("Armstrongc", 2104.75m, 2),
                               new Player("TTR116060183", 2058.75m, 4),
                               new Player("yutant", 1600.94m, 7),                               
                               new Player("BurnN0tice", 2648m, 9),
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("doubting", 217.75m, 1)
                                   {
                                       HoleCards = HoleCards.FromCards("Ts9s")
                                   },
                               new Player("JohnJordan", 188m, 3)
                                   {
                                       HoleCards = HoleCards.FromCards("AcKh")
                                   },                               
                               new Player("wingirl3", 266.94m, 5),                               
                               new Player("IPullGuard", 110.80m, 6),                               
                               new Player("MrJeremiahPokers", 200m, 8),
                               new Player("HEISENBERGG", 230m, 10),
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get
            {
                return new PlayerList()
                           {
                               new Player("carpediem424", 256m, 1)
                                   {
                                       HoleCards = HoleCards.FromCards("QdQh")
                                   },      
                               new Player("BehindDeMusgow", 300m, 3)
                                   {
                                       IsSittingOut = true
                                   },
                               new Player("H4nkMoody", 300m, 5)
                                   {
                                       IsSittingOut = true
                                   },
                               new Player("numbersnletters", 1000m, 6)
                                   {
                                       IsSittingOut = true
                                   },  
                               new Player("PAYNLES", 2921m, 8),
                               new Player("leokadia19", 1020m, 10)
                                   {
                                       HoleCards = HoleCards.FromCards("9hAs")
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
                               new Player("is0lator", 200m, 1),      
                               new Player("brunzen", 172m, 3),
                               new Player("ShortStackExpert", 161.40m, 5)
                                   {
                                       HoleCards = HoleCards.FromCards("7s2s2h4c")
                                   },                                                             
                               new Player("ailiceC", 334.60m, 6),  
                               new Player("CSHOPE", 95.90m, 10)
                                    {
                                       HoleCards = HoleCards.FromCards("KsAsKc7h")
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
