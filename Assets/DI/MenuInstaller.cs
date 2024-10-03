using Zenject;
using Arkanodia.Core;

public class MenuInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ISaveGame>().To<SaveGame>().AsSingle();
    }
}