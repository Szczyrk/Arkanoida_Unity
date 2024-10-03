using System;
using UnityEngine;

public class CurrentScore : MonoBehaviour
{
    // Property to hold the player's current score
    public int Score { get; set; }
    
    // Event that is triggered whenever the score is updated
    public static event Action GetPoint;

    // Method to add points to the current score
    public void AddScore(int score)
    {
        // Log the score being added for debugging purposes
        Debug.Log($"Adding {score} points to current score. Previous score: {Score}");
        
        Score += score; // Update the score
        
        // Log the new score after adding points
        Debug.Log($"New score: {Score}");
        
        // Invoke the GetPoint event to notify other systems of the score change
        GetPoint?.Invoke();
    }
}