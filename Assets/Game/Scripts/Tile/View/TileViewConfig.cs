using System;
using UnityEngine;

namespace Game.Scripts.Tile
{
    [CreateAssetMenu(fileName = "TileViewConfig", menuName = "Tile/TileViewConfig", order = 0)]
    public sealed class TileViewConfig : ScriptableObject
    {
        [Tooltip("Tile levels")]
        [SerializeField] private float[] tileLevelHeights = { 0f, 0.5f, 1.0f, 1.5f, 2f };
        
        public float GetTileHeightForLevel(int level)
        {
            if (level < 0 || level >= tileLevelHeights.Length)
                throw new ArgumentOutOfRangeException(nameof(level), "Level index out of range");
            
            return tileLevelHeights[level];
        }
    }
}