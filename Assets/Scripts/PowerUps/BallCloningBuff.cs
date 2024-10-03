using UnityEngine;
using System.Collections;
using Arkanodia.Players;

namespace Arkanodia.PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/BallCloningBuff")] // Create a menu item for creating BallCloningBuff assets
    public class BallCloningBuff : APowerUp // Inherit from APowerUp
    {
        public int amount; // Number of balls to spawn
        public float delay; // Delay between each ball spawn
        public float startingY; // Vertical offset for ball spawning

        // Execute method is called when the power-up is collected by the player
        public override void Execute(Player caller)
        {
            if (caller == null) // Validate that the caller is not null
            {
                Debug.LogWarning("Caller is null. Cannot apply BallCloningBuff."); // Log a warning if caller is null
                return; // Exit the method
            }

            // Start the coroutine to spawn balls
            caller.StartCoroutine(SpawnBall(caller));
        }

        // Coroutine to spawn balls over time
        private IEnumerator SpawnBall(Player caller)
        {
            int times = 0; // Counter for spawned balls

            // Loop to spawn the specified number of balls
            while (amount > times)
            {
                Ball ball = caller.BallPool.Get(); // Get a new ball from the pool
                ball.transform.position = caller.Position + Vector3.up * startingY; // Set the position of the new ball
                times++; // Increment the counter

                Debug.Log($"Spawned ball {times} at position: {ball.transform.position}"); // Log the spawning event

                yield return new WaitForSeconds(delay); // Wait for the specified delay before spawning the next ball
            }
        }
    }
}