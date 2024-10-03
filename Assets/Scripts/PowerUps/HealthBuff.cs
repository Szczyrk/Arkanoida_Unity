using UnityEngine;
using Arkanodia.Players;

namespace Arkanodia.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/HealthBuff")] // Create a menu item for creating HealthBuff assets
    public class HealthBuff : APowerUp // Inherit from APowerUp
    {
        public int amount; // Amount of health to add

        // Execute method is called when the power-up is collected by the player
        public override void Execute(Player caller)
        {
            if (caller == null) // Validate that the caller is not null
            {
                Debug.LogWarning("Caller is null. Cannot apply HealthBuff."); // Log a warning if caller is null
                return; // Exit the method
            }

            // Call the ChangeLife method on the Player object to increase health
            caller.ChangeLife(amount);
            Debug.Log($"HealthBuff executed. Player health increased by {amount}. New health: {caller.Life}"); // Log the health increase
        }
    }
}