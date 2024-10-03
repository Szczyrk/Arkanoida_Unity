using System.Collections.Generic;
using Arkanodia.ObjectPools;
using Arkanodia.Core;
using UnityEngine;
using Zenject;

namespace Arkanodia.MapGenerators
{
    public class GridBlocks : MonoBehaviour, IGridBocks // Class implementing the IGridBocks interface
    {
        public int[,] CurrentMap { get; set; } // Property to hold the current map data

        private IObjectPool<Block> _blockPool; // Object pool for Block instances
        private IGameManager _gameManager; // Reference to the game manager
        private MapGenerator _mapGenerator; // Reference to the map generator
        private Vector2 _edgePadding; // Padding to adjust the grid edges
        private Vector2 _blockSize; // Size of individual blocks
        private Block[,] _blockGrid; // 2D array to hold the blocks

        [SerializeField] private Vector2Int gridSize; // Size of the grid (width, height)
        [SerializeField] private Vector2 panelSize; // Size of the panel containing the grid
        [SerializeField] private Transform parentGrid; // Parent transform for organizing blocks in the hierarchy
        [SerializeField] private List<BlockType> blockTypes = new(); // List of different block types
        [SerializeField] private float spacing; // Spacing between blocks

        // Method for injecting dependencies via Zenject
        [Inject]
        public void Setup(IObjectPool<Block> blockPool, IGameManager gameManager)
        {
            _blockPool = blockPool; // Initialize block pool
            _gameManager = gameManager; // Initialize game manager
        }

        private void Awake()
        {
            Debug.Log("Awake: Initializing grid."); // Log message for initialization
            
            // Initialize the block grid with specified dimensions
            _blockGrid = new Block[gridSize.x, gridSize.y];
            float ratioScale = panelSize.x / gridSize.x; // Calculate width ratio
            float h = panelSize.y / gridSize.y; // Calculate height ratio
            _blockSize = new Vector2(ratioScale, h); // Set block size
            _edgePadding = _blockSize / 2; // Set edge padding

            // Check if there is saved game data to load
            if (GameLoader.Instance == null || !GameLoader.Instance.isSave)
            {
                Debug.Log("Creating new map since no save was found."); // Log if creating a new map
                CreateMap(null); // Create a new map
                return;
            }

            Debug.Log("Loading game data from save."); // Log for loading saved game data
            _gameManager.LoadGame(GameLoader.Instance.gameData); // Load the game data
        }

        // Method to create a new map or use an existing one
        public void CreateMap(int[,] map)
        {
            if (map == null) // Check if map is not provided
            {
                Debug.Log("Generating new map."); // Log for map generation
                _mapGenerator = new MapGenerator(gridSize.x, gridSize.y, blockTypes.Count + 1, 0); // Create a new map generator
                CurrentMap = _mapGenerator.GenerateMap(_gameManager.Level, 40); // Generate the map based on level
            }
            else
            {
                Debug.Log("Using existing map for grid."); // Log for using an existing map
                CurrentMap = map; // Use the provided map
            }

            GenerateGrid(CurrentMap); // Generate the grid based on the current map
        }

        // Method to remove existing blocks from the grid
        private void Remove()
        {
            Debug.Log("Removing existing blocks from the grid."); // Log for block removal
            foreach (var block in _blockGrid) // Iterate through all blocks
            {
                if (block != null) // Check if block is not null
                {
                    _blockPool.Return(block); // Return block to the pool
                }
            }
        }

        // Method to generate the grid based on map data
        private void GenerateGrid(int[,] borderedMap)
        {
            Debug.Log("Generating grid based on map data."); // Log for grid generation
            if (_blockGrid[0, 0] != null) // Check if there are existing blocks
            {
                Remove(); // Remove existing blocks
            }

            // Loop through grid size and place blocks accordingly
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (borderedMap[x, y] != 0) // Check if the current map cell is not empty
                    {
                        Debug.Log($"Placing block at position [{x},{y}]."); // Log the position of the block being placed
                        _blockGrid[x, y] = _blockPool.Get(); // Get a block from the pool
                        _blockGrid[x, y].ChangeType(blockTypes[borderedMap[x, y] - 1]); // Change the block type
                        _blockGrid[x, y].transform.SetParent(parentGrid); // Set parent for organization
                        _blockGrid[x, y].transform.name = $"block[{x},{y}]"; // Name the block for identification
                        Transform transform = _blockGrid[x, y].transform; // Reference to the block's transform
                        transform.localPosition =
                            new Vector3(x * _blockSize.x + _edgePadding.x, y * _blockSize.y + _edgePadding.y, 0); // Set local position based on grid coordinates
                        transform.localScale = new Vector3(_blockSize.x - spacing, _blockSize.y - spacing, 1); // Set scale while considering spacing
                        _blockGrid[x, y].gridPos = new Vector2Int(x, y); // Store the grid position in the block
                    }
                }
            }
        }

        // Method to visualize the grid in the editor using Gizmos
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red; // Set color for Gizmos
            Gizmos.DrawCube(transform.position + (Vector3) panelSize / 2, new Vector3(panelSize.x, panelSize.y, 1)); // Draw a cube representing the grid area
        }
    }
}