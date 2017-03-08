using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.FastParsing;
using HandHistories.Parser.Utils.Uncalled;
using HandHistories.Parser.Parsers.JSONParser.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using HandHistories.Parser.Parsers.FastParser.BossMedia;
using HandHistories.Parser.Utils.Pot;
using HandHistories.Parser.Utils.RaiseAdjuster;

namespace HandHistories.Parser.Parsers.JSONParser.IGT
{
    public class IGTJSONParserImpl : HandHistoryParserJSONImpl
    {
        public override SiteName SiteName
        {
            get { return SiteName.IGT; }
        }

        protected override void FinalizeHandHistory(HandHistory hand)
        {
            //hand.HandActions = RaiseAdjuster.AdjustRaiseSizes(hand.HandActions);
        }

        protected override void ParseExtraHandInformation(JObject JSON, HandHistorySummary summary)
        {
            var handJSON = JSON["history"][0];
            var showdownJSON = handJSON["showDown"];

            var totalpot = showdownJSON["pot"].Value<decimal>();
            var rake = showdownJSON["rake"].Value<decimal>();

            summary.TotalPot = totalpot + rake;
            summary.Rake = rake;
        }

        protected override List<HandAction> ParseHandActions(JObject JSON)
        {
            var handJSON = JSON["history"][0];
            var actionsJSON = handJSON["action"];

            Street currentStreet = Street.Preflop;
            List<HandAction> actions = new List<HandAction>();
            foreach (var action in actionsJSON)
            {
                string type = action["type"].ToString();
                switch (type)
                {
                    case "HAND_DEAL":
                        continue;

                    case "HAND_BOARD":
                        currentStreet = (Street)((int)currentStreet + 1);
                        continue;

                    case "HAND_BLINDS":
                        actions.Add(ParseBlindAction(action, Street.Preflop));
                        break;

                    case "ACTION_CHECK":
                        actions.Add(ParseAction(HandActionType.CHECK, action, currentStreet));
                        break;

                    case "ACTION_CALL":
                        actions.Add(ParseCallAction(action, actions, currentStreet));
                        break;

                    case "ACTION_BET":
                        actions.Add(ParseAmountAction(HandActionType.BET, action, currentStreet));
                        break;

                    case "ACTION_RAISE":
                        actions.Add(ParseAmountAction(HandActionType.RAISE, action, currentStreet));
                        break;

                    case "ACTION_FOLD":
                        actions.Add(ParseAction(HandActionType.FOLD, action, currentStreet));
                        break;

                    case "ACTION_ALLIN":
                        actions.Add(ParseAllInAction(action, actions, currentStreet));
                        break;

                    case "ACTION_ADJUSTEMENT":
                        actions.Add(ParseAction(HandActionType.UNCALLED_BET, action, currentStreet));
                        break;

                    default:
                        throw new ArgumentException("Unhandled Action Type: " + type);
                }
            }

            //Parse Winners
            var showdownJSON = handJSON["showDown"];
            foreach (var resultJSON in showdownJSON["result"])
            {
                var winAmount = resultJSON["win"].Value<decimal>();
                if (winAmount > 0)
                {
                    string name = resultJSON["player"].ToString();
                    actions.Add(new WinningsAction(name, HandActionType.WINS, winAmount, 0));
                }
            }

            return actions;
        }

        private HandAction ParseAllInAction(JToken action, List<HandAction> actions, Street currentStreet)
        {
            string name = action["player"].ToString();
            decimal amount = action["value"].Value<decimal>();

            HandActionType allInType = BossMediaAllInAdjuster.GetAllInActionType(name, amount, currentStreet, actions);
            if (allInType == HandActionType.CALL)
            {
                amount = BossMediaAllInAdjuster.GetAdjustedCallAllInAmount(name, amount, currentStreet, actions);
            }

            return new HandAction(name, allInType, amount, currentStreet, true);
        }

        static HandAction ParseCallAction(JToken action, List<HandAction> actions, Street currentStreet)
        {
            string name = action["player"].ToString();
            decimal amount = action["value"].Value<decimal>();
            decimal reduction = actions.Where(p => p.Street == currentStreet && p.PlayerName == name).Sum(p => p.Amount);

            return new HandAction(name, HandActionType.CALL, amount - Math.Abs(reduction), currentStreet);
        }


        static HandAction ParseAmountAction(HandActionType handActionType, JToken action, Street street)
        {
            string name = action["player"].ToString();
            decimal amount = action["value"].Value<decimal>();
            return new HandAction(name, handActionType, amount, street);
        }

        static HandAction ParseAction(HandActionType handActionType, JToken action, Street street)
        {
            string name = action["player"].ToString();
            return new HandAction(name, handActionType, 0, street);
        }

        static HandAction ParseBlindAction(JToken action, Street street)
        {
            string blindType = action["kind"].ToString();
            HandActionType actionType = ParseBlindActionType(blindType);

            return ParseAmountAction(actionType, action, street);
        }

        static HandActionType ParseBlindActionType(string blindType)
        {
            switch (blindType)
            {
                case "HAND_SB":
                    return HandActionType.SMALL_BLIND;

                case "HAND_BB":
                    return HandActionType.BIG_BLIND;

                default:
                    throw new ArgumentException("Unhandled Blind Type");
            }
        }

        protected override PlayerList ParsePlayers(JObject JSON)
        {
            var handJSON = JSON["history"][0];
            var playersJSON = handJSON["player"];

            var playerlist = new PlayerList();
            foreach (var playerJSON in playersJSON)
            {
                var name = playerJSON["name"].ToString();
                if (name == "UNKNOWN")
                {
                    continue;
                }

                var stack = playerJSON["amount"].Value<decimal>();
                var seat = playerJSON["seat"].Value<int>();
                var state = playerJSON["state"].ToString();

                playerlist.Add(new Player(name, stack, seat) { IsSittingOut = state != "STATE_PLAYING" });
            }

            var actionsJSON = handJSON["action"];
            foreach (var action in actionsJSON)
            {
                string type = action["type"].ToString();
                if (type.StartsWithFast("ACTION"))
                {
                    break;
                }
                else if (type == "HAND_DEAL")
                {
                    if (isDealtCards(action))
                    {
                        var playerName = action["player"].ToString();
                        playerlist[playerName].HoleCards = ParseHoleCards(action["card"]);
                        break;
                    }
                }
            }

            var resultJSON = handJSON["showDown"]["result"];
            foreach (var result in resultJSON)
            {
                var cardsJSON = result["card"];
                if (cardsJSON != null && cardsJSON.Count() > 0)
                {
                    var name = result["player"].ToString();
                    playerlist[name].HoleCards = ParseHoleCards(cardsJSON);
                }
            }
            return playerlist;
        }

        protected override BoardCards ParseCommunityCards(JObject JSON)
        {
            var handJSON = JSON["history"][0];
            var actionsJSON = handJSON["action"];

            var lastBoardJSON = actionsJSON.LastOrDefault(p => p["type"].ToString() == "HAND_BOARD");
            if (lastBoardJSON == null)
            {
                return BoardCards.ForPreflop();
            }

            var cards = lastBoardJSON["card"].Select(ParseCard);

            return BoardCards.FromCards(cards.ToArray());
        }

        static Card[] IGTCardLookup = new Card[]
        {
            #region Diamonds 0-12
            new Card('A', 'd'),
		    new Card('2', 'd'),
            new Card('3', 'd'),
            new Card('4', 'd'),
            new Card('5', 'd'),
            new Card('6', 'd'),
            new Card('7', 'd'),
            new Card('8', 'd'),
            new Card('9', 'd'),
            new Card('T', 'd'),
            new Card('J', 'd'),
            new Card('Q', 'd'),
            new Card('K', 'd'),
	        #endregion
            #region Clubs 13-25
            new Card('A', 'c'), 
		    new Card('2', 'c'),
            new Card('3', 'c'),
            new Card('4', 'c'),
            new Card('5', 'c'),
            new Card('6', 'c'),
            new Card('7', 'c'),
            new Card('8', 'c'),
            new Card('9', 'c'),
            new Card('T', 'c'),
            new Card('J', 'c'),
            new Card('Q', 'c'),
            new Card('K', 'c'),
	        #endregion
            #region Hearts 26-38
            new Card('A', 'h'), 
		    new Card('2', 'h'),
            new Card('3', 'h'),
            new Card('4', 'h'),
            new Card('5', 'h'),
            new Card('6', 'h'),
            new Card('7', 'h'),
            new Card('8', 'h'),
            new Card('9', 'h'),
            new Card('T', 'h'),
            new Card('J', 'h'),
            new Card('Q', 'h'),
            new Card('K', 'h'),
	        #endregion
            #region Spades 39-51
            new Card('A', 's'),
		    new Card('2', 's'),
            new Card('3', 's'),
            new Card('4', 's'),
            new Card('5', 's'),
            new Card('6', 's'),
            new Card('7', 's'),
            new Card('8', 's'),
            new Card('9', 's'),
            new Card('T', 's'),
            new Card('J', 's'),
            new Card('Q', 's'),
            new Card('K', 's'),
	        #endregion
        };

        static Card ParseCard(JToken cardJSON)
        {
            //<CARD LINK="b"></CARD> is unkown card
            //<CARD LINK="13"></CARD>
            //LINK 1 - 13 is diamonds
            //LINK 14 - 26 is clubs
            //LINK 27 - 39 is hearts
            //LINK 40 - 52 is spades
            int cardID = cardJSON["link"].Value<int>();

            return IGTCardLookup[cardID];
        }

        static HoleCards ParseHoleCards(JToken JSON)
        {
            return HoleCards.FromCards(null, JSON.Select(ParseCard).ToArray());
        }

        protected override int ParseDealerPosition(JObject JSON)
        {
            var handJSON = JSON["history"][0];
            var playersJSON = handJSON["player"];

            foreach (var playerJSON in playersJSON)
            {
                var dealer = playerJSON["dealer"].ToString();
                if (dealer == "Y")
                {
                    var seat = playerJSON["seat"].Value<int>();
                    return seat;
                }
            }
            throw new ArgumentException("No Dealer");
        }

        protected override DateTime ParseDateUtc(JObject JSON)
        {
            var hand = JSON["history"][0];

            //ITG have now POSIX in milliseconds instead of seconds
            //POSIX Time:
            //http://en.wikipedia.org/wiki/Unix_time
            long ticks = hand["date"].Value<long>();

            DateTime POSIX_EPOCH = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            DateTime time = POSIX_EPOCH + TimeSpan.FromSeconds(ticks / 1000);

            return time;
        }

        protected override long ParseHandId(JObject JSON)
        {
            var hand = JSON["history"][0];
            var handIDString = hand["handId"].ToString();

            var items = handIDString.Split(new string[] { ".hand." }, StringSplitOptions.None);
            var tableID = long.Parse(items[0]) * 100000;
            var handID = long.Parse(items[1]);

            return tableID + handID;
        }

        protected override string ParseTableName(JObject JSON)
        {
            var hand = JSON["history"][0];
            var name = hand["table"];
            return name.ToString();
        }

        protected override SeatType ParseSeatType(JObject JSON)
        {
            return SeatType.FromMaxPlayers(ParsePlayers(JSON).Count);
        }

        protected override GameType ParseGameType(JObject JSON)
        {
            var hand = JSON["history"][0];
            var gametype = hand["gameType"];
            var limit = hand["limit"];

            return new GameType(GetGameLimit(limit), GetGameType(gametype));
        }

        static GameEnum GetGameType(JToken JSON)
        {
            string game = JSON.ToString();
            switch (game)
            {
                case "OMAHAHILO":
                    return GameEnum.OmahaHiLo;

                case "OMAHA":
                    return GameEnum.Omaha;

                case "TEXASHOLDEM":
                    return GameEnum.Holdem;

                default:
                    throw new ArgumentException("Unhandled Game: " + game);
            }
        }

        static GameLimitEnum GetGameLimit(JToken JSON)
        {
            string limit = JSON.ToString();
            switch (limit)
            {
                case "Fixed Limit":
                    return GameLimitEnum.FixedLimit;

                case "Pot Limit":
                    return GameLimitEnum.PotLimit;

                case "No Limit":
                    return GameLimitEnum.NoLimit;

                default:
                    throw new ArgumentException("Unhandled Limit: " + limit);
            }
        }

        protected override TableType ParseTableType(JObject JSON)
        {
            return new TableType(TableTypeDescription.Regular);
        }

        protected override Limit ParseLimit(JObject JSON)
        {
            var hand = JSON["history"][0];
            var limit = hand["stake"].ToString();

            var items = limit.Split('/');
            var SB = items[0].ParseAmount();
            var BB = items[1].ParseAmount();
            return Limit.FromSmallBlindBigBlind(SB, BB, Currency.USD);
        }

        protected override int ParseNumPlayers(JObject JSON)
        {
            return ParsePlayers(JSON).Count;
        }

        protected override PokerFormat ParsePokerFormat(JObject JSON)
        {
            var hand = JSON["history"][0];
            var format = hand["gameKind"].ToString();

            switch (format)
            {
                case "GAMEKIND_CASH":
                    return PokerFormat.CashGame;

                case "GAMEKIND_TOURNAMENT":
                    return PokerFormat.MultiTableTournament;

                default:
                    throw new NotImplementedException("PokerFormat: " + format);
            }
        }

        protected override string ParseHeroName(JObject JSON)
        {
            var hand = JSON["history"][0];
            var actionsJSON = hand["action"];

            foreach (var action in actionsJSON)
            {
                string type = action["type"].ToString();
                if (type == "HAND_BOARD")
                {
                    break;
                }
                else if (type == "HAND_DEAL")
                {
                    if (isDealtCards(action))
                    {
                        return action["player"].ToString();
                    }
                }
            }

            return null;
        }

        static bool isDealtCards(JToken JSON)
        {
            var cards = JSON["card"];
            if (cards[0]["link"].ToString() == "b")
            {
                return false;
            }
            return true;
        }
    }
}
