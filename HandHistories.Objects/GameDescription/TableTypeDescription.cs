using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [DataContract]    
    public enum TableTypeDescription : byte 
    {
        [EnumMember] 
        Unknown = 0,
        [EnumMember] 
        Regular = 1,
        [EnumMember] 
        Anonymous = 2,
        [EnumMember] 
        SuperSpeed = 3,
        [EnumMember] 
        Deep = 4,
        [EnumMember] 
        Ante = 5,
        [EnumMember] 
        Cap = 6,
        [EnumMember] 
        Speed = 7,
        [EnumMember] 
        Jackpot = 8,
        [EnumMember] 
        SevenDeuceGame = 9,
        [EnumMember] 
        FiftyBigBlindsMin = 10,
        [EnumMember] 
        Shallow = 11,
        [EnumMember] 
        PushFold = 12,
        [EnumMember] 
        Zoom = 13,
        [EnumMember] 
        Strobe = 14,
        [EnumMember]
        Any = 30,
        [EnumMember]
        All = 31,
    }
}
