using UnityEngine;
using Arkanodia.PowerUps;

[CreateAssetMenu(menuName = "Block Type")] // Allows creating BlockType assets from the Unity Editor
public class BlockType : ScriptableObject
{
    public int Life; // The life of the block, determines how many hits it can take
    public Color Color; // The color of the block for visual representation
    public bool isIndestructible; // Indicates if the block cannot be destroyed
    public APowerUp powerUpType; // The type of power-up that may spawn when this block is destroyed

    private void OnValidate() // Called when the scriptable object is modified in the inspector
    {
        // Log when a BlockType asset is validated or changed
        Debug.Log($"BlockType '{name}' validated. Life: {Life}, Color: {Color}, Indestructible: {isIndestructible}, PowerUp: {powerUpType?.name}");
    }
}