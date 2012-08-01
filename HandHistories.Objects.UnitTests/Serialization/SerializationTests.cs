using System.Text;
using System.Linq.Expressions;
using System.Linq;
using System;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Objects.UnitTests.Utils.Serialization;
using NUnit.Framework;

namespace HandHistories.Objects.UnitTests.Serialization
{
    [TestFixture]
    class SerializationTests
    {
        private readonly ISerializationHandler _serializationHandler = new SerializationHandlerDataContractImpl();

        private HandHistory _handHistory;

        [SetUp]
        public void SetUp()
        {
            _handHistory = new HandHistory()
                               {                                  
                                   ComumnityCards =
                                       BoardCards.ForFlop(Card.GetCardFromIntValue(5), Card.GetCardFromIntValue(14),
                                                          Card.GetCardFromIntValue(40)),
                                   DateOfHandUtc = new DateTime(2012, 3, 20, 12, 30, 44),
                                   DealerButtonPosition = 5,
                                   FullHandHistoryText = "some hand text",
                                   GameDescription =
                                       new GameDescriptor(PokerFormat.CashGame,
                                                          SiteName.PartyPoker, 
                                                          GameType.NoLimitHoldem,
                                                          Limit.FromSmallBlindBigBlind(10, 20, Currency.USD),
                                                          TableType.FromTableTypeDescriptions(TableTypeDescription.Regular),
                                                          SeatType.FromMaxPlayers(6)),
                                   HandActions = new List<HandAction>()
                                                     {
                                                         new HandAction("Player1", HandActionType.POSTS, 0.25m, Street.Preflop)
                                                     },
                                   HandId = 141234124,
                                   NumPlayersSeated = 2,
                                   Players = new PlayerList()
                                                 {
                                                     new Player("Player1", 1000, 1),
                                                     new Player("Player2", 300, 5),
                                                 },
                                   TableName = "Test Table",                                   
                               };
        }

        [Test]
        public void CanSerailizeHandHistory()
        {           
            string serialized =
                _serializationHandler.Serialize(_handHistory);

            Assert.IsNotNullOrEmpty(serialized);
        }

        [Test]
        public void CanDeSerailizeHandHistory()
        {
            string serialized = _serializationHandler.Serialize(_handHistory);

            HandHistory deserailizedHandHistory = _serializationHandler.Deserialize<HandHistory>(serialized);

            Assert.AreEqual(_handHistory, deserailizedHandHistory);
        }
    }
}
