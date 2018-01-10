using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Serializer.JSON
{
    partial class JSONHandSerializer
    {
        public HandHistory Deserialize(string json)
        {
            var JSON = JObject.Parse(json);
            HandHistory hand = new HandHistory();
            ReadGameInfo(hand, JSON);

            return hand;
        }

        private void ReadGameInfo(HandHistory hand, JObject JSON)
        {
            var gameinfo = new GameDescriptor();
            gameinfo.Site = (SiteName)Enum.Parse(typeof(SiteName), (string)JSON["site"]);
            hand.TableName = (string)JSON["tablename"];
            hand.HandId = (long)JSON["gameId"];
            //JSON.Add("currency", GetJSONCurrency(game.Limit.Currency));
            //JSON.Add("limit", game.GameType.Limit.ToString());
            //JSON.Add("game", game.GameType.Game.ToString());
            //JSON.Add("date", GetUnixTime(hand.DateOfHandUtc));
            //JSON.Add("dealer", hand.DealerButtonPosition);
            //JSON.Add("maxSeats", game.SeatType.MaxPlayers);
            //JSON.Add("bigBlind", game.Limit.BigBlind);
            //JSON.Add("smallBlind", game.Limit.SmallBlind);
            //JSON.Add("ante", game.Limit.Ante);
        }
    }
}
