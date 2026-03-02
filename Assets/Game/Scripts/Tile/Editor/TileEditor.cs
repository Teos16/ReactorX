using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Tile.Editor
{
    [CustomEditor(typeof(TilePresenter))]
    public class TileEditor : UnityEditor.Editor
    {
        private TilePresenter _presenter;
        private TileConfig _config;
        private SerializedProperty _configProperty;
        private SerializedProperty _currentLevelProperty;

        private void OnEnable()
        {
            _configProperty = serializedObject.FindProperty(nameof(TilePresenter.config));
            _currentLevelProperty = serializedObject.FindProperty(nameof(TilePresenter.currentLevel));
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();

            _presenter = (TilePresenter)target;
            _config = _configProperty.objectReferenceValue as TileConfig;

            if (!DrawConfigWarningIfMissing())
                return;

            ValidateCurrentLevel();

            EditorGUILayout.Space(10);
            DrawCurrentLevelLabel();
            EditorGUILayout.Space(4);
            DrawLevelButtons();
            EditorGUILayout.Space(4);
            DrawUpDownButtons();

            serializedObject.ApplyModifiedProperties();
        }

        private bool DrawConfigWarningIfMissing()
        {
            if (_config != null)
                return true;

            EditorGUILayout.HelpBox("Assign a TileConfig", MessageType.Warning);
            return false;
        }

        private void ValidateCurrentLevel()
        {
            int current = _presenter.currentLevel;
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
            EditorGUILayout.LabelField($"Current level: {_presenter.currentLevel}", EditorStyles.boldLabel);
        }

        private void DrawLevelButtons()
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = _config.MinTileLevel; i <= _config.MaxTileLevel; i++)
            {
                bool isCurrent = _presenter.currentLevel == i;
                GUI.backgroundColor = isCurrent ? Color.green : Color.white;

                if (GUILayout.Button($"Lvl {i}", GUILayout.Height(30)))
                {
                    SetLevelWithUndo(i, $"Set Tile Level {i}");
                }
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawUpDownButtons()
        {
            int current = _presenter.currentLevel;
            int min = _config.MinTileLevel;
            int max = _config.MaxTileLevel;

            using (new EditorGUILayout.HorizontalScope())
            {
                bool canIncrease = current < max;
                EditorGUI.BeginDisabledGroup(!canIncrease);
                if (GUILayout.Button("▲ Level up"))
                {
                    SetLevelWithUndo(current + 1, "Increase Tile Level");
                }
                EditorGUI.EndDisabledGroup();

                bool canDecrease = current > min;
                EditorGUI.BeginDisabledGroup(!canDecrease);
                if (GUILayout.Button("▼ Level down"))
                {
                    SetLevelWithUndo(current - 1, "Decrease Tile Level");
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        private void SetLevelWithUndo(int newLevel, string undoName)
        {
            Undo.RecordObject(_presenter, undoName);
            _presenter.SetLevel(newLevel);
            EditorUtility.SetDirty(_presenter);
        }
    }
}