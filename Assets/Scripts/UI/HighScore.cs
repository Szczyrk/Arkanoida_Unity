using TMPro;
using UnityEngine;
using Zenject;
using Arkanodia.Core;

namespace Arkanodia.UI
{
    public class HighScore : MonoBehaviour
    {
        public TextMeshProUGUI TextScore; // Reference to the TextMeshProUGUI component for displaying the score
        private CurrentScore _currentScore; // Reference to the current score manager
        private string keyHighScore = "HighScore"; // Key for storing high score in PlayerPrefs
        private int _highScore; // Variable to hold the high score

        // Inject the CurrentScore dependency
        [Inject]
        public void Setup(CurrentScore currentScore)
        {
            _currentScore = currentScore; // Assign the injected current score to the local variable
        }

        private void Awake()
        {
            // Retrieve the high score from PlayerPrefs or default to 0 if not set
            _highScore = PlayerPrefs.GetInt(keyHighScore, 0);
            // Update the UI text to display the current high score
            TextScore.text = $"Score {_highScore}";
            Debug.Log($"High Score loaded: {_highScore}"); // Log the loaded high score for debugging
        }

        // Subscribe to the GameOver event when this component is enabled
        private void OnEnable() => GameOverManager.GameOver += UpdateScore;

        // Unsubscribe from the GameOver event when this component is disabled
        private void OnDisable() => GameOverManager.GameOver -= UpdateScore;

        // Update the high score when the game is over
        private void UpdateScore()
        {
            // Check if the current score exceeds the high score
            if (_currentScore.Score > _highScore)
            {
                // Update the high score in PlayerPrefs
                PlayerPrefs.SetInt(keyHighScore, _currentScore.Score);
                PlayerPrefs.Save(); // Save the updated PlayerPrefs
                Debug.Log($"New High Score: {_currentScore.Score}"); // Log the new high score for debugging
            }
        }
    }
}