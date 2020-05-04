using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Utils.Extensions;

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
            return filetext.StartsWithFast("Full Tilt Poker ");
        }
    }
}
