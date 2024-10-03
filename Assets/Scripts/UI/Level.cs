using TMPro;
using UnityEngine;
using Zenject;
using Arkanodia.Core;

namespace Arkanodia.UI
{
    public class Level : MonoBehaviour
    {
        public TextMeshProUGUI Text; // Reference to the TextMeshProUGUI component for displaying the current level
        private IGameManager _gameManager; // Reference to the game manager interface

        // Inject the IGameManager dependency
        [Inject]
        public void Setup(IGameManager gameManager)
        {
            _gameManager = gameManager; // Assign the injected game manager to the local variable
        }

        // Subscribe to the ChangedLevel event when this component is enabled
        private void OnEnable() => GameManager.ChangedLevel += UpdateLevel;

        // Unsubscribe from the ChangedLevel event when this component is disabled
        private void OnDisable() => GameManager.ChangedLevel -= UpdateLevel;

        // Update the displayed level text when the level changes
        private void UpdateLevel()
        {
            Text.text = $"Level: {_gameManager.Level}"; // Update the UI text to reflect the current level
            Debug.Log($"Current Level Updated: {_gameManager.Level}"); // Log the current level for debugging
        }
    }
}