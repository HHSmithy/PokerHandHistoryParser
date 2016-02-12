using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Utils
{
    class XMLHandLineSplitter
    {
        internal static string[] Split(string handText)
        {
            int lineStartIndex = 0;
            int lineEndIndex = 0;

            List<string> lines = new List<string>();

            while (lineStartIndex != -1)
            {
                lineEndIndex = FindXMLTagEnd(handText, lineStartIndex + 2);

                string line = handText.Substring(lineStartIndex, lineEndIndex - lineStartIndex);
                lines.Add(line);

                lineStartIndex = handText.IndexOf('<', lineEndIndex);
            }

            if (lines[0].StartsWith("<?xml"))
            {
                lines.RemoveAt(0);
            }

            return lines.ToArray();
        }

        static int FindXMLTagEnd(string text, int startIndex)
        {
            if (text[startIndex + 1] == '/')
            {
                return text.IndexOf('>', startIndex + 2) + 1;
            }

            int end = text.IndexOfAny(new char[] { '/', '<' }, startIndex + 1);

            if (end == -1)
            {
                return text.IndexOf('>', startIndex + 1);
            }

            char c = text[end];
            char c2 = text[end + 1];
            if (c == '/' && c2 == '>')
            {
                return end + 1;
            }
            else if (c == '<' && c2 == '/')
            {
                return text.IndexOf('>', end + 1) + 1;
            }
            else if (c == '<')
            {
                int prevEnd = text.LastIndexOf('>', end);

                return prevEnd + 1;
            }
            else
            {
                return FindXMLTagEnd(text, end);
            }
        }
    }
}
