using UnityEngine;

namespace Game.Scripts.Tiles
{
    public sealed class TileView : MonoBehaviour
    {
        private const string COLOR_PROPERTY = "_Color";
        private const string BASE_COLOR_PROPERTY = "_BaseColor";

        [SerializeField] private Tile _tile;
        [SerializeField] private TileViewConfig _config;
        [SerializeField] private MeshRenderer _meshRenderer;

        private MaterialPropertyBlock _propertyBlock;

        private readonly int _colorProperty = Shader.PropertyToID(COLOR_PROPERTY);

        private readonly int _baseColorProperty = Shader.PropertyToID(BASE_COLOR_PROPERTY); // URP

        private void OnEnable()
        {
            _tile.OnLevelChanged += SetHeight;
            _tile.OnOwnerChanged += SetColor;
        }

        private void OnDisable()
        {
            if (_tile != null)
            {
                _tile.OnLevelChanged -= SetHeight;
                _tile.OnOwnerChanged -= SetColor;
            }
        }
        
        private void OnDestroy()
        {
            if (_tile != null)
            {
                _tile.OnLevelChanged -= SetHeight;
                _tile.OnOwnerChanged -= SetColor;
            }
        }

        private void SetHeight(int level)
        {
            float targetY = _config.GetTileHeightForLevel(level);
            Vector3 start = transform.position;
            transform.position = new Vector3(start.x, targetY, start.z);
        }
        
        private void SetColor(Players.Player player)
        {
            if(_propertyBlock == null)
                _propertyBlock = new MaterialPropertyBlock();
            
            _meshRenderer.GetPropertyBlock(_propertyBlock);

            _propertyBlock.SetColor(_colorProperty, player.MainColor);
            _propertyBlock.SetColor(_baseColorProperty, player.MainColor);

            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}