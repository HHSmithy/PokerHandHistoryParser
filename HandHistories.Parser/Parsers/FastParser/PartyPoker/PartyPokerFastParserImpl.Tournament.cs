using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Parsers.Exceptions;

namespace HandHistories.Parser.Parsers.FastParser.PartyPoker
{
    partial class PartyPokerFastParserImpl
    {
        protected override long ParseTournamentId(string[] handLines)
        {
            throw new NotImplementedException();
        }

        static GameType ParseGametypeTournament(string[] handLines)
        {
            string line = handLines[1];

            char limit = line[0];
            char game = line[3];

            switch (limit)
            {
                case 'N':
                    switch (game)
                    {
                        case 'O':
                            if (line[9] == 'H')
                            {
                                return GameType.NoLimitOmahaHiLo;
                            }
                            else
                            {
                                return GameType.NoLimitOmaha;
                            }
                        case 'T':
                            return GameType.NoLimitHoldem;
                    }
                    break;
                case 'F':
                    switch (game)
                    {
                        case 'O':
                            if (line[9] == 'H')
                            {
                                return GameType.FixedLimitOmahaHiLo;
                            }
                            else
                            {
                                return GameType.FixedLimitOmaha;
                            }
                        case 'T':
                            return GameType.FixedLimitHoldem;
                    }
                    break;
                case 'P':
                    switch (game)
                    {
                        case 'O':
                            if (line[9] == 'H')
                            {
                                return GameType.PotLimitOmahaHiLo;
                            }
                            else
                            {
                                return GameType.PotLimitOmaha;
                            }
                        case 'T':
                            return GameType.PotLimitHoldem;
                    }
                    break;
            }
            throw new UnrecognizedGameTypeException(line, "Unknown Tournament GameType: " + line);
        }

        static Limit ParseTournamentLimit(string[] handLines)
        {
            string line = handLines[1];
            int limitStartIndex = line.IndexOf('(') + 1;
            int limitEndIndex = line.IndexOf(')', limitStartIndex);

            string limitLine = line.Substring(limitStartIndex, limitEndIndex - limitStartIndex);
            char[] splitChars = new char[]{ '/', ' ', '-' };
            var limitItems = limitLine.Split(splitChars);

            decimal SB = limitItems[0].ParseAmount();
            decimal BB = limitItems[1].ParseAmount();

            if (limitItems.Length == 3)
            {
                decimal ante = limitItems[2].ParseAmount();
                return Limit.FromSmallBlindBigBlind(SB, BB, Currency.CHIPS, true, ante);
            }
            else
            {
                return Limit.FromSmallBlindBigBlind(SB, BB, Currency.CHIPS);
            }
        }

        protected override Buyin ParseBuyin(string[] handLines)
        {
            string line = handLines[1];
            int buyinStartIndex = line.IndexOf('$');
            int buyinEndIndex = line.IndexOf(' ', buyinStartIndex);

            string buyin = line.Substring(buyinStartIndex, buyinEndIndex - buyinStartIndex);
            return Buyin.FromBuyinRake(buyin.ParseAmount(), 0, Currency.USD);
        }
    }
}
