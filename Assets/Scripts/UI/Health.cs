using TMPro;
using UnityEngine;
using Zenject;
using Arkanodia.Players;

namespace Arkanodia.UI
{
    public class Health : MonoBehaviour
    {
        private IPlayer _player; // Reference to the player interface

        public TextMeshProUGUI textHealth; // TextMeshPro component to display health

        // Subscribe to the ChangedLife event when this component is enabled
        private void OnEnable() => Player.ChangedLife += UpdateHealth;

        // Unsubscribe from the ChangedLife event when this component is disabled
        private void OnDisable() => Player.ChangedLife -= UpdateHealth;

        // Inject the IPlayer dependency
        [Inject]
        public void Setup(IPlayer player)
        {
            _player = player; // Assign the injected player to the local variable
        }

        // Update the health UI whenever the player's life changes
        void UpdateHealth()
        {
            // Log the current health for debugging purposes
            Debug.Log($"Updating health display: {_player.Life} lives remaining.");
            
            // Update the health text to reflect the player's current life
            textHealth.text = $"Life\n{_player.Life}";
        }
    }
}