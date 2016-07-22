using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Utils.Extensions;

namespace HandHistories.Parser.FileIdentifiers.Winamax
{
    class WinamaxFileIdentifier : IFileIdentifier
    {
        public SiteName Site
        {
            get { return SiteName.Winamax; }
        }

        public bool Match(string filetext)
        {
            return filetext.StartsWithFast("Winamax Poker - ");
        }
    }
}
