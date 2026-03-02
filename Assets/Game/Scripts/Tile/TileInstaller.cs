using Zenject;

namespace Game.Scripts.Tile
{
    public class TileInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Tile>().AsSingle();
        }
    }
}