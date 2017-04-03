namespace HandHistories.Objects.Actions
{
    public enum HandActionType
    {
        UNKNOWN = 0,
        #region Blinds/Posts
        /// <summary>
        /// POST that is added to your bet
        /// </summary>
        POSTS,
        /// <summary>
        /// POST that is not added to your bet
        /// </summary>
        POSTS_DEAD,
        ANTE,
        SMALL_BLIND,
        BIG_BLIND,
        #endregion

        #region Game Actions
        FOLD,
        CALL,
        CHECK,
        RAISE,
        BET,
        UNCALLED_BET,
        #endregion

        #region Showdown Actions
        SHOW,
        SHOWS_FOR_LOW,
        WINS,
        WINS_THE_LOW,
        WINS_SIDE_POT,
        TIES,
        TIES_SIDE_POT,
        MUCKS,
        #endregion

        #region Other
        JACKPOTCONTRIBUTION,
        DEALT_HERO_CARDS,
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
        /// <summary>
        /// this is primarily used by the parsers internally, Use HandAction.IsAllIn instead
        /// </summary>
        ALL_IN, //
        #endregion
    }
}