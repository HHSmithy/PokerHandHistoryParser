using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [DataContract]    
    public enum TableTypeDescription : int 
    {
        [EnumMember] Regular,
        [EnumMember] Anonymous,
        [EnumMember] SuperSpeed,
        [EnumMember] Deep,
        [EnumMember] Ante,
        [EnumMember] Cap,
        [EnumMember] Speed,
        [EnumMember] Jackpot,
        [EnumMember] SevenDeuceGame,
        [EnumMember] FiftyBigBlindsMin,
        [EnumMember] Shallow,
        [EnumMember] PushFold,
        [EnumMember] Any,
        [EnumMember] Unknown,
    }
}