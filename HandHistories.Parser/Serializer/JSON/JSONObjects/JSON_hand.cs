using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Serializer.JSON.JSONObjects
{
    public class JSON_hand
    {
        public JSON_gameinfo gameinfo;
        public List<JSON_player> players;
        public List<JSON_handaction> actions;
        public List<JSON_winner> winners;
        public string board;
        public string raw;
    }
}
