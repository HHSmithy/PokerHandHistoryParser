using System;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    [TestFixture]
    class HandParserPlayersTestsWinningPokerV2Impl : HandParserPlayersTests
    {
        public HandParserPlayersTestsWinningPokerV2Impl()
            : base("WinningPokerV2")
        {
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get
            {
                //Seat 1: pandabear44($12.63)
                //Seat 2: GreatShannon ($20.00)
                //Seat 3: Swedish Fish($8.63)
                //Seat 4: sever4523($0.00) is sitting out
                //Seat 5: !? ($3945.11)
                //Seat 6: EMUDGE($25.60)
                //!? sits out
                return new PlayerList()
                {
                    new Player("pandabear44", 12.63m, 1),
                    new Player("GreatShannon", 20.00m, 2)
                    {
                        IsSittingOut = true
                    },
                    new Player("Swedish Fish", 8.63m, 3),
                    new Player("sever4523", 0m, 4)
                    {
                        IsSittingOut = true
                    },
                    new Player("!?", 3945.11m, 5)
                    {
                        IsSittingOut = true
                    },
                    new Player("EMUDGE", 25.60m, 6),
                };
            }
        }

        protected override PlayerList ExpectedOmahaShowdownPlayers
        {
            get
            {
                //Seat 1: Player1($12.73)
                //Seat 3: Hero($8.88)
                //Seat 4: Player4($12.50)
                //Seat 6: Player6($14.01)
                //Dealt to Hero [Ad 3s Ts 4s]
                //Player4 shows [4h 4c 7d 6c] (a flush, King high [Kc Jc 6c 4c 3c])
                //Player6 shows[6s Ah 3d Kd] (a full house, Kings full of Threes[Kh Kd Kc 3d 3c])
                return new PlayerList()
                {
                    new Player("Player1", 12.73m, 1),
                    new Player("Hero", 8.88m, 3)
                    {
                        HoleCards = HoleCards.FromCards("Ad3sTs4s"),
                    },
                    new Player("Player4", 12.50m, 4)
                    {
                        HoleCards = HoleCards.FromCards("4h4c7d6c"),
                    },
                    new Player("Player6", 14.01m, 6)
                    {
                        HoleCards = HoleCards.FromCards("6sAh3dKd"),
                    },
                };
            }
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get
            {
                Assert.Ignore("No Test HH");
                throw new NotImplementedException();
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                Assert.Ignore("No Test HH");
                throw new NotImplementedException();
            }
        }
        
        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get
            {
                Assert.Ignore("No Test HH");
                throw new NotImplementedException();
            }
        }
    }
}
