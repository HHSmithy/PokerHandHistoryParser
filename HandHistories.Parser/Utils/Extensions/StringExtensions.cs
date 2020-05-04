using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace HandHistories.Parser.Utils.Extensions
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

        public static char Last(this string str)
        {
            return str[str.Length - 1];
        }

        public static bool EndsWith(this string str, char end)
        {
            return str[str.Length - 1] == end;
        }

        public static bool EndsWithFast(this string str, string end)
        {
            return str.EndsWith(end, StringComparison.Ordinal);
        }

        public static int IndexOfFast(this string str, string value)
        {
            return str.IndexOf(value, StringComparison.Ordinal);
        }

        public static int IndexOfFast(this string str, string value, int startindex)
        {
            return str.IndexOf(value, startindex, StringComparison.Ordinal);
        }

        public static int LastIndexOfFast(this string str, string value)
        {
            return str.LastIndexOf(value, StringComparison.Ordinal);
        }

        public static int LastIndexOfFast(this string str, string value, int startindex)
        {
            return str.LastIndexOf(value, startindex, StringComparison.Ordinal);
        }

        public static bool StartsWith(this string str, char start)
        {
            return str[0] == start;
        }

        public static bool StartsWithFast(this string str, string value)
        {
            return str.StartsWith(value, StringComparison.Ordinal);
        }

        public static string SubstringBetween(this string str, int startIndex, int endIndex)
        {
            return str.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Removes any currency symbols before parsing
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ParseAmount(this string str)
        {
            str = str.Trim('£', '€', '$', '¥');
            return Decimal.Parse(str, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Removes any currency symbols before parsing
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ParseAmount(this string str, NumberFormatInfo numberFormat)
        {
            str = str.Trim('£', '€', '$', '¥');
            return Decimal.Parse(str, numberFormat);
        }

        static readonly char[] ParseWSTrimChars = new char[]
        {
            '£', '€', '$', ' ', '¥', (char)160
        };

        /// <summary>
        /// Removes any currency symbols and whitespaces before parsing
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ParseAmountWS(this string str)
        {
            str = str.Trim(ParseWSTrimChars);
            return Decimal.Parse(str, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Removes any currency symbols and whitespaces before parsing
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ParseAmountWS(this string str, NumberFormatInfo numberFormat)
        {
            str = str.Trim(ParseWSTrimChars);
            return Decimal.Parse(str, numberFormat);
        }
    }
}
