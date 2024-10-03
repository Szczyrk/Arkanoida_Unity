[System.Serializable]
public class GameData
{
    // Player's score in the last game session
    public int score;

    // Player's current level in the game
    public int level;

    // Player's health in the last game session
    public int health;

    // 2D array storing the map of the last game session
    public int[,] MapLastGame;

    // Constructor to initialize game data
    public GameData(int score, int[,] map, int level, int health)
    {
        this.score = score;       // Assign player's score
        this.level = level;       // Assign player's current level
        this.health = health;     // Assign player's health
        MapLastGame = map;        // Assign the last game map
    }
}
