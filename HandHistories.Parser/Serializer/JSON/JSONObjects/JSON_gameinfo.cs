using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Serializer.JSON.JSONObjects
{
    public class JSON_gameinfo
    {
        public string format;
        public string site;
        public string tablename;
        public string gameId;
        public string currency;
        public bool cap;
        public string limit;
        public string game;
        public int dealer;
        public int maxSeats;
        public decimal smallBlind;
        public decimal bigBlind;
        public decimal ante;
        public long date;
        public decimal totalPot;
        public decimal rake;
        public string hero;
    }
}
