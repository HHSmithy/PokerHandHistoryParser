using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils.Time
{
    class UnixTimestamp
    {
        static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixtimestamp(long timestamp)
        {
            return EPOCH + TimeSpan.FromSeconds(timestamp);
        }

        public static DateTime FromUnixtimestampInNanos(long timestamp)
        {
            return EPOCH + TimeSpan.FromSeconds(timestamp / 1000000000);
        }

        public static long GetUnixTimestamp(DateTime dt)
        {
            return (long)(dt - EPOCH).TotalSeconds;
        }
    }
}
