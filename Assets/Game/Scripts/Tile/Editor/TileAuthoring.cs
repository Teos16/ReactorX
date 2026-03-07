using Sirenix.OdinInspector;
using UnityEngine;
using Game.Scripts.Players;
using Game.Scripts.Player.Editor;
using Game.Scripts.Tile.Editor;
using UnityEditor;

namespace Game.Scripts.Tiles.Editor
{
    [RequireComponent(typeof(TileViewEditor))]
    public sealed class TileAuthoring : SerializedMonoBehaviour
    {
        [FoldoutGroup("⚙️ Configuration", expanded: true)]
        [PropertyOrder(-2)]
        [SerializeField, Required]
        private TileConfig tileConfig;

        [FoldoutGroup("⚙️ Configuration")]
        [PropertyOrder(-1)]
        [SerializeField]
        private PlayerServiceEditor playerService;

        [FoldoutGroup("👤 Owner")]
        [PropertyOrder(0)]
        [SerializeField, OnValueChanged(nameof(ApplyOwner))]
        private PlayerConfig playerConfig;

        // --- Level Control теперь выше Cluster ---
        [FoldoutGroup("📊 Level Control", Expanded = true)]
        [PropertyOrder(1)]
        [OnInspectorGUI]
        private void DrawLevelControl()
        {
            if (tileConfig == null)
            {
                EditorGUILayout.HelpBox("Assign a TileConfig to configure levels.", MessageType.Info);
                return;
            }

            int min = MinTileLevel;
            int max = MaxTileLevel;
            int buttonCount = max - min + 1;
            int buttonsPerRow = 5;
            int rows = Mathf.CeilToInt((float)buttonCount / buttonsPerRow);

            // Сетка кнопок уровней
            for (int row = 0; row < rows; row++)
            {
                GUILayout.BeginHorizontal();
                for (int col = 0; col < buttonsPerRow; col++)
                {
                    int level = min + row * buttonsPerRow + col;
                    if (level > max) break;

                    bool isCurrent = currentLevel == level;
                    GUI.backgroundColor = isCurrent ? new Color(0.3f, 0.8f, 0.3f) : new Color(0.4f, 0.4f, 0.4f);

                    if (GUILayout.Button($"Lvl {level}", GUILayout.Width(50), GUILayout.Height(28)))
                    {
                        SetLevel(level);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(4);
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(12);

            // Кнопки увеличения/уменьшения с фиксированной шириной
            GUILayout.BeginHorizontal();

            bool canIncrease = currentLevel < MaxTileLevel;
            GUI.backgroundColor = canIncrease ? new Color(0.5f, 1f, 0.5f) : Color.gray;
            EditorGUI.BeginDisabledGroup(!canIncrease);
            if (GUILayout.Button("▲ Level Up", GUILayout.Width(120), GUILayout.Height(30)))
                SetLevel(currentLevel + 1);
            EditorGUI.EndDisabledGroup();

            bool canDecrease = currentLevel > MinTileLevel;
            GUI.backgroundColor = canDecrease ? new Color(1f, 0.5f, 0.5f) : Color.gray;
            EditorGUI.BeginDisabledGroup(!canDecrease);
            if (GUILayout.Button("▼ Level Down", GUILayout.Width(120), GUILayout.Height(30)))
                SetLevel(currentLevel - 1);
            EditorGUI.EndDisabledGroup();

            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            // Предупреждение, если уровень вышел за пределы
            if (!IsLevelValid)
            {
                EditorGUILayout.HelpBox($"⚠️ Level {currentLevel} is out of range [{MinTileLevel}, {MaxTileLevel}].", MessageType.Error);
                if (GUILayout.Button("Fix Level", GUILayout.Height(25)))
                {
                    SetLevel(Mathf.Clamp(currentLevel, MinTileLevel, MaxTileLevel));
                }
            }
        }

        [FoldoutGroup("🗂️ Cluster")]
        [PropertyOrder(2)]
        [OnValueChanged(nameof(NotifyClusterChanged))]
        public int cluster;

        [FoldoutGroup("🎨 View", Expanded = false)]
        [PropertyOrder(3)]
        [SerializeField]
        private TileViewEditor _view;

        [HideInInspector]
        public int currentLevel;

        private int MinTileLevel => tileConfig ? tileConfig.MinTileLevel : 0;
        private int MaxTileLevel => tileConfig ? tileConfig.MaxTileLevel : 10;
        private bool IsLevelValid => tileConfig && currentLevel >= MinTileLevel && currentLevel <= MaxTileLevel;

        [OnInspectorInit]
        private void Initialize()
        {
            _view = GetComponent<TileViewEditor>();
            ApplyAll();
        }

        private void ApplyOwner() => _view?.SetColor(playerConfig);
        private void NotifyClusterChanged() => TileClusterChangeNotifier.NotifyClusterChanged();

        private void ApplyAll()
        {
            if (this == null || tileConfig == null || _view == null) return;
            int clamped = Mathf.Clamp(currentLevel, MinTileLevel, MaxTileLevel);
            if (currentLevel != clamped) currentLevel = clamped;
            _view.SetHeight(currentLevel);
            _view.SetColor(playerConfig);
        }

        public void SetLevel(int level)
        {
            currentLevel = Mathf.Clamp(level, MinTileLevel, MaxTileLevel);
            ApplyAll();
        }
    }
}