using UnityEngine;

public class GameLoader : MonoBehaviour
{
    // Singleton instance to ensure only one GameLoader exists in the game
    public static GameLoader Instance;

    // Holds the current game data (score, level, health, map, etc.)
    public GameData gameData;

    // Flag to determine whether to save the game state
    public bool isSave;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Check if an instance of GameLoader already exists
        if (Instance == null)
        {
            // If no instance exists, set this as the Singleton instance
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist this object across scenes
        }
        else
        {
            // If an instance already exists, destroy the duplicate
            Destroy(gameObject);
        }
    }
}
