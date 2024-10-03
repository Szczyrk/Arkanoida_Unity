namespace Arkanodia.Core
{
    public interface IGameManager
    {
        public int Level { get; set; }
        public void LoadGame(GameData gameData);
    }
}