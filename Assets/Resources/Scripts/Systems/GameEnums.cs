public enum TileTaken
{
    NONE,
    PLAYER_A_TAKEN,
    PLAYER_B_TAKEN
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
    PLAYER_B
}

public enum Piece
{
    NONE = -1,
    SMALL = 0,
    MEDIUM = 1,
    LARGE = 2
}

public enum GameState
{
    IDLE,
    START,
    END
}
