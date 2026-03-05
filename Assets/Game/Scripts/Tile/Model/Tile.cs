using System;
using UnityEngine;

namespace Game.Scripts.Tiles
{
    public sealed class Tile : MonoBehaviour
    {
        public event Action<int> OnLevelChanged;
        public event Action<Players.Player> OnOwnerChanged;

        private Players.Player _owner;
        private TileConfig _config;
        
        private int _level;

        public void SetConfig(TileConfig config) => _config = config;
        
        public void SetLevel(int level)
        {
            int newLevel = Mathf.Clamp(level, _config.MinTileLevel, _config.MaxTileLevel);
            if (_level == newLevel) return;
            _level = newLevel;
            OnLevelChanged?.Invoke(_level);
        }

        public void SetOwner(Players.Player owner)
        {
            if (_owner != owner)
            {
                _owner = owner;
                OnOwnerChanged?.Invoke(owner);
            }
        }
    }
}