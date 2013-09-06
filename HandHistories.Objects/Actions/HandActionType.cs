namespace HandHistories.Objects.Actions
{
    public enum HandActionType : byte
    {
        UNKNOWN = 0x0,
        FOLD,
        CALL,
        CHECK,
        RAISE,
        BET,
        ALL_IN,
        ANTE,
        SHOW,
        SHOWS_FOR_LOW,
        DEALT_HERO_CARDS,
        UNCALLED_BET,
        REQUEST_TIME,
        FIFTEEN_SECONDS_LEFT,
        FIVE_SECONDS_LEFT,
        MUCKS,
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

        #region Blinds
        POSTS = 0x40,
        SMALL_BLIND = 0x41,
        BIG_BLIND = 0x42,
        #endregion

        #region Wins
        WINS = 0x80,
        WINS_THE_LOW = 0x81,
        WINS_SIDE_POT = 0x82,
        TIES = 0x83,
        TIES_SIDE_POT = 0x84,
        #endregion
    }
}