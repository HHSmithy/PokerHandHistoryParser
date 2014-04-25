using System;
using System.Collections.Generic;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Interfaces;

namespace HandHistories.Parser.Parsers.Base
{
    public interface IHandHistorySummaryParser
    {
        SiteName SiteName { get; }
        
        IEnumerable<string> SplitUpMultipleHands(string rawHandHistories);

        HandHistorySummary ParseFullHandSummary(string handText, bool rethrowExceptions = false);
        int ParseDealerPosition(string handText);
        
        DateTime ParseDateUtc(string handText);

        long ParseHandId(string handText);

        string ParseTableName(string handText);

        GameDescriptor ParseGameDescriptor(string handText);

        SeatType ParseSeatType(string handText);

        GameType ParseGameType(string handText);

        TableType ParseTableType(string handText);

        Limit ParseLimit(string handText);

        int ParseNumPlayers(string handText);        

        /// <summary>
        /// An intial bit of verification to check if the hand text
        /// is valid. For instance Party hands must contain ' wins '.
        /// </summary>
        /// <param name="handText">The entire hand text.</param>
        /// <returns>True if the hand is valid. False if not.</returns>
        bool IsValidHand(string handText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handText"></param>
        /// <param name="isCancelled"></param>
        /// <returns>True if the hand is valid, false if not. Outs the cancelled state.</returns>
        bool IsValidOrCancelledHand(string handText, out bool isCancelled);
    }
}