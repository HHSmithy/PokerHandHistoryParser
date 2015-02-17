namespace HandHistories.Objects.Actions
{
    public enum ActionTypeFlags
    {
        None = 0,
        PlayerAction = GameAction | (1 << 16),
        GameAction = 1 << 15,
        ShowDown = 1 << 14,
    }

    public enum HandActionType
    {
        UNKNOWN     = 0,
        FOLD        = (int)ActionTypeFlags.PlayerAction + 1,
        CALL        = (int)ActionTypeFlags.PlayerAction + 2,
        CHECK       = (int)ActionTypeFlags.PlayerAction + 3,
        RAISE       = (int)ActionTypeFlags.PlayerAction + 5,
        BET         = (int)ActionTypeFlags.PlayerAction + 6,
        ALL_IN      = (int)ActionTypeFlags.PlayerAction + 7,
        ANTE        = (int)ActionTypeFlags.GameAction + 1,
        SMALL_BLIND = (int)ActionTypeFlags.GameAction + 2,
        BIG_BLIND   = (int)ActionTypeFlags.GameAction + 3,
        POSTS       = (int)ActionTypeFlags.GameAction + 4,
        UNCALLED_BET = (int)ActionTypeFlags.GameAction + 5,
        SHOW            = (int)ActionTypeFlags.ShowDown + 1,
        SHOWS_FOR_LOW   = (int)ActionTypeFlags.ShowDown + 2,
        WINS            = (int)ActionTypeFlags.ShowDown + 3,
        WINS_THE_LOW    = (int)ActionTypeFlags.ShowDown + 4,
        WINS_SIDE_POT   = (int)ActionTypeFlags.ShowDown + 5,
        TIES            = (int)ActionTypeFlags.ShowDown + 6,
        TIES_SIDE_POT   = (int)ActionTypeFlags.ShowDown + 7,
        MUCKS           = (int)ActionTypeFlags.ShowDown + 8,
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
    }
}