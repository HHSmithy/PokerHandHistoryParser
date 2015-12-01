using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.FileIdentifiers
{
    interface IFileIdentifier
    {
        SiteName Site { get; }

        bool Match(string filetext);
    }
}
