using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.CommonObjects
{
    public class HandHistorySummary
    {
        public DateTime DateOfHandUtc { get; set; }

        public SiteNames Site { get; set; }

        public GameTypes GameType { get; set; }

        public Limit Limit { get; set; }

        public int NumPlayers { get; set; }

        public SeatTypes SeatType { get; set; }

        public string TableName { get; set; }
    }
}
