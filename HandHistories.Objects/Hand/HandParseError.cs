using System;
using HandHistories.Objects.GameDescription;

namespace HandHistories.Objects.Hand
{
    public class HandParseError
    {
        public HandParseError(string handText, SiteName site, Exception ex)
        {
            HandText = handText;
            Site = site;
            Exception = ex;
        }

        public string HandText { get; private set; }
        public SiteName Site { get; set; }
        public Exception Exception { get; private set; }
    }
}