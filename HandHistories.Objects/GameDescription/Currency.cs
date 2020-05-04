using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [DataContract]
    public enum Currency : byte 
    {
        [EnumMember] PlayMoney = 0,
        [EnumMember] USD = 1,
        [EnumMember] GBP = 2,
        [EnumMember] EURO = 3,
        /// <summary>
        /// Only used in tournaments.
        /// </summary>
        [EnumMember] CHIPS = 4,
        /// <summary>
        /// The currency used for buying in to rake point Tournaments.
        /// PokerStars: FPPs
        /// IPoker: iPoints
        /// </summary>
        [EnumMember] RAKE_POINTS = 5,
        /// <summary>
        /// Used for Satelite tournament
        /// </summary>
        [EnumMember] SATELLITE = 6,
        [EnumMember] SEK = 7,
        [EnumMember] CNY = 8,
        [EnumMember] All = 255,
    }
}