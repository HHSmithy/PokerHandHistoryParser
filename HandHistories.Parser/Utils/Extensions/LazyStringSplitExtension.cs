using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils.Extensions
{
    static class LazyStringSplitExtension
    {
        public static IEnumerable<string> LazyStringSplit(this string str, string splitter, StringComparison comp = StringComparison.Ordinal)
        {
            int l = str.Length;
            int start = 0, end = str.IndexOf(splitter, comp);
            if (end == -1) // No such substring
            {
                yield return str; // Return original and break
                yield break;
            }

            while (end != -1)
            {
                //if (end - start > 0) // Non empty? 
                //{
                yield return str.Substring(start, end - start); // Return non-empty match
                //}
                start = end + splitter.Length;
                end = str.IndexOf(splitter, start, comp);
            }

            if (start < l) // Has remainder?
            {
                yield return str.Substring(start, l - start); // Return remaining trail
            }
        }

        /// <summary>
        /// This LazySting split checks directly for an immediate split then jumps forward a couple of characters based
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitter"></param>
        /// <param name="jump">characters to skip if no immediate split</param>
        /// <param name="jumpAfter">characters to check before jumping</param>
        /// <returns></returns>
        public static IEnumerable<string> LazyStringSplitFastSkip(this string str, char splitter, int jump, int jumpAfter = 0)
        {
#if DEBUG
            if (jump <= jumpAfter)
            {
                throw new ArgumentException("jump cant be less or equal to jumpAfter");
            }

            if (jump < 0)
            {
                throw new ArgumentException("jump cant be less then 0");
            }
#endif

            int l = str.Length;
            int start = 0, end = str.IndexOf(splitter);
            if (end == -1) // No such substring
            {
                yield return str; // Return original and break
                yield break;
            }

            while (end != -1)
            {
                //if (end - start > 0) // Non empty? 
                //{
                yield return str.Substring(start, end - start);
                //}

                start = end + 1;

                if (str.Length < jump + start)
                {
                    end = str.IndexOf(splitter, start);
                }
                else if (jumpAfter == 0)
                {
                    end = str.IndexOf(splitter, start + jump);
                }
                else
                {
                    end = str.IndexOf(splitter, start, jumpAfter);

                    if (end == -1)
                    {
                        end = str.IndexOf(splitter, start + jump);
                    }
                }
            }

            if (start < l) // Has remainder?
            {
                yield return str.Substring(start, l - start); // Return remaining trail
            }
        }
    }
}
