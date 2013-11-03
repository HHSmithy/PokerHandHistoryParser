namespace HandHistories.Objects.Actions
{
    public enum HandActionFlags : byte
    {
        VPIP = 0x21
    }

    public enum HandActionType : byte
    {
        UNKNOWN = 0x0, 
        SHOWS_FOR_LOW,
        DEALT_HERO_CARDS,
        UNCALLED_BET,
        REQUEST_TIME,
        FIFTEEN_SECONDS_LEFT,
        FIVE_SECONDS_LEFT,
        DISCONNECTED,
        RECONNECTED,
        STANDS_UP,
        SITS_DOWN,
        SITTING_OUT,
        ERROR,
        ADDS,
        TIMED_OUT,
        RETURNED,
        SECONDS_TO_RECONNECT,
        CHAT,
        FEELING_CHANGE,
        GAME_CANCELLED,
        RABBIT,

        #region GameAction
        FOLD = 0x20,
        CHECK = 0x22,
        BET = 0x21,
        CALL = 0x23,
        RAISE = 0x25,
        ALL_IN = 0x27,
        #endregion

        #region Blinds
        POSTS = 0x40,
        SMALL_BLIND = 0x41,
        BIG_BLIND = 0x42,
        ANTE = 0x43,
        #endregion

        #region ShowDown Action
        MUCKS = 0x80,
        SHOW = 0x88,
        WINS = 0x81,
        WINS_THE_LOW = 0x85,
        WINS_SIDE_POT = 0x89,
        TIES = 0x87,
        TIES_SIDE_POT = 0x83,
        #endregion
    }
}