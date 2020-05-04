using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Utils.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Parsers.JSONParser.IGT
{
    class IGT2018
    {
        public static Limit ParseLimit(JToken hand)
        {
            var limit = hand["stake"].ToString();

            var items = limit.Split('/');
            var SB = items[0].ParseAmount();
            var BB = items[1].ParseAmount();
            return Limit.FromSmallBlindBigBlind(SB, BB, ParseCurrency(hand));
        }

        static Currency ParseCurrency(JToken hand)
        {
            string currency = hand["tableCurrency"].ToString();
            switch (currency)
            {
                case "SEK": return Currency.SEK;
                case "USD": return Currency.USD;
                case "EUR": return Currency.EURO;

                default:
                    throw new ArgumentException("Unhandled Currrency: " + currency);
            }
        }
    }
}
