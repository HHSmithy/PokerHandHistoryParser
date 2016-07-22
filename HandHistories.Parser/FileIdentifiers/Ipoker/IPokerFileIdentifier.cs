using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Utils.Extensions;

namespace HandHistories.Parser.FileIdentifiers.IPoker
{
    class IPokerFileIdentifier : IFileIdentifier
    {
        public SiteName Site
        {
            get { return SiteName.IPoker; }
        }

        public bool Match(string filetext)
        {
            bool stage1 = false;
            if (filetext.StartsWithFast("<?xml"))
            {
                stage1 = true;
            }
            else if(filetext.StartsWithFast("<session sessioncode="))
            {
                return true;
            }

            if (stage1)
            {
                if (filetext.LastIndexOfFast("<session sessioncode=", 200) != -1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
