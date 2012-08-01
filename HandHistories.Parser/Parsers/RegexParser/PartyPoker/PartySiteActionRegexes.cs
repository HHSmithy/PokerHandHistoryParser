using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.RegexParser.PartyPoker;

namespace HandHistories.Parser.Parsers.PartyPoker
{
    public class PartySiteActionRegexes : SiteActionRegexesBase
    {
        protected override string FoldRegex
        {
            get { return @"^.* folds$"; }
        }

        protected override string AddsRegex
        {
            // Mantis Bug 94 need to track down a hh for this
            get { return null; }
        }

        protected override string WinsSidePotRegex
        {
            get { return @"^.* wins \$[0-9,.]+ USD from the side pot [0-9]+ with .*$"; }
        }

        protected override string WinsPotRegex
        {
            get { return @"(^.* wins \$[0-9,.]+ USD$)|(^.* wins \$[0-9,.]+ USD from the main pot with .*$)"; }
        }

        protected override string WinsTheLowRegex
        {
            get { return @"^.* wins Lo \(\$[0-9,.]+ USD\) from the main pot with .*$"; }
        }

        protected override string UncalledBetRegex
        {
            // Party poker doesn't let you know about uncalled bets
            get { return null; }
        }

        protected override string TimesOutRegex
        {
            get { return @"^.* did not respond in time$"; }
        }

        protected override string TiesSidePotRegex
        {
            // Mantis Bug 94 need to track down a hh for this
            get { return null; }
        }

        protected override string TiesPotRegex
        {
            // Mantis Bug 94 need to track down a hh for this
            get { return null; }
        }

        protected override string StandsUpRegex
        {
            get { return @"^.* has left the table\.$"; }
        }

        protected override string PostsSmallBlindRegex
        {
            get { return @"^.* posts small blind \[\$[0-9.,]+ USD\]\.$"; }
        }

        protected override string PostsBigBlindRegex
        {
            get { return @"^.* posts big blind \[\$[0-9.,]+ USD\]\.$"; }
        }

        protected override string SittingOutRegex
        {
            get { return @"^.* is sitting out$"; }
        }

        protected override string SitsDownRegex
        {
            get { return @"^.* has joined the table\.$"; }
        }

        protected override string ShowsRegex
        {
            get { return @"^.* show[s]* \[ .* \].*$"; }
        }

        protected override string ShowsForLow
        {
            get { return @"^.* show[s]*.*for low.$"; }
        }

        protected override string SecondsToReconnectRegex
        {
            // Mantis Bug 94 need to track down a hh for this
            get { return null;  }
        }

        protected override string RaiseToRegex
        {
            get { return @"^.* raises \[\$[0-9,.]+ USD\]$"; }
        }

        protected override string PostsRegex
        {            
            get { return @"^.* posts big blind \+ dead \[\$[0-9.,]+\]\.$"; }
        }

        protected override string AllInRegex
        {
            get { return @"^.* is all-In[ ]+[\$[0-9,.]+ USD\]$"; }
        }

        protected override string HasReturnedRegex
        {
            // Mantis Bug 94 need to track down a hh for this
            get { return null; }
        }

        protected override string RequestsTimeRegex
        {
            get { return @"^.* will be using his time bank for this hand.$"; }
        }

        protected override string ReconnectedRegex
        {
            get { return @"^.* has been reconnected and has [0-9]+ seconds to act.$"; }
        }

        // Party doesn't have Ante tables as of 5-9-2011
        protected override string AntesRegex
        {
            get { return null; }
        }

        protected override string BetsRegex
        {
            get { return @"^.* bets \[\$[0-9.,]+ USD\]$"; }
        }

        protected override string CallsRegex
        {
            get { return @"^.* calls [\$[0-9,.]+ USD\]$"; }
        }

        protected override string ChatRegex
        {
            get { return @"^.*: "; }
        }

        protected override string ChecksRegex
        {
            get { return @"^.* checks$"; }
        }

        protected override string DisconnectedRegex
        {
            get { return @"^.* is disconnected. We will wait for .* to reconnect for a maximum of [0-9]+ seconds.$"; }
        }        

        protected override string SecondsLeftToActRegex
        {
            // Mantis Bug 94 need to track down a hh for this
            get { return null; }
        }

        protected override string MucksRegex
        {
            get { return @"^.* does not show cards."; }
        }

        public override string AmountRegex
        {
            get { return @"(?<=\$)[0-9.,]+(?= USD)"; }
        }
    }
}
