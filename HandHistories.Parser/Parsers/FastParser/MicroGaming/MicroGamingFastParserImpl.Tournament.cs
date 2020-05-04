using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Parsers.FastParser.MicroGaming
{
    partial class MicroGamingFastParserImpl
    {
        protected override Buyin ParseBuyin(string[] handLines)
        {
            return Buyin.FromBuyinRake(0, 0, Currency.CHIPS);
        }
    }
}
