using System;
using UnityEngine;

namespace Game.Scripts.Tile
{
    public sealed class Tile : Entity
    {
        public event Action<int> OnLevelChanged;
        
        private TileConfig _config;

        private int _level;

        public void SetConfig(TileConfig config) => _config = config;
        
        public void SetLevel(int level)
        {
            _level = Mathf.Clamp(level, _config.MinTileLevel, _config.MaxTileLevel);
            if (_level == level)
            {
                OnLevelChanged?.Invoke(_level);
                Debug.Log($"Tile level changed to {_level}");
            }
        }
    }
}