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
    }
}