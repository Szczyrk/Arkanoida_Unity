using UnityEngine;

namespace Arkanodia.Players
{
    [RequireComponent(typeof(Rigidbody2D))] // Ensures that a Rigidbody2D component is always attached to the GameObject
    public class Ball : MonoBehaviour
    {
        public float speed; // Speed of the ball
        private Rigidbody2D _rigidbody2D; // Reference to the Rigidbody2D component for physics control
        
        private void Awake()
        {
            // Initialize the Rigidbody2D component when the ball is instantiated or enabled
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        // This method handles the ball's collision with other objects in the 2D environment
        private void OnCollisionEnter2D(Collision2D other)
        {
            // Check if the object collided with has the tag "Player" (e.g., the paddle or racket)
            if (other.transform.CompareTag("Player"))
            {
                // Calculate the hit factor, determining where the ball hit on the player's racket
                float x = hitFactor(transform.position,
                    other.transform.position,
                    other.collider.bounds.size.x);

                // Create a new direction vector based on the hit location. The Y component is always 1
                // to ensure the ball moves upwards, while the X component is determined by the hit factor.
                Vector2 dir = new Vector2(x, 1).normalized; // Normalize the vector to maintain consistent speed

                // Set the ball's velocity in the new direction, multiplied by its speed
                _rigidbody2D.velocity = dir * speed;
            }
        }

        // This method calculates how far along the racket the ball has hit. 
        // A value between -1 and 1 is returned, where -1 means the left edge and 1 means the right edge.
        float hitFactor(Vector2 ballPos, Vector2 racketPos, float racketWidth)
        {
            // Calculate the relative position of the ball on the player's racket
            // A value between -1 and 1 will be returned based on the ball's X position.
            return (ballPos.x - racketPos.x) / racketWidth;
        }
    }
}