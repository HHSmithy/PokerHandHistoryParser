using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.FileIdentifiers.MicroGaming
{
    class MicroGamingFileIdentifier : IFileIdentifier
    {
        public SiteName Site
        {
            get { return SiteName.MicroGaming; }
        }

        public bool Match(string filetext)
        {
            return filetext.StartsWith("<Game hhversion=\"4\"");
        }
    }
}
