using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [DataContract]    
    public enum TableTypeDescription : uint
    {
        [EnumMember]
        Unknown = 0,
        [EnumMember]
        Regular = 0x1 << 0,
        [EnumMember]
        Anonymous = 0x1 << 1,
        [EnumMember]
        SuperSpeed = 0x1 << 2,
        [EnumMember]
        Deep = 0x1 << 3,
        [EnumMember]
        Ante = 0x1 << 4,
        [EnumMember]
        Cap = 0x1 << 5,
        [EnumMember]
        Speed = 0x1 << 6,
        [EnumMember]
        Jackpot = 0x1 << 7,
        [EnumMember]
        SevenDeuceGame = 0x1 << 8,
        [EnumMember]
        FiftyBigBlindsMin = 0x1 << 9,
        [EnumMember]
        Shallow = 0x1 << 10,
        [EnumMember]
        PushFold = 0x1 << 11,
        [EnumMember]
        Zoom = 0x1 << 12,
        [EnumMember]
        Strobe = 0x1 << 13,
        [EnumMember]
        Any = 0x1 << 32,
        [EnumMember]
        All = 0xFFFFFFFF,
    }
}
