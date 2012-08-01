using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Base;

namespace HandHistories.Parser.Parsers.Factory
{
    public interface IHandHistoryParserFactory
    {
        IHandHistoryParser GetFullHandHistoryParser(SiteName siteName);

        IHandHistorySummaryParser GetHandHistorySummaryParser(SiteName siteName);        
    }
}