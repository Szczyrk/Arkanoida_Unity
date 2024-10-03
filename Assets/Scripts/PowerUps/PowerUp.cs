using Arkanodia.ObjectPools;
using Arkanodia.Players;
using UnityEngine;
using Zenject;

namespace Arkanodia.PowerUps
{
    [RequireComponent(typeof(SpriteRenderer))] // Ensure a SpriteRenderer component is attached
    public class PowerUp : MonoBehaviour
    {
        // Reference to the object pool for PowerUp instances
        private IObjectPool<PowerUp> _powerUpPool;
        
        // Type of the power-up, encapsulated in a separate class
        public APowerUp powerUpType;
        
        // Reference to the SpriteRenderer component
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            // Get the SpriteRenderer component attached to this GameObject
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Method to change the type of power-up and update its visual representation
        public void ChangeType(APowerUp powerUpType)
        {
            this.powerUpType = powerUpType;
            _spriteRenderer.color = this.powerUpType.color; // Change color based on power-up type
            Debug.Log($"PowerUp type changed to: {powerUpType.name} with color: {this.powerUpType.color}"); // Log the change
        }
        
        // Dependency injection setup for the object pool
        [Inject]
        public void Setup(IObjectPool<PowerUp> powerPool)
        {
            _powerUpPool = powerPool; // Assign the injected object pool
        }

        // Triggered when another collider enters the trigger collider attached to this GameObject
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.CompareTag("Player")) // Check if the collider belongs to a player
            {
                // Execute the effect of the power-up on the player
                powerUpType.Execute(other.GetComponent<Player>());
                
                // Return this power-up instance to the pool
                _powerUpPool.Return(this);
                
                // Log the power-up collection for debugging
                Debug.Log($"PowerUp collected by player: {powerUpType.name}"); 
            }
        }
    }
}