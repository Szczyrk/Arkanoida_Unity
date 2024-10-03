using Arkanodia.ObjectPools;
using Arkanodia.PowerUps;
using Arkanodia.MapGenerators;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SpriteRenderer))] // Ensure a SpriteRenderer component is attached to this GameObject
public class Block : MonoBehaviour
{
    private IObjectPool<Block> _blockPool; // Object pool for blocks
    private IObjectPool<PowerUp> _powerUpPool; // Object pool for power-ups
    private CurrentScore _currentScore; // Reference to the current score
    private int _currentLife; // Current life of the block
    private SpriteRenderer _spriteRenderer; // Reference to the SpriteRenderer component

    public BlockType BlockType; // Type of the block (includes properties like color and life)
    public Vector2Int gridPos; // Position of the block in the grid
    private IGridBocks _gridBlocks; // Reference to the grid blocks

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    // Method to change the block type and its appearance
    public void ChangeType(BlockType blockType)
    {
        BlockType = blockType; // Set the block type
        _spriteRenderer.color = BlockType.Color; // Change color based on block type
        _currentLife = BlockType.Life; // Set current life based on block type
    }

    // Method for injecting dependencies via Zenject
    [Inject]
    public void Setup(IObjectPool<Block> blockPool, IObjectPool<PowerUp> powerPool, CurrentScore currentScore,
        IGridBocks gridBocks)
    {
        _blockPool = blockPool; // Initialize block pool
        _powerUpPool = powerPool; // Initialize power-up pool
        _currentScore = currentScore; // Initialize current score
        _gridBlocks = gridBocks; // Initialize grid blocks
    }

    // Method called when the block collides with another object
    void OnCollisionEnter2D(Collision2D other)
    {
        // If the block is indestructible, ignore the collision
        if (BlockType.isIndestructible)
        {
            Debug.Log($"Block at {gridPos} is indestructible. Ignoring collision."); // Log indestructible block collision
            return;
        }

        // Decrease current life or destroy the block if life reaches zero
        if (_currentLife > 1)
        {
            _currentLife--; // Decrease life by 1
            Debug.Log($"Block at {gridPos} life decreased. Current life: {_currentLife}"); // Log current life
        }
        else
        {
            Debug.Log($"Destroying block at {gridPos}"); // Log block destruction
            _blockPool.Return(this); // Return the block to the pool
            _gridBlocks.CurrentMap[gridPos.x, gridPos.y] = 0; // Update the current map
            _currentScore.AddScore(BlockType.Life); // Add score based on block's life

            // If the block has a power-up associated with it, create the power-up
            if (BlockType.powerUpType != null)
            {
                PowerUp powerUp = _powerUpPool.Get(); // Get a power-up from the pool
                powerUp.ChangeType(BlockType.powerUpType); // Change its type
                powerUp.transform.position = transform.position; // Set position to block's position
                Debug.Log($"Power-up spawned at {transform.position} for block at {gridPos}."); // Log power-up spawning
            }
        }
    }
}