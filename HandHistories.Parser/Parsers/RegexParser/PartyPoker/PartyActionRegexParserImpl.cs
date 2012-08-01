using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.RegexParser.PartyPoker;

namespace HandHistories.Parser.Parsers.PartyPoker
{
    public class PartyActionRegexParserImpl : HandHistoryRegexActionParserImplBase
    {
        // note couldnt find DOTALL so used (.|\r)* instead
        public override string StartOfActionsRegex { get { return @"(?<=\)\r)(?!Seat)(.|\r)*"; } }

        // Mantis Bug 92 - this is code duplicaion between partyhandregexparser

        public override string BoardRegexFlop { get { return @"(?<=\*\* Dealing Flop \*\* \[).*(?=\])"; } }

        public override string BoardRegexTurn { get { return @"(?<=\*\* Dealing Turn \*\* \[).*(?=\])"; } }

        public override string BoardRegexRiver { get { return @"(?<=\*\* Dealing River \*\* \[).*(?=\])"; } }

        private SiteActionRegexesBase _siteActionRegexs = null;

        public override SiteActionRegexesBase SiteActionRegexes
        {
            get
            {
                if (_siteActionRegexs == null)
                {
                    _siteActionRegexs = new PartySiteActionRegexes();
                }

                return _siteActionRegexs;
            }
        }
    }
}
