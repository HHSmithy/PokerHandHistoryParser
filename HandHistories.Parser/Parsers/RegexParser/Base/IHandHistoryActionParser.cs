using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;

namespace HandHistories.Parser.Parsers.Base
{
    public interface IHandHistoryActionParser
    {
        List<HandAction> ParseActions(string handText, PlayerList players);

        List<HandAction> ParseActions(string handText, PlayerList players, Street currentStreet);
    }
}