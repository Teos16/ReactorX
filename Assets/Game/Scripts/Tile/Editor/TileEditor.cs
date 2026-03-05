using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Tiles.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TileAuthoring))]
    public class TileEditor : UnityEditor.Editor
    {
        private TileAuthoring _authoring;
        private TileConfig _config;
        private SerializedProperty _configProperty;
        private SerializedProperty _currentLevelProperty;
        private SerializedProperty _playerConfigProperty;
        private SerializedProperty _playerServiceProperty;

        private void OnEnable()
        {
            _configProperty = serializedObject.FindProperty(nameof(TileAuthoring.tileConfig));
            _currentLevelProperty = serializedObject.FindProperty(nameof(TileAuthoring.currentLevel));
            _playerConfigProperty = serializedObject.FindProperty(nameof(TileAuthoring.playerConfig));
            _playerServiceProperty = serializedObject.FindProperty(nameof(TileAuthoring.playerService));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            RefreshTargetData();

            DrawTileConfigSection();
            DrawPlayerServiceField();

            if (!DrawConfigWarningIfMissing())
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            DrawPlayerConfigSection();
            DrawLevelControlsSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void RefreshTargetData()
        {
            _authoring = (TileAuthoring)target;
            _config = _configProperty.objectReferenceValue as TileConfig;
        }

        // ------------------------------------------------------------
        // Tile Config Section
        // ------------------------------------------------------------
        private void DrawTileConfigSection()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_configProperty);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                foreach (TileAuthoring t in targets)
                {
                    Undo.RecordObject(t, "Set Tile Config");
                    t.ApplyTileConfig();
                    EditorUtility.SetDirty(t);
                }
            }
        }

        private void DrawPlayerServiceField()
        {
            EditorGUILayout.PropertyField(_playerServiceProperty);
        }

        // ------------------------------------------------------------
        // Owner Assignment Section (Player Config)
        // ------------------------------------------------------------
        private void DrawPlayerConfigSection()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Owner", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_playerConfigProperty, new GUIContent("Player Config"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                foreach (TileAuthoring t in targets)
                {
                    Undo.RecordObject(t, "Set Tile Owner");
                    t.SetOwnerFromConfig();
                    EditorUtility.SetDirty(t);
                }
            }
        }

        // ------------------------------------------------------------
        // Missing Config Warning
        // ------------------------------------------------------------
        private bool DrawConfigWarningIfMissing()
        {
            if (_config != null)
                return true;

            EditorGUILayout.HelpBox("Assign a TileConfig", MessageType.Warning);
            return false;
        }

        // ------------------------------------------------------------
        // Level Controls (Common Logic)
        // ------------------------------------------------------------
        private void DrawLevelControlsSection()
        {
            if (targets.Length > 1)
            {
                DrawMultiTileLevelControls();
            }
            else
            {
                DrawSingleTileLevelControls();
            }
        }

        private void DrawSingleTileLevelControls()
        {
            ValidateCurrentLevel();

            EditorGUILayout.Space(10);
            DrawCurrentLevelLabel();
            EditorGUILayout.Space(4);
            DrawLevelButtons();
            EditorGUILayout.Space(4);
            DrawUpDownButtons();
        }

        private void DrawMultiTileLevelControls()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField($"Level ({targets.Length} tiles)", EditorStyles.boldLabel);
            DrawMultiLevelButtons();
            DrawMultiUpDownButtons();
        }

        // ------------------------------------------------------------
        // Current Level Validation (Single Tile)
        // ------------------------------------------------------------
        private void ValidateCurrentLevel()
        {
            int current = _authoring.currentLevel;
            int min = _config.MinTileLevel;
            int max = _config.MaxTileLevel;

            if (current >= min && current <= max)
                return;

            EditorGUILayout.HelpBox(
                $"Current level ({current}) is out of allowed range [{min}, {max}].",
                MessageType.Error);

            if (GUILayout.Button("Fix Level (clamp to range)"))
            {
                int clamped = Mathf.Clamp(current, min, max);
                SetLevelWithUndo(clamped, "Fix Tile Level");
            }

            EditorGUILayout.Space(4);
        }

        private void DrawCurrentLevelLabel()
        {
            EditorGUILayout.LabelField($"Current level: {_authoring.currentLevel}", EditorStyles.boldLabel);
        }

        // ------------------------------------------------------------
        // Level Buttons (Single Tile)
        // ------------------------------------------------------------
        private void DrawLevelButtons()
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = _config.MinTileLevel; i <= _config.MaxTileLevel; i++)
            {
                bool isCurrent = _authoring.currentLevel == i;
                GUI.backgroundColor = isCurrent ? Color.green : Color.white;

                if (GUILayout.Button($"Lvl {i}", GUILayout.Height(30)))
                    SetLevelWithUndo(i, $"Set Tile Level {i}");
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        // ------------------------------------------------------------
        // Up/Down Buttons (Single Tile)
        // ------------------------------------------------------------
        private void DrawUpDownButtons()
        {
            int current = _authoring.currentLevel;
            int min = _config.MinTileLevel;
            int max = _config.MaxTileLevel;

            using (new EditorGUILayout.HorizontalScope())
            {
                bool canIncrease = current < max;
                EditorGUI.BeginDisabledGroup(!canIncrease);
                if (GUILayout.Button("▲ Level up"))
                    SetLevelWithUndo(current + 1, "Increase Tile Level");
                EditorGUI.EndDisabledGroup();

                bool canDecrease = current > min;
                EditorGUI.BeginDisabledGroup(!canDecrease);
                if (GUILayout.Button("▼ Level down"))
                    SetLevelWithUndo(current - 1, "Decrease Tile Level");
                EditorGUI.EndDisabledGroup();
            }
        }

        // ------------------------------------------------------------
        // Level Buttons (Multi-selection)
        // ------------------------------------------------------------
        private void DrawMultiLevelButtons()
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = _config.MinTileLevel; i <= _config.MaxTileLevel; i++)
            {
                int level = i;
                bool allAtLevel = true;
                foreach (Object obj in targets)
                {
                    if (obj is TileAuthoring t && t.currentLevel != level)
                    {
                        allAtLevel = false;
                        break;
                    }
                }

                GUI.backgroundColor = allAtLevel ? Color.green : Color.white;

                if (GUILayout.Button($"Lvl {level}", GUILayout.Height(30)))
                    SetLevelAllWithUndo(level, $"Set All Tiles Level {level}");
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        // ------------------------------------------------------------
        // Up/Down Buttons (Multi-selection)
        // ------------------------------------------------------------
        private void DrawMultiUpDownButtons()
        {
            int min = _config.MinTileLevel;
            int max = _config.MaxTileLevel;

            bool anyCanIncrease = false;
            bool anyCanDecrease = false;
            foreach (TileAuthoring t in targets)
            {
                if (t.currentLevel < max) anyCanIncrease = true;
                if (t.currentLevel > min) anyCanDecrease = true;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginDisabledGroup(!anyCanIncrease);
                if (GUILayout.Button("▲ Level up"))
                {
                    foreach (TileAuthoring t in targets)
                        SetLevelForTargetWithUndo(t, Mathf.Min(t.currentLevel + 1, max), "Increase Tile Level");
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!anyCanDecrease);
                if (GUILayout.Button("▼ Level down"))
                {
                    foreach (TileAuthoring t in targets)
                        SetLevelForTargetWithUndo(t, Mathf.Max(t.currentLevel - 1, min), "Decrease Tile Level");
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        // ------------------------------------------------------------
        // Helper Methods for Setting Level with Undo
        // ------------------------------------------------------------
        private void SetLevelAllWithUndo(int newLevel, string undoName)
        {
            foreach (TileAuthoring t in targets)
                SetLevelForTargetWithUndo(t, newLevel, undoName);
        }

        private void SetLevelForTargetWithUndo(TileAuthoring t, int newLevel, string undoName)
        {
            Undo.RecordObject(t, undoName);
            t.SetLevel(newLevel);
            EditorUtility.SetDirty(t);
        }

        private void SetLevelWithUndo(int newLevel, string undoName)
        {
            Undo.RecordObject(_authoring, undoName);
            _authoring.SetLevel(newLevel);
            EditorUtility.SetDirty(_authoring);
        }
    }
}