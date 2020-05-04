using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Serializer.JSON.JSONObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Serializer.JSON
{
    public partial class JSONHandSerializer
    {
        public string Serialize(HandHistory hand)
        {
            return JsonConvert.SerializeObject(GetHand(hand));
        }

        public JObject GetJObject(HandHistory hand)
        {
            return JObject.FromObject(GetHand(hand));
        }

        public JSON_hand GetHand(HandHistory hand)
        {
            JSON_hand jhand = new JSON_hand();
            jhand.gameinfo = GetGameInfo(hand);
            jhand.players = GetPlayers(hand);
            jhand.actions = GetHandActions(hand);
            jhand.winners = GetWinners(hand);
            jhand.board = hand.CommunityCards.ToString();
            jhand.raw = hand.FullHandHistoryText;

            return jhand;
        }

        private static List<JSON_winner> GetWinners(HandHistory hand)
        {
            var winners = new List<JSON_winner>();
            foreach (var winner in hand.Winners)
            {
                winners.Add(new JSON_winner()
                {
                    actionType = winner.ActionType.ToString(),
                    amount = winner.Amount,
                    player = winner.PlayerName,
                    potNumber = winner.PotNumber,
                });
            }

            return winners;
        }

        private static List<JSON_handaction> GetHandActions(HandHistory hand)
        {
            var actions = new List<JSON_handaction>();
            foreach (var action in hand.HandActions)
            {
                actions.Add(new JSON_handaction()
                {
                    actionType = action.HandActionType.ToString(),
                    allin = action.IsAllIn,
                    amount = action.Amount,
                    player = action.PlayerName,
                    street = action.Street.ToString(),
                });
            }

            return actions;
        }

        private static List<JSON_player> GetPlayers(HandHistory hand)
        {
            var players = new List<JSON_player>();
            foreach (var player in hand.Players)
            {
                players.Add(new JSON_player()
                {
                    player = player.PlayerName,
                    cards = player.hasHoleCards ? player.HoleCards.ToString() : "",
                    seat = player.SeatNumber,
                    sitout = player.IsSittingOut,
                    startingStack = player.StartingStack,
                });
            }

            return players;
        }

        private static JSON_gameinfo GetGameInfo(HandHistory hand)
        {
            return new JSON_gameinfo()
            {
                format = hand.GameDescription.PokerFormat.ToString(),
                site = hand.GameDescription.Site.ToString(),
                tablename = hand.TableName,
                gameId = hand.HandIdString,
                cap = hand.GameDescription.GameType.Cap,
                limit = hand.GameDescription.GameType.Limit.ToString(),
                game = hand.GameDescription.GameType.Game.ToString(),
                currency = hand.GameDescription.Limit.Currency.ToString(),
                bigBlind = hand.GameDescription.Limit.BigBlind,
                smallBlind = hand.GameDescription.Limit.SmallBlind,
                ante = hand.GameDescription.Limit.Ante,
                dealer = hand.DealerButtonPosition,
                maxSeats = hand.GameDescription.SeatType.MaxPlayers,
                date = GetUnixTime(hand.DateOfHandUtc),
                totalPot = hand.TotalPot.Value,
                rake = hand.Rake.Value,
                hero = hand.Hero != null ? hand.Hero.PlayerName : "",
            };
        }

        static long GetUnixTime(DateTime time)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (long)(time.ToUniversalTime() - dtDateTime).TotalSeconds;
        }
    }
}
