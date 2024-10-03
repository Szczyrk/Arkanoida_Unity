using System;
using System.Linq;
using Arkanodia.ObjectPools;
using Arkanodia.Players;
using Arkanodia.MapGenerators;
using UnityEngine;
using Zenject;

namespace Arkanodia.Core
{
    public class GameManager : MonoBehaviour, IGameManager // Class implementing IGameManager interface
    {
        private IGridBocks _gridBocks; // Reference to the grid blocks
        private IPlayer _player; // Reference to the player
        private CurrentScore _currentScore; // Reference to the current score
        private IObjectPool<Block> _blockPool; // Object pool for blocks
        public int Level { get; set; } // Current level of the game
        
        public static event Action ChangedLevel; // Event for level changes

        private void OnEnable()
        {
            Debug.Log("GameManager enabled."); // Log message indicating that GameManager is enabled
            CurrentScore.GetPoint += IsNextLevel; // Subscribe to the GetPoint event
        }

        private void OnDisable()
        {
            Debug.Log("GameManager disabled."); // Log message indicating that GameManager is disabled
            CurrentScore.GetPoint -= IsNextLevel; // Unsubscribe from the GetPoint event
        }

        // Method for injecting dependencies via Zenject
        [Inject]
        public void Setup(IGridBocks gridBocks, IPlayer player, CurrentScore currentScore, IObjectPool<Block> blockPool)
        {
            _gridBocks = gridBocks; // Initialize grid blocks
            _player = player; // Initialize player
            _currentScore = currentScore; // Initialize current score
            _blockPool = blockPool; // Initialize block pool
            Debug.Log("GameManager setup with dependencies."); // Log message for successful setup
        }

        // Method to check if the player can proceed to the next level
        private void IsNextLevel()
        {
            Debug.Log("Checking if all blocks are indestructible."); // Log for checking block status
            
            // Log number of objects in the game for easier debugging
            Debug.Log($"Total blocks in game: {_blockPool.ObjectInGame.Count}"); 

            // Check if all blocks are indestructible
            bool allIndestructible = _blockPool.ObjectInGame.All(o => o.BlockType.isIndestructible);
            Debug.Log($"All blocks indestructible: {allIndestructible}"); // Log the result of the check

            if (allIndestructible) // If all blocks are indestructible
            {
                Debug.Log("Proceeding to next level."); // Log for proceeding to next level
                NextLevel(); // Call NextLevel method
            }
            else
            {
                Debug.Log("Not all blocks are indestructible. Continuing current level."); // Log for staying in current level
            }
        }

        // Method to move to the next level
        private void NextLevel()
        {
            Level++; // Increment the level
            Debug.Log($"Moving to next level: {Level}"); // Log the new level
            _gridBocks.CreateMap(null); // Create a new map
            ChangedLevel?.Invoke(); // Invoke the ChangedLevel event
        }

        // Method to load game data
        public void LoadGame(GameData gameData)
        {
            if (gameData == null) // Check if game data is null
            {
                Debug.Log("No game data found to load."); // Log for missing game data
                return; // Exit method
            }

            Debug.Log($"Loading game at level {gameData.level}."); // Log for loading game at specific level
            Level = gameData.level; // Set the current level
            ChangedLevel?.Invoke(); // Invoke the ChangedLevel event
            _gridBocks.CreateMap(gameData.MapLastGame); // Create the map from saved data
            _player.ChangeLife(gameData.health - _player.Life); // Adjust player's life based on saved data
            _currentScore.AddScore(gameData.score - _currentScore.Score); // Adjust score based on saved data
        }
    }
}