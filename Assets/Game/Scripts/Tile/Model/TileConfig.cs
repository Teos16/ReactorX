using UnityEngine;

namespace Game.Scripts.Tiles
{
    [CreateAssetMenu(fileName = "TileConfig", menuName = "Tile/TileConfig", order = 0)]
    public sealed class TileConfig : ScriptableObject
    {
        [Tooltip("Tile levels")] 
        [field : SerializeField] public int MinTileLevel { get; private set; }
        [field : SerializeField] public int MaxTileLevel { get; private set; }
    }
}