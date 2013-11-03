using System;
using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [Flags]
    [DataContract]    
    public enum TableTypeDescription : int 
    {
        [EnumMember] Regular = 0x1,
        [EnumMember] Anonymous = 0x2,
        [EnumMember] SuperSpeed = 0x4,
        [EnumMember] Deep = 0x8,
        [EnumMember] Ante = 0x10,
        [EnumMember] Cap = 0x20,
        [EnumMember] Speed = 0x40,
        [EnumMember] Jackpot = 0x80,
        [EnumMember] SevenDeuceGame = 0x100,
        [EnumMember] FiftyBigBlindsMin = 0x200,
        [EnumMember] Shallow = 0x400,
        [EnumMember] PushFold = 0x800,
        [EnumMember] Any = 0x1000,
        [EnumMember] Zoom = 0x2000,
        [EnumMember] Strobe = 0x4000,
        [EnumMember] All = -1,
        [EnumMember] Unknown = 0,
    }
}
