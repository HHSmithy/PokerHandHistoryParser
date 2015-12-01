using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.FileIdentifiers.FullTiltPoker
{
    class FullTiltPokerFileIdentifier : IFileIdentifier
    {
        public SiteName Site
        {
            get { return SiteName.FullTilt; }
        }

        public bool Match(string filetext)
        {
            return filetext.StartsWith("Full Tilt Poker ");
        }
    }
}
