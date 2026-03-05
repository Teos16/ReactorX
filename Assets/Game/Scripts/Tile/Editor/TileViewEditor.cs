using Game.Scripts.Players;
using Game.Scripts.Tiles;
using UnityEngine;

namespace Game.Scripts.Tile.Editor
{
    [ExecuteAlways]
    public sealed class TileViewEditor : MonoBehaviour
    {
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");

        [SerializeField] private TileViewConfig _config;
        [SerializeField] private MeshRenderer _meshRenderer;

        private MaterialPropertyBlock _propertyBlock;

        public void SetHeight(int level)
        {
            if (_config == null) return;
            float targetY = _config.GetTileHeightForLevel(level);
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, targetY, pos.z);
        }

        public void SetColor(PlayerConfig playerConfig)
        {
            if (playerConfig == null || _meshRenderer == null) return;
            if (_propertyBlock == null) _propertyBlock = new MaterialPropertyBlock();
            _meshRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(ColorProperty, playerConfig.MainColor);
            _propertyBlock.SetColor(BaseColorProperty, playerConfig.MainColor);
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}