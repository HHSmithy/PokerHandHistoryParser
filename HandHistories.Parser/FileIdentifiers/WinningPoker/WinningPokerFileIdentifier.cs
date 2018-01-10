﻿using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Utils.Extensions;

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
            return filetext.StartsWithFast("Game started at: ");
        }
    }
}
