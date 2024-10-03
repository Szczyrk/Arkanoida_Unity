using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Arkanodia.Players;
using Arkanodia.MapGenerators;

namespace Arkanodia.Core
{
    public class SaveGame : ISaveGame
    {
        // Method to save game data to a file
        public void SaveFile(CurrentScore currentScore, IPlayer player, IGameManager gameManager, IGridBocks gridBlocks)
        {
            // Define the destination path for the save file
            string destination = Application.persistentDataPath + "/save.dat";
            FileStream file;

            // Open the file for writing; create if it doesn't exist
            if (File.Exists(destination))
            {
                file = File.OpenWrite(destination); // Open existing file for writing
                Debug.Log("Opened existing save file for writing."); // Log that we are opening an existing file
            }
            else
            {
                file = File.Create(destination); // Create a new file
                Debug.Log("Created a new save file."); // Log that a new file is created
            }

            // Create a GameData object to hold the current game state
            GameData data = new GameData(currentScore.Score, gridBlocks.CurrentMap, gameManager.Level, player.Life);

            // Serialize the game data into the file
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, data);
            file.Close(); // Close the file stream
            Debug.Log("Game data saved successfully."); // Log successful save
        }

        // Method to attempt loading game data from a file
        public bool TryLoadFile(out GameData gameData)
        {
            // Define the destination path for the save file
            string destination = Application.persistentDataPath + "/save.dat";
            FileStream file;

            // Check if the save file exists
            if (File.Exists(destination))
            {
                file = File.OpenRead(destination); // Open the file for reading
                Debug.Log("Save file found. Loading data..."); // Log that the save file is found
            }
            else
            {
                Debug.LogWarning("Save file not found."); // Log a warning if the file doesn't exist
                gameData = null; // Set output parameter to null
                return false; // Return false to indicate loading failed
            }

            // Deserialize the game data from the file
            BinaryFormatter bf = new BinaryFormatter();
            gameData = (GameData)bf.Deserialize(file);
            file.Close(); // Close the file stream
            Debug.Log("Game data loaded successfully."); // Log successful loading
            return true; // Return true to indicate loading was successful
        }
    }
}