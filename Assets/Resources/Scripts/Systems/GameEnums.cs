public enum TileTaken
{
    NONE,
    PLAYER_A_TAKEN,
    PLAYER_B_TAKEN,
    PLAYER_Com_TAKEN,
}

public enum GameMode
{
    NONE,
    ONLINE,
    OFFLINE
}

public enum Turn
{
    NONE,
    PLAYER_A,
    PLAYER_B,
    PLAYER_Com,
}

public enum Piece
{
    NONE = -1,
    SMALL = 0,
    MEDIUM = 1,
    LARGE = 2,
    BYPASS=-99
}

public enum GameState
{
    IDLE,
    START,
    END
}
