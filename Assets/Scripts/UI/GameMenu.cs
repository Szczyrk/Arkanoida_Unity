using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using Arkanodia.Core;
using Arkanodia.Players;
using Arkanodia.MapGenerators;

namespace Arkanodia.UI
{
    public class GameMenu : MonoBehaviour
    {
        // Serialized fields for UI Buttons and Game Menu Panel
        [SerializeField] private Button ButtonBack; // Button to return to the game
        [SerializeField] private Button ButtonSave; // Button to save the game
        [SerializeField] private Button ButtonQuit; // Button to quit to main menu
        [SerializeField] private GameObject GameMenuPanel; // Panel that displays the game menu

        // Dependencies
        private ISaveGame _saveGame; // Interface for saving game data
        private CurrentScore _currentScore; // Class to track the current score
        private IPlayer _player; // Interface for the player
        private IGameManager _gameManager; // Interface for managing game states
        private IGridBocks _gridBocks; // Interface for grid blocks in the game

        // Subscribe to GameOver event when this script is enabled
        private void OnEnable() => GameOverManager.GameOver += LoadMainMenu;
        
        // Unsubscribe from GameOver event when this script is disabled
        private void OnDisable() => GameOverManager.GameOver -= LoadMainMenu;

        // Inject dependencies using Zenject
        [Inject]
        public void Setup(ISaveGame saveGame, CurrentScore currentScore, IPlayer player, IGameManager gameManager, IGridBocks gridBlocks)
        {
            _saveGame = saveGame; // Assign injected save game interface
            _currentScore = currentScore; // Assign injected current score
            _player = player; // Assign injected player interface
            _gameManager = gameManager; // Assign injected game manager
            _gridBocks = gridBlocks; // Assign injected grid blocks
        }

        private void Awake()
        {
            // Add listener for button clicks
            ButtonBack.onClick.AddListener(Return);
            ButtonSave.onClick.AddListener(SaveGame);
            ButtonQuit.onClick.AddListener(LoadMainMenu);
        }

        private void Update()
        {
            // Check for Escape key to pause the game
            if (Input.GetKey(KeyCode.Escape))
            {
                Pause();
            }
        }

        // Method to save the game
        private void SaveGame()
        {
            _saveGame.SaveFile(_currentScore, _player, _gameManager, _gridBocks); // Call the save method
            Debug.Log("Game saved successfully!"); // Log successful save
        }

        // Load the main menu scene
        private void LoadMainMenu()
        {
            Return(); // Ensure menu is closed before loading the main menu
            SceneManager.LoadScene("Menu", LoadSceneMode.Single); // Load the main menu scene
            Debug.Log("Loading main menu..."); // Log scene change
        }

        // Method to pause the game and show the menu
        private void Pause()
        {
            GameMenuPanel.SetActive(true); // Show the game menu
            Time.timeScale = 0; // Pause the game
            Debug.Log("Game paused."); // Log the pause action
        }

        // Method to return to the game (close the menu)
        private void Return()
        {
            GameMenuPanel.SetActive(false); // Hide the game menu
            Time.timeScale = 1; // Resume the game
            Debug.Log("Returning to the game."); // Log returning to the game
        }
    }
}