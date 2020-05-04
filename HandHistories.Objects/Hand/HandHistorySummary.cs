using System;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;

namespace HandHistories.Objects.Hand
{
    public class HandHistorySummary
    {
        public HandHistorySummary()
        {           
            GameDescription = new GameDescriptor();            
        }

        public DateTime DateOfHandUtc { get; set; }

        /// <summary>
        /// Does not necessarily use the same separator as the site does
        /// </summary>
        public string HandIdString { get { return HandID.GetString(HandId); } }

        /// <summary>
        /// Hand id is represented as a long array because sites such as ongame, merge etc. have seperated ids for tableid and handid.
        /// </summary>
        public long[] HandId { get; set; }

        /// <summary>
        /// Dealers seat.
        /// Get the dealer with Players[DealerButtonPosition].
        /// Can be -1 if it's a cancelled hand.
        /// </summary>
        public int DealerButtonPosition { get; set; }

        public string TableName { get; set; }

        public GameDescriptor GameDescription { get; set; }

        public bool Cancelled { get; set; }

        public int NumPlayersSeated { get; set; }

        public string FullHandHistoryText { get; set; }

        public IEnumerable<string> FullHandHistoryLines { get; set; }

        /// <summary>
        /// The sum of all bets and blinds excluding uncalled bets and winnings. Includes Rake
        /// </summary>
        public decimal? TotalPot { get; set; }

        public decimal? Rake { get; set; }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            HandHistorySummary hand = obj as HandHistorySummary;
            if (hand == null) return false;

            return ToString().Equals(hand.ToString());
        }
           
        public override string ToString()
        {
            return string.Format("[{0}] {1}", HandId, GameDescription.ToString());
        }
    }
}
