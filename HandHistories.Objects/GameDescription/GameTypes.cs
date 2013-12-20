using System;
using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [Flags]
    [DataContract]
    public enum GameType : byte 
    {
        [EnumMember]
        Unknown = 0,
        #region Holdem
        [EnumMember]
        Holdem = 1 << 3,
        [EnumMember]
        NoLimitHoldem = Holdem | 1,
        [EnumMember]
        CapNoLimitHoldem = Holdem | 2,
        [EnumMember]
        FixedLimitHoldem = Holdem | 3,
        [EnumMember]
        PotLimitHoldem = Holdem | 4,
        #endregion

        #region Omaha
        [EnumMember]
        Omaha = 1 << 4,
        [EnumMember]
        PotLimitOmaha = Omaha | 1,
        [EnumMember]
        CapPotLimitOmaha = Omaha | 2,
        [EnumMember]
        FiveCardPotLimitOmaha = Omaha | 3,
        [EnumMember]
        NoLimitOmaha = Omaha | 4,
        [EnumMember]
        FixedLimitOmaha = Omaha | 5,
        #endregion
        
        #region OmahaHiLow
        [EnumMember]
        OmahaHiLow = 1 << 5,
        [EnumMember]
        PotLimitOmahaHiLo = OmahaHiLow | 1,
        [EnumMember]
        FiveCardPotLimitOmahaHiLo = OmahaHiLow | 2,
        [EnumMember]
        NoLimitOmahaHiLo = OmahaHiLow | 3,
        [EnumMember]
        FixedLimitOmahaHiLo = OmahaHiLow | 4,
        #endregion
        [EnumMember]
        Any = 31,        
    }
}