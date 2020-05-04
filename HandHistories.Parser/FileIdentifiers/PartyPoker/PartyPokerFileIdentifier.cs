using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Utils.Extensions;

namespace HandHistories.Parser.FileIdentifiers.PartyPoker
{
    class PartyPokerFileIdentifier : IFileIdentifier
    {
        public SiteName Site
        {
            get { return SiteName.PartyPoker; }
        }

        public bool Match(string filetext)
        {
            return filetext.StartsWithFast("***** Hand History for Game ");
        }
    }
}
