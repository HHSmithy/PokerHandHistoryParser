using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            var endIndex = line.IndexOf("- ", commaIndex) - 1;

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
                if (gameString.EndsWith("Hold'em No Limit", StringComparison.Ordinal))
                {
                    return GameType.NoLimitHoldem;
                }
                if (gameString.EndsWith("Omaha Pot Limit", StringComparison.Ordinal))
                {
                    return GameType.PotLimitOmaha;
                }

                throw;
            }
        }
    }
}
