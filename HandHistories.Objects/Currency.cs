using System.Runtime.Serialization;

namespace HandHistories.CommonObjects
{
    [DataContract]
    public enum Currency : byte 
    {
        [EnumMember] USD = 0,
        [EnumMember] GBP = 1,
        [EnumMember] EURO = 2,
        [EnumMember] All = 7
    }
}