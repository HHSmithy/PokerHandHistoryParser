using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.GameDescription;
using Newtonsoft.Json.Linq;

namespace HandHistories.Parser.Parsers.JSONParser.IGT
{
    class IGT2019
    {
        internal static Limit ParseLimit(JToken hand)
        {
            var sb = hand["small"].Value<decimal>();
            var bb = hand["large"].Value<decimal>();

            return Limit.FromSmallBlindBigBlind(sb, bb, ParseCurrency(hand));
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
