using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Parsers.LineCategoryParser.Base
{
    class Categories
    {
        public readonly List<string> Header = new List<string>();
        public readonly List<string> Seat = new List<string>(10);
        public readonly List<string> Action = new List<string>();
        public readonly List<string> Showdown = new List<string>();
        public readonly List<string> Summary = new List<string>(10);
        public readonly List<string> Other = new List<string>();
        public readonly List<string> Ignore = new List<string>();

        public void Add(LineCategory cat, string line)
        {
            switch (cat)
            {
                case LineCategory.Header:
                    Header.Add(line);
                    break;
                case LineCategory.Seat:
                    Seat.Add(line);
                    break;
                case LineCategory.Action:
                    Action.Add(line);
                    break;
                case LineCategory.Showdown:
                    Showdown.Add(line);
                    break;
                case LineCategory.Summary:
                    Summary.Add(line);
                    break;
                case LineCategory.Other:
                    Other.Add(line);
                    break;
                case LineCategory.Ignore:
                    Ignore.Add(line);
                    break;
                default:
                    throw new ArgumentException("Unknown category: " + cat);
            }
        }

        public void Clear()
        {
            Header.Clear();
            Seat.Clear();
            Action.Clear();
            Showdown.Clear();
            Summary.Clear();
            Other.Clear();
            Ignore.Clear();
        }
    }
}
