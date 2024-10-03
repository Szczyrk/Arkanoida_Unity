using System;
using System.Linq;
using Arkanodia.ObjectPools;
using Arkanodia.Players;
using Arkanodia.PowerUps;
using UnityEngine;
using Zenject;

namespace Arkanodia.Core
{
    public class GameOverManager : MonoBehaviour
    {
        // Object pool for Ball objects
        private IObjectPool<Ball> _ballPool;

        // Object pool for PowerUp objects
        private IObjectPool<PowerUp> _powerUp;

        // Reference to the player object
        private IPlayer _player;

        // Event triggered when the game is over
        public static event Action GameOver;

        // Dependency injection for setting up the ball pool, power-up pool, and player reference
        [Inject]
        public void Setup(IObjectPool<Ball> ballPool, IObjectPool<PowerUp> powerUp, IPlayer player)
        {
            _ballPool = ballPool;
            _powerUp = powerUp;
            _player = player;
        }

        // Called when the script instance is being loaded
        private void Awake()
        {
            // Add all active Ball objects in the scene to the object pool (using their "Ball" tag)
            _ballPool.ObjectInGame.AddRange(
                GameObject.FindGameObjectsWithTag("Ball")
                .Select(g => g.GetComponent<Ball>())
            );

            // Log the number of active balls found in the scene
            Debug.Log($"Initialized with {_ballPool.ObjectInGame.Count} balls in the pool.");
        }

        // Triggered when an object enters the 2D collider
        private void OnTriggerEnter2D(Collider2D other)
        {
            // If the object that collided is tagged as a "Ball"
            if (other.transform.CompareTag("Ball"))
            {
                Ball ball = other.gameObject.GetComponent<Ball>();

                // Return the ball to the object pool
                _ballPool.Return(ball);

                // Log that a ball has been returned
                Debug.Log("Ball returned to the pool.");

                // Check if all balls have been returned (i.e., no balls in the game)
                if (_ballPool.ObjectInGame.Count == 0)
                {
                    // If the player has lives remaining, reduce life and respawn a new ball
                    if (_player.Life > 0)
                    {
                        _player.ChangeLife(-1);

                        // Log life deduction
                        Debug.Log($"Player lost a life. Remaining lives: {_player.Life}");

                        Ball newBall = _ballPool.Get();
                        newBall.transform.position = _player.Position;

                        // Log that a new ball has been spawned at the player's position
                        Debug.Log("New ball spawned at the player's position.");
                        return;
                    }

                    // If no lives are left, trigger the Game Over event
                    GameOver?.Invoke();
                    Debug.Log("Game Over! No lives remaining.");
                }
            }

            // If the object that collided is tagged as a "PowerUp"
            if (other.transform.CompareTag("PowerUp"))
            {
                // Return the power-up to the object pool
                _powerUp.Return(other.gameObject.GetComponent<PowerUp>());

                // Log that the power-up has been returned
                Debug.Log("PowerUp returned to the pool.");
            }
        }
    }
}