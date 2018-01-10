using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Utils.Extensions;

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
            return filetext.StartsWithFast("***** History for hand ");
        }
    }
}
