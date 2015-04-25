using System;
using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [Flags]
    public enum TableTypeDescription : long
    {
        // General TableTypes
        [EnumMember]
        Unknown = 0,
        [EnumMember]
        Regular = 0x1L << 0,
        [EnumMember]
        Anonymous = 0x1L << 1,
        [EnumMember]
        SuperSpeed = 0x1L << 2,
        [EnumMember]
        Deep = 0x1L << 3,
        [EnumMember]
        Ante = 0x1L << 4,
        [EnumMember]
        Cap = 0x1L << 5,
        [EnumMember]
        Speed = 0x1L << 6,
        [EnumMember]
        Jackpot = 0x1L << 7,
        [EnumMember]
        SevenDeuceGame = 0x1L << 8,
        [EnumMember]
        FiftyBigBlindsMin = 0x1L << 9,
        [EnumMember]
        Shallow = 0x1L << 10,
        [EnumMember]
        PushFold = 0x1L << 11,
        [EnumMember]
        Zoom = 0x1L << 12,
        [EnumMember]
        Strobe = 0x1L << 13,
        [EnumMember]
        Rush = 0x1L << 14,
        [EnumMember]
        SpeedPoker = 0x1L << 15,
        [EnumMember]
        FastForward = 0x1L << 16,
        [EnumMember]
        Snap = 0x1L << 17,
        [EnumMember]
        Turbo = 0x1L << 18,
        [EnumMember]
        HyperTurbo = 0x1L << 19,
        [EnumMember]
        Slow = 0x1L << 20,

        // SNG specific tabletypes
        [EnumMember]
        Rebuy = 0x1L << 21,
        [EnumMember]
        NoBlindIncreases = 0x1L << 22,
        [EnumMember]
        Knockout = 0x1L << 23,
        [EnumMember]
        ProgressiveKnockout = 0x1L << 24,
        [EnumMember]
        Fifty50 = 0x1L << 25,


        // This is an indicator for the max amount of players in the SNG according to latest PokerStars TableType descriptions
        // e.g. a HU SNG can have 2,4,16 or 32 players involved
        // Not needed for other sites ( mostly because they don't have a huge variety in SNG types )
        // Might be worth to move this to its own type, but at HHSmithy we have various internal reasons to keep it here
        [EnumMember]
        P2 = 0x1L << 26,
        [EnumMember]
        P4 = 0x1L << 27,
        [EnumMember]
        P16 = 0x1L << 28,
        [EnumMember]
        P32 = 0x1L << 29,
        [EnumMember]
        P12 = 0x1L << 30,
        [EnumMember]
        P18 = 0x1L << 31,
        [EnumMember]
        P27 = 0x1L << 32,
        [EnumMember]
        P45 = 0x1L << 33,
        [EnumMember]
        P90 = 0x1L << 34,
        [EnumMember]
        P180 = 0x1L << 35,
        [EnumMember]
        P240 = 0x1L << 36,
        [EnumMember]
        P360 = 0x1L << 37,
        [EnumMember]
        P990 = 0x1L << 38,

        [EnumMember]
        Any = 0x1L << 63,
        [EnumMember]
        All = 0x7FFFFFFFFFFFFFFF
    }
}
