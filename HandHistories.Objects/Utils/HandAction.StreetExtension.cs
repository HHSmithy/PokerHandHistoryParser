using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.Actions
{
    public static class StreetExtension
    {
        public static IEnumerable<HandAction> Street(this IEnumerable<HandAction> actions, Street street)
        {
            return actions.Where(p => p.Street == street);
        }
    }
}
