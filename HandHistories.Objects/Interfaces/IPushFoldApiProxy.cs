using System.Collections.Generic;
using System.Net;

namespace HandHistories.Objects.Interfaces
{
    /// <summary>
    /// Push Fold tables are not identifiable in the hand-history. This is an optional proxy to an API
    /// which will tell you if the table is push-fold. For HHSmithy we mantain a list based on our lobby scanning
    /// and call a web api.
    /// 
    /// For the hand parser there is no concrete implementation of this.
    /// </summary>
    public interface IPushFoldApiProxy
    {
        HttpStatusCode TryGetTables(out List<string> getTableResponse);

        HttpStatusCode SendTable(string table);
        HttpStatusCode SendTables(string tables);
    }
}
