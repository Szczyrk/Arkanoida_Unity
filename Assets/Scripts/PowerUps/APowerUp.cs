using UnityEngine;
using Arkanodia.Players;

namespace Arkanodia.PowerUps
{
    // Abstract class representing a base Power-Up
    public abstract class APowerUp : ScriptableObject
    {
        // The color associated with the power-up, can be used for visual representation
        public Color color;

        // Abstract method to execute the power-up effect on the player
        public abstract void Execute(Player caller);
    }
}