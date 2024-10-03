using Arkanodia.ObjectPools;
using System;
using UnityEngine;
using Zenject;

namespace Arkanodia.Players
{
    [RequireComponent(typeof(Rigidbody2D))] // Ensure the GameObject has a Rigidbody2D for physics interactions
    public class Player : MonoBehaviour, IPlayer
    {
        private Camera _mainCamera; // Reference to the main camera for movement controls
        
        public IObjectPool<Ball> BallPool; // Reference to the ball object pool
        public static event Action ChangedLife; // Event that is triggered when the player's life changes

        // Property to track the player's life
        public int Life { get; set; }

        // Property to get or set the player's position
        public Vector3 Position
        {
            get => transform.position; // Get the current position of the player
            set => transform.position = value; // Set the player's position
        }

        public float speed = 1; // Speed of the player

        // Method to set up dependencies using Zenject
        [Inject]
        public void Setup(IObjectPool<Ball> ballPool)
        {
            BallPool = ballPool; // Assign the injected ball pool to the BallPool property
        }

        private void Awake()
        {
            _mainCamera = Camera.main; // Get a reference to the main camera
            ChangeLife(2); // Initialize the player's life to 2
        }

        // Method to change the player's life
        public void ChangeLife(int health)
        {
            Life += health; // Adjust the life by the given amount
            ChangedLife?.Invoke(); // Invoke the ChangedLife event if there are any subscribers
        }

        void Update()
        {
            // Get the camera's transform for relative movement
            Transform camTransform = _mainCamera.transform;

            // Calculate the horizontal movement based on player input
            Vector3 horizontalMovement = camTransform.right * Input.GetAxis("Horizontal");

            // Clamp the magnitude of the movement vector to a maximum length of 1
            Vector3 movement = Vector3.ClampMagnitude(horizontalMovement, 1);

            // Move the player in the calculated direction at the specified speed
            transform.Translate(movement * (speed * Time.deltaTime), Space.World);
        }
    }
}