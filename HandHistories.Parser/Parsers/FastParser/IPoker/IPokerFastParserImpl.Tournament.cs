using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Parsers.FastParser.IPoker
{
    partial class IPokerFastParserImpl
    {
        protected override Buyin ParseBuyin(string[] handLines)
        {
            for (int i = 4; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line.StartsWith("<totalbuyin"))
                {
                    return ParseBuyin(line);
                }
            }

            throw new ArgumentException("No Buyin Error");
        }

        public new static Buyin ParseBuyin(string line)
        {
            const int buyInStart = 12;
            int buyInEnd = line.Length - 13;

            string amountStr = line.Substring(buyInStart, buyInEnd - buyInStart);

            return null;
        }

        protected override long ParseTournamentId(string[] handLines)
        {
            for (int i = 4; i < handLines.Length; i++)
            {
                string line = handLines[i];

                if (line.StartsWith("<tablename"))
                {
                    return ParseTournamentId(line);
                }
            }

            throw new ArgumentException("Tablename not found");
        }

        public new static long ParseTournamentId(string line)
        {
            const int buyInStart = 12;
            int buyInEnd = line.Length - 13;

            string idStr = line.Substring(buyInStart, buyInEnd - buyInStart);

            return long.Parse(idStr);
        }
    }
}
