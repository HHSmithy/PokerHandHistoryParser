using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils.Extensions
{
    static class LazyStringSplitExtension
    {
        public static IEnumerable<string> LazyStringSplit(this string str, string splitter)
        {
            int l = str.Length;
            int i = 0, j = str.IndexOf(splitter, 0, l);
            if (j == -1) // No such substring
            {
                yield return str; // Return original and break
                yield break;
            }

            while (j != -1)
            {
                if (j - i > 0) // Non empty? 
                {
                    yield return str.Substring(i, j - i); // Return non-empty match
                }
                i = j + 1;
                j = str.IndexOf(splitter, i, l - i);
            }

            if (i < l) // Has remainder?
            {
                yield return str.Substring(i, l - i); // Return remaining trail
            }
        }
    }
}
