using Game.Scripts.Player.Editor;
using Game.Scripts.Players;
using Game.Scripts.Tile.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Tiles.Editor
{
    [ExecuteAlways]
    [RequireComponent(typeof(TileViewEditor))]
    public sealed class TileAuthoring : MonoBehaviour
    {
        public int currentLevel;
        public TileConfig tileConfig;
        public PlayerConfig playerConfig;
        public PlayerServiceEditor playerService;

        private TileViewEditor _view;

        private void OnEnable()
        {
            Undo.undoRedoPerformed += Apply;
            EditorApplication.delayCall += Apply;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= Apply;
        }

        private void Apply()
        {
            if (this == null || tileConfig == null) return;
            if (_view == null) _view = GetComponent<TileViewEditor>();
            int clamped = Mathf.Clamp(currentLevel, tileConfig.MinTileLevel, tileConfig.MaxTileLevel);
            _view?.SetHeight(clamped);
            _view?.SetColor(playerConfig);
        }

        public void SetLevel(int level)
        {
            currentLevel = Mathf.Clamp(level, tileConfig.MinTileLevel, tileConfig.MaxTileLevel);
            Apply();
        }

        public void SetOwnerFromConfig()
        {
            if (_view == null) _view = GetComponent<TileViewEditor>();
            _view?.SetColor(playerConfig);
        }

        public void ApplyTileConfig() => Apply();

        public void Reset() => Apply();
    }
}