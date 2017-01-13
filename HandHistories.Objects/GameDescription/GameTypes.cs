using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    //[DataContract]
    //public enum GameType : byte 
    //{
    //    [EnumMember]
    //    Unknown = 0,
    //    [EnumMember]
    //    NoLimitHoldem = 1,
    //    [EnumMember]
    //    FixedLimitHoldem = 2,
    //    [EnumMember]
    //    PotLimitOmaha = 3,
    //    [EnumMember]
    //    PotLimitHoldem = 4,
    //    [EnumMember]
    //    PotLimitOmahaHiLo = 5,
    //    [EnumMember]
    //    CapNoLimitHoldem = 6,
    //    [EnumMember]
    //    CapPotLimitOmaha = 7,
    //    [EnumMember]
    //    FiveCardPotLimitOmaha = 8,
    //    [EnumMember]
    //    FiveCardPotLimitOmahaHiLo = 9,        
    //    [EnumMember]
    //    NoLimitOmaha = 10,
    //    [EnumMember]
    //    NoLimitOmahaHiLo = 11,
    //    [EnumMember]
    //    FixedLimitOmaha = 12,
    //    [EnumMember]
    //    FixedLimitOmahaHiLo = 13,
    //    [EnumMember]
    //    Any = 31,        
    //}

    [DataContract]
    public enum GameLimitEnum : byte
    {
        [EnumMember]
        NoLimit,
        [EnumMember]
        FixedLimit,
        [EnumMember]
        PotLimit,
    }

    [DataContract]
    public enum GameEnum : byte
    {
        [EnumMember]
        Unknown,
        [EnumMember]
        Holdem,
        [EnumMember]
        Omaha,
        [EnumMember]
        FiveCardOmaha,
        [EnumMember]
        OmahaHiLo,
        [EnumMember]
        FiveCardOmahaHiLo,
        [EnumMember]
        Any = 255,
    }

    [DataContract]
    public partial struct GameType
    {
        [DataMember]
        public readonly bool Cap;
        [DataMember]
        public readonly GameLimitEnum Limit;
        [DataMember]
        public readonly GameEnum Game;

        public GameType(GameLimitEnum Limit, GameEnum Game, bool Cap = false)
        {
            this.Cap = Cap;
            this.Limit = Limit;
            this.Game = Game;
        }

        #region Operators
        public static bool operator ==(GameType g1, GameType g2)
        {
            return g1.Cap == g2.Cap && g1.Game == g2.Game && g1.Limit == g2.Limit;
        }

        public static bool operator !=(GameType g1, GameType g2)
        {
            return g1.Cap != g2.Cap || g1.Game != g2.Game || g1.Limit != g2.Limit;
        }
        #endregion

        public override string ToString()
        {
            return (Cap ? "Cap " : "") + Limit.ToString() + Game.ToString();
        }
    }
}