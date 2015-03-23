using System.Text;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using System;

namespace HandHistories.Objects.GameDescription
{
    public static class PokerFormatUtils
    {
        public static string GetDisplayName(PokerFormat pokerFormat)
        {
            switch (pokerFormat)
            {
                case PokerFormat.CashGame:
                    return "Cash Game";
                default:
                    return pokerFormat.ToString();
            }
        }

        public static PokerFormat ParseFormatName(string pokerformat)
        {
            switch (pokerformat.ToLower())
            {
                case "cash game":
                case "cashgame":
                case "cg":
                case "cash":
                    return PokerFormat.CashGame;

                case "sng":
                case "sitandgo":
                case "sit and go":
                case "sitngo":
                case "sit&go":
                    return PokerFormat.SitAndGo;

                case "mtt":
                case "multitabletournament":
                case "multi table tournament":
                    return PokerFormat.MultiTableTournament;

            }

            return PokerFormat.Unknown;
        }
    }
}