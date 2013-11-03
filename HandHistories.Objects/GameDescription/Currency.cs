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
        [EnumMember] All = 0xff
    }
}