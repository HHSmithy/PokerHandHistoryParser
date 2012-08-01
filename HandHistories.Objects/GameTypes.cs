using System.Runtime.Serialization;

namespace HandHistories.CommonObjects
{
    [DataContract()]
    public enum GameTypes : byte 
    {
        [EnumMember]
        NLH = 1,
        [EnumMember]
        FL = 2,
        [EnumMember]
        PLO = 3,
        [EnumMember]
        PLH = 4,
        [EnumMember]
        PLO_HILO = 5,
        [EnumMember]
        CAP_NL = 6,
        [EnumMember]
        CAP_PLO = 7,
        [EnumMember]
        FIVE_CARD_PLO = 8,
        [EnumMember]
        FIVE_CARD_PLO_HILO = 9,
        [EnumMember]
        Unknown = 0,
        [EnumMember]
        Any = 30,
        [EnumMember]
        All = 31,
    }
}