using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Parsers.Exceptions;

namespace HandHistories.Parser.Parsers.FastParser.PokerStars
{
    partial class PokerStarsFastParserImpl
    {
        private GameType ParseTournamentGameType(string line)
        {
            var TournamentIdStartindex = GetTournamentIdStartIndex(line);
            // Expect first line to look like 
            // PokerStars Hand #121732576120: Tournament #974090903, $13.79+$1.21 USD Hold'em No Limit - Level III (25/50) - 2014/09/18 16:59:24 ET
            // Or
            // PokerStars Hand #132690000000: Tournament #1174000000, 70FPP Hold'em No Limit - Level I (10/20) - 2015/03/01 17:32:02 ET
            var commaIndex = line.IndexOf(',', TournamentIdStartindex);

            var endIndex = line.IndexOfFast("- ", commaIndex) - 1;

            //We need to work in re
            var secondSpaceIndex = line.IndexOf(' ', commaIndex + 3);

            // starts after the currency after the Buyin
            var startIndex = line.IndexOf(' ', secondSpaceIndex + 2) + 1;

            var length = endIndex - startIndex;

            try
            {
                return GetGameTypeFromLength(startIndex, line, length);
            }
            catch (Exception)
            {
                string gameString = line.Substring(commaIndex + 2, endIndex - commaIndex - 2);

                //if the length lookup fails we bruteforce the gametype
                if (gameString.EndsWithFast("Hold'em No Limit"))
                {
                    return GameType.NoLimitHoldem;
                }
                if (gameString.EndsWithFast("Omaha Pot Limit"))
                {
                    return GameType.PotLimitOmaha;
                }

                throw;
            }
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            // Expect the first line to look like:
            // PokerStars Hand #121723607468: Tournament #973955807, $2.30+$2.30+$0.40 USD Hold'em No Limit - Level XIII (600/1200)
            // PokerStars Hand #121732709812: Tournament #974092011, $55.56+$4.44 USD Hold'em No Limit - Level VI (100/200) - 2014/09/18 17:02:21 ET
            // this is obviously not needed for CashGame
            var TournamentIdStartindex = GetTournamentIdStartIndex(handLines[0]);

            int startIndex = handLines[0].IndexOf(',', TournamentIdStartindex) + 2;
            int endIndex = handLines[0].IndexOf(' ', startIndex);

            string buyinSubstring = handLines[0].Substring(startIndex, endIndex - startIndex);

            Currency currency;
            if (buyinSubstring.EndsWithFast("FPP"))
            {
                currency = Currency.RAKE_POINTS;
            }
            else if (buyinSubstring.Contains("Freeroll"))
            {
                currency = Currency.SATELLITE;
            }
            else
            {
                currency = ParseCurrency(handLines[0], buyinSubstring[0]);
            }

            decimal prizePoolValue;
            decimal rake;
            decimal knockoutValue = 0m;

            var buyinSplit = buyinSubstring.Split('+');
            if (buyinSplit.Length == 3)
            {
                prizePoolValue = buyinSplit[0].ParseAmount();
                knockoutValue = buyinSplit[1].ParseAmount();
                rake = buyinSplit[2].ParseAmount();
            }
            else if (buyinSplit.Length == 2)
            {
                prizePoolValue = buyinSplit[0].ParseAmount();
                rake = buyinSplit[1].ParseAmount();
            }
            else if (buyinSplit.Length == 1)
            {
                if (!buyinSplit[0].EndsWithFast("FPP"))
                {
                    throw new BuyinException(handLines[0], "Expected FPP Buyin Format");
                }

                prizePoolValue = 0;
                rake = 0;
            }
            else
            {
                throw new BuyinException(handLines[0], "Unrecognized Buyin Format");
            }

            return Buyin.FromBuyinRake(prizePoolValue, rake, currency, knockoutValue != 0m, knockoutValue);
        }
    }
}
