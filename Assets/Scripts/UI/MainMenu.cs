using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using Arkanodia.Core;

namespace Arkanodia.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button ButtonNewGame; // Reference to the 'New Game' button
        [SerializeField] private Button ButtonContinue; // Reference to the 'Continue' button
        [SerializeField] private Button ButtonExit; // Reference to the 'Exit' button

        private ISaveGame _saveGame; // Interface for saving game data

        // Inject the ISaveGame dependency
        [Inject]
        public void Setup(ISaveGame saveGame)
        {
            _saveGame = saveGame; // Assign the injected save game interface
        }

        private void Awake()
        {
            // Set up the Exit button listener
            ButtonExit.onClick.AddListener(ExitGame);

            // Check if there's a saved game and set the continue button's interactable state
            ButtonContinue.interactable = _saveGame.TryLoadFile(out GameData gameData);
            Debug.Log($"Continue button interactable state: {ButtonContinue.interactable}"); // Log the state of the continue button

            // Set up the listeners for the New Game and Continue buttons
            ButtonNewGame.onClick.AddListener(() => LoadGame(null));
            ButtonContinue.onClick.AddListener(() => LoadGame(gameData));
        }

        // Exit the game or stop play mode in the editor
        private void ExitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the Unity editor
            #else
                Application.Quit(); // Quit the application in a build
            #endif
            Debug.Log("Exiting game."); // Log the exit action
        }

        // Load the game scene and manage game data
        private void LoadGame(GameData gameData)
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Single); // Load the game scene
            if (gameData != null)
            {
                GameLoader.Instance.gameData = gameData; // Set the loaded game data
                GameLoader.Instance.isSave = true; // Mark that the game is loaded from a save
                Debug.Log("Loading saved game."); // Log that a saved game is being loaded
            }
            else
            {
                GameLoader.Instance.isSave = false; // Mark that no save is being loaded
                Debug.Log("Starting new game."); // Log that a new game is starting
            }
        }
    }
}