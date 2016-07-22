using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Utils.Extensions;

namespace HandHistories.Parser.FileIdentifiers.PokerStars
{
    class PokerStarsFileIdentifier : IFileIdentifier
    {
        public SiteName Site
        {
            get { return SiteName.PokerStars; }
        }

        public bool Match(string filetext)
        {
            return filetext.StartsWithFast("PokerStars ");
        }
    }
}
