using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils.Strings
{
    public static class StringExtensions
    {
        public static int LastIndexLoopsBackward(this string str, char c, int lastIndex)
        {
            for (int i = lastIndex; i >= 0; i--)
            {
                if (str[i] == c)
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool FastEndsWith(this string str, string end)
        {
            return str.EndsWith(end, StringComparison.Ordinal);
        }
    }
}
