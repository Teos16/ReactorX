#if UNITY_EDITOR
namespace Game.Scripts.Tile.Editor
{
    // Статический класс для уведомления об изменениях кластера
    public static class TileClusterChangeNotifier
    {
        public static System.Action OnClusterChanged;

        public static void NotifyClusterChanged()
        {
            OnClusterChanged?.Invoke();
        }
    }
}
#endif