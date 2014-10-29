using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;

namespace HandHistories.Parser.UnitTests.Parsers.Base
{
    internal interface ISampleHandHistoryRepository
    {
        string GetCancelledHandHandHistoryText(PokerFormat pokerFormat, SiteName siteName);

        string GetValidHandHandHistoryText(PokerFormat pokerFormat, SiteName siteName, bool isValid);

        string GetSeatExampleHandHistoryText(PokerFormat pokerFormat, SiteName siteName, SeatType seatType);

        string GetLimitExampleHandHistoryText(PokerFormat pokerFormat, SiteName siteName, string fileName);

        string GetTableExampleHandHistoryText(PokerFormat pokerFormat, SiteName siteName, int tableTestNumber);

        string GetGeneralHandHistoryText(PokerFormat pokerFormat, SiteName siteName, string name);

        string GetFormatHandHistoryText(PokerFormat pokerFormat, SiteName siteName, string name);

        string GetGameTypeHandHistoryText(PokerFormat pokerFormat, SiteName siteName, GameType gameType);

        string GetCommunityCardsHandHistoryText(PokerFormat pokerFormat, SiteName siteName, Street street);

        string GetMultipleHandExampleText(PokerFormat pokerFormat, SiteName siteName, int handCount);

        string GetHandExample(PokerFormat pokerFormat, SiteName siteName, string subFolder, string fileName);
    }
}
