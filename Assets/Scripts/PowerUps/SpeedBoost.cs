using UnityEngine;
using Arkanodia.Players;
using System.Collections;

namespace Arkanodia.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/SpeedBoost")] // Create a menu item for creating SpeedBoost assets
    public class SpeedBoost : APowerUp
    {
        public float speedIncrease; // Amount of speed to increase
        public float duration; // Duration of the speed boost

        // Execute method to apply the speed boost effect to the player
        public override void Execute(Player caller)
        {
            if (caller == null) // Validate that the caller is not null
            {
                Debug.LogWarning("Caller is null. Cannot apply SpeedBoost."); // Log a warning if caller is null
                return; // Exit the method
            }

            caller.speed += speedIncrease; // Increase the player's speed
            Debug.Log($"Speed increased by {speedIncrease} for {duration} seconds.");

            // Optionally, you could also set a timer to reset the speed after the duration
            caller.StartCoroutine(ResetSpeedAfterDuration(caller, duration));
        }

        // Coroutine to reset the player's speed after the duration
        private IEnumerator ResetSpeedAfterDuration(Player caller, float duration)
        {
            yield return new WaitForSeconds(duration); // Wait for the duration
            caller.speed -= speedIncrease; // Reset the player's speed
            Debug.Log($"Speed reset by {speedIncrease}.");
        }
    }
}