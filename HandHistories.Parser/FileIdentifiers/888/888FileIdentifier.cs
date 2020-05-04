using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Utils.Extensions;

namespace HandHistories.Parser.FileIdentifiers.Poker888
{
    class Poker888FileIdentifier : IFileIdentifier
    {
        static List<string> SiteStrings = new List<string>() 
        { 
            "888poker",
        };

        public SiteName Site
        {
            get { return SiteName.Pacific; }
        }

        public bool Match(string filetext)
        {
            bool Stage1 = false;
            if (filetext.StartsWithFast("#Game No : "))
            {
                Stage1 = true;
            }
            if (filetext.StartsWithFast("***** "))
            {
                Stage1 = true;
            }

            if (!Stage1)
            {
                return false;
            }

            if (filetext.Length < 220)
            {
                return false;
            }

            foreach (var str in SiteStrings)
            {
                int index = filetext.LastIndexOf(str, 200);

                return index != -1;
            }
            
            return false;
        }
    }
}
