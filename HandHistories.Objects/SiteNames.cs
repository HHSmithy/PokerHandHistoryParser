namespace HandHistories.CommonObjects
{
    public enum SiteNames : byte 
    {
        Unknown = 0,
        PartyPoker = 1,
        FullTilt = 2,
        PokerStars = 3,
        Cereus = 4,
        IPoker = 5,
        OnGame = 6,
        Bodog = 7,
        Merge = 8,
        MicroGaming = 9,
        Entraction = 10,
        Everest = 11,  
        BossMedia = 12,
        Ladbrokes = 13,
        DollaroPoker = 14,
        PokerStarsFr = 15,
        PokerStarsIt = 16,
        PartyPokerFr = 17,
        PartyPokerIt = 18,
        OnGameIt = 19,
        OnGameFr = 20,
        IPokerIt = 21,
        IPokerFr = 22,
        Pacific = 23, // This is 888
        All = 63 // note: can't go higher than 63 due to bit value optimizations
    }
}