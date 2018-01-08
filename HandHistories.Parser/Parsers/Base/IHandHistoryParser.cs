using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;

namespace HandHistories.Parser.Parsers.Base
{
    public interface IHandHistoryParser : IHandHistorySummaryParser
    {
        HandHistory ParseFullHandHistory(string handText, bool rethrowExceptions = false);
        
        List<HandAction> ParseHandActions(string handText, out List<WinningsAction> winners);

        PlayerList ParsePlayers(string handText);
        
        BoardCards ParseCommunityCards(string handText);

        string ParseHeroName(string handText);
    }
}
