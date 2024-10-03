using TMPro;
using UnityEngine;
using Zenject;

namespace Arkanodia.UI
{
    public class Score : MonoBehaviour
    {
        public TextMeshProUGUI TextScore; // Reference to the TextMeshProUGUI component for displaying the score
        private CurrentScore _currentScore; // Reference to the CurrentScore instance to access score data
        
        // Inject the CurrentScore dependency
        [Inject]
        public void Setup(CurrentScore currentScore)
        {
            _currentScore = currentScore; // Assign the injected CurrentScore instance
        }

        // Subscribe to the GetPoint event when the object is enabled
        private void OnEnable() => CurrentScore.GetPoint += UpdateScore;

        // Unsubscribe from the GetPoint event when the object is disabled
        private void OnDisable() => CurrentScore.GetPoint -= UpdateScore;

        // Update the score displayed on the UI
        private void UpdateScore()
        {
            TextScore.text = $"Score {_currentScore.Score}"; // Update the text with the current score
            Debug.Log($"Score updated: {_currentScore.Score}"); // Log the updated score for debugging purposes
        }
    }
}