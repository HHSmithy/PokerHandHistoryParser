using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [DataContract]
    public enum GameType : byte 
    {
        [EnumMember]
        Unknown = 0,
        [EnumMember]
        NoLimitHoldem = 1,
        [EnumMember]
        FixedLimitHoldem = 2,
        [EnumMember]
        PotLimitOmaha = 3,
        [EnumMember]
        PotLimitHoldem = 4,
        [EnumMember]
        PotLimitOmahaHiLo = 5,
        [EnumMember]
        CapNoLimitHoldem = 6,
        [EnumMember]
        CapPotLimitOmaha = 7,
        [EnumMember]
        FiveCardPotLimitOmaha = 8,
        [EnumMember]
        FiveCardPotLimitOmahaHiLo = 9,        
        [EnumMember]
        NoLimitOmaha = 10,
        [EnumMember]
        NoLimitOmahaHiLo = 11,
        [EnumMember]
        FixedLimitOmaha = 12,
        [EnumMember]
        FixedLimitOmahaHiLo = 13,
        [EnumMember]
        Any = 31,        
    }
}