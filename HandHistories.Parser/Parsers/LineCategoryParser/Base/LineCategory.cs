using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.Parsers.LineCategoryParser.Base
{
    enum LineCategory
    {
        Ignore,
        Other,
        Header,
        Seat,
        Action,
        Summary,
        Showdown,
    }
}
