using UnityEngine;

namespace Game.Scripts.Tile
{
    [ExecuteAlways]
    [RequireComponent(typeof(TileView))]
    public sealed class TilePresenter : MonoBehaviour
    {
        public int currentLevel;
        public TileConfig config;

        private Tile _tile;
        private TileView _view;

        public Tile Tile => _tile;

        private void OnEnable() => Initialize();

        private void Initialize()
        {
            if (config == null) return;

            _tile = new Tile();
            _tile.SetConfig(config);
            _tile.SetLevel(0);

            _view = GetComponent<TileView>();
            _view.Construct(_tile);
        }

        public void SetLevel(int level)
        {
            if (_tile == null) Initialize();
            _tile?.SetLevel(level);
            currentLevel = level;
        }

        public void Reset() => Initialize();
    }
}