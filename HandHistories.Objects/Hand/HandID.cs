using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.Hand
{
    public class HandID
    {
        public static long[] From(long value)
        {
            return new long[] { value };
        }

        public static long[] From(long[] value)
        {
            return value;
        }

        public static long[] Parse(string value)
        {
            return new long[] { long.Parse(value) };
        }
        
        public static long[] Parse(string value, char seperator)
        {
            var items = value.Split(seperator);
            return items.Select(long.Parse).ToArray();
        }

        public static long[] Parse(string[] values)
        {
            return values.Select(long.Parse).ToArray();
        }

        public static string GetString(long[] handID, string seperator = ".")
        {
            return string.Join(seperator, handID);
        }

        public static bool Equals(long[] a, long[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
