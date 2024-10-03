using Arkanodia.ObjectPools;
using Arkanodia.MapGenerators;
using Arkanodia.PowerUps;
using Arkanodia.Players;
using Arkanodia.Core;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public Block BlockPrefab;
    public Ball BallPrefab;
    public PowerUp PowerUpPrefab;
    public Player Player;
    public CurrentScore currentScore;
    public GameManager GameManager;
    public GridBlocks GridBlocks;
    
    public override void InstallBindings()
    {
        Container.Bind<IObjectPool<Block>>().To<BlockPool>().AsSingle();
        Container.Bind<IObjectPool<Ball>>().To<BallPool>().AsSingle();
        Container.Bind<IObjectPool<PowerUp>>().To<PowerUpPool>().AsSingle();
        Container.Bind<ISaveGame>().To<SaveGame>().AsSingle();
        Container.Bind<IGameManager>().FromInstance(GameManager).AsSingle();
        Container.Bind<IPlayer>().FromInstance(Player).AsSingle();
        Container.Bind<IGridBocks>().FromInstance(GridBlocks).AsSingle();
        Container.Bind<CurrentScore>().FromInstance(currentScore).AsSingle();
        
        Container.BindFactory<Block, BlockPool.Factory>().FromComponentInNewPrefab(BlockPrefab);
        Container.BindFactory<Ball, BallPool.Factory>().FromComponentInNewPrefab(BallPrefab);
        Container.BindFactory<PowerUp, PowerUpPool.Factory>().FromComponentInNewPrefab(PowerUpPrefab);
    }
}