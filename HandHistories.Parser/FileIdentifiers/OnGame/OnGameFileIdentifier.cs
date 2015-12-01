using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.FileIdentifiers.OnGame
{
    class OnGameFileIdentifier : IFileIdentifier
    {
        public SiteName Site
        {
            get { return SiteName.OnGame; }
        }

        public bool Match(string filetext)
        {
            return filetext.StartsWith("***** History for hand ");
        }
    }
}
