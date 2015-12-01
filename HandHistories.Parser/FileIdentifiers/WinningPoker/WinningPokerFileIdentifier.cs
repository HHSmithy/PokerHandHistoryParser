using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.FileIdentifiers.WinningPoker
{
    class WinningPokerFileIdentifier : IFileIdentifier
    {
        public SiteName Site
        {
            get { return SiteName.WinningPoker; }
        }

        public bool Match(string filetext)
        {
            return filetext.StartsWith("Game started at: ");
        }
    }
}
