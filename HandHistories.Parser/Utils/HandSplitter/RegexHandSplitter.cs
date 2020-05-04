using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HandHistories.Parser.Utils.HandSplitter
{
    class RegexHandSplitter
    {
        public static List<string> Split(string text, Regex regex)
        {
            var result = new List<string>();

            var indices = regex.Matches(text)
                .OfType<Match>()
                .ToList();

            for (int i = 0; i + 1 < indices.Count; i++)
            {
                var start = indices[i].Index;
                var end = indices[i + 1].Index;
                result.Add(text.Substring(start, end - start));
            }
            result.Add(text.Substring(indices[indices.Count - 1].Index));

            return result;
        }
    }
}
