using UnityEngine;
using Zenject;

namespace Game.Scripts.Tile
{
    public sealed class TileView : MonoBehaviour
    {
        [SerializeField] private TileViewConfig _config;
        
        private Tile _tile;

        [Inject]
        public void Construct(Tile tile)
        {
            _tile = tile;
            _tile.OnLevelChanged += ChangeHeightPerLevel; //TODO bad practice to subscribe right in the constructor with DI
        }

        private void ChangeHeightPerLevel(int level)
        {
            float targetY = _config.GetTileHeightForLevel(level);
            Vector3 start = transform.position;
            transform.position = new Vector3(start.x, targetY, start.z);
        }
    }
}