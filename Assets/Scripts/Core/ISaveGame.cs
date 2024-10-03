using Arkanodia.Players;
using Arkanodia.MapGenerators;

namespace Arkanodia.Core
{
    public interface ISaveGame
    {
        bool TryLoadFile(out GameData gameData);
        void SaveFile(CurrentScore currentScore, IPlayer player, IGameManager gameManager, IGridBocks gridBlocks);
    }
}