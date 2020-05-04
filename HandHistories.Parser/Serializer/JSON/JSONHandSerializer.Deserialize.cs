using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Serializer.JSON.JSONObjects;
using Newtonsoft.Json;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using HandHistories.Objects.Actions;

namespace HandHistories.Parser.Serializer.JSON
{
    partial class JSONHandSerializer
    {
        public HandHistory Deserialize(string json)
        {
            var jhand = JsonConvert.DeserializeObject<JSON_hand>(json);
            HandHistory hand = new HandHistory();
            ReadGameInfo(hand, jhand.gameinfo);
            ReadPlayers(hand, jhand.players);
            ReadActions(hand, jhand.actions);
            ReadWinner(hand, jhand.winners);
            hand.CommunityCards = BoardCards.FromCards(jhand.board);
            return hand;
        }

        private void ReadWinner(HandHistory hand, List<JSON_winner> winners)
        {
            hand.Winners = new List<WinningsAction>();
            foreach (var winner in winners)
            {
                var actionType = ParseEnum<WinningsActionType>(winner.actionType);
                hand.Winners.Add(new WinningsAction(winner.player, actionType, winner.amount, winner.potNumber));
            }
        }

        private void ReadActions(HandHistory hand, List<JSON_handaction> actions)
        {
            hand.HandActions = new List<HandAction>();
            foreach (var action in actions)
            {
                var actionType = ParseEnum<HandActionType>(action.actionType);
                var street = ParseEnum<Street>(action.street);
                hand.HandActions.Add(new HandAction(action.player, actionType, action.amount, street, action.allin));
            }
        }

        private void ReadPlayers(HandHistory hand, List<JSON_player> players)
        {
            hand.Players = new PlayerList();
            foreach (var player in players)
            {
                hand.Players.Add(new Player(player.player, player.startingStack, player.seat)
                {
                    IsSittingOut = player.sitout,
                    HoleCards = HoleCards.FromCards(player.cards),
                });
            }
        }

        private void ReadGameInfo(HandHistory hand, JSON_gameinfo info)
        {
            var game = ParseEnum<GameEnum>(info.game);
            var limit = ParseEnum<GameLimitEnum>(info.limit);
            hand.GameDescription.GameType = new GameType(limit, game, info.cap);

            var currency = ParseEnum<Currency>(info.currency);
            hand.GameDescription.Limit = Limit.FromSmallBlindBigBlind(info.smallBlind, info.bigBlind, currency, info.ante != 0, info.ante);
            hand.GameDescription.PokerFormat = ParseEnum<PokerFormat>(info.format);
            hand.GameDescription.SeatType = SeatType.FromMaxPlayers(info.maxSeats);
            hand.GameDescription.Site = ParseEnum<SiteName>(info.site);

            hand.TableName = info.tablename;
            hand.DateOfHandUtc = GetFromUnixTime(info.date);
            hand.DealerButtonPosition = info.dealer;
            hand.HandId = HandID.Parse(info.gameId, '.');
            hand.TotalPot = info.totalPot;
            hand.Rake = info.rake;
        }

        static DateTime GetFromUnixTime(long time)
        {
            // Unix timestamp is seconds past epoch
            var sinceEpoch = TimeSpan.FromSeconds(time);
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime + sinceEpoch;
        }

        static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
