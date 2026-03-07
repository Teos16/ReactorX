#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Game.Scripts.Tiles.Editor;

namespace Game.Scripts.Tile.Editor
{
    public class TileClusterManager : OdinEditorWindow
    {
        private const int ITEMS_PER_PAGE = 10;
        private const float BUTTON_SPACING = 6f;
        
        [MenuItem("Tools/Tile Cluster Manager")]
        private static void OpenWindow()
        {
            GetWindow<TileClusterManager>().Show();
        }

        private Dictionary<int, ClusterGroup> _clusters = new Dictionary<int, ClusterGroup>();

        [OnInspectorGUI]
        private void DrawClusters()
        {
            DrawGlobalButtons();
            GUILayout.Space(20);

            if (_clusters == null || _clusters.Count == 0)
            {
                SirenixEditorGUI.MessageBox("No clusters found. Click 'Refresh Clusters' to scan for tiles.", MessageType.Info);
                return;
            }

            foreach (var kvp in _clusters)
            {
                kvp.Value.DrawClusterGroup();
                GUILayout.Space(10);
            }
        }

        private void DrawGlobalButtons()
        {
            GUILayout.Space(8);
            
            // Заголовок глобальной секции
            SirenixEditorGUI.Title("🌍 Global Controls", null, TextAlignment.Left, true);
            GUILayout.Space(4);
            
            SirenixEditorGUI.BeginHorizontalToolbar();

            var buttonData = new (string text, Color color, System.Action action)[]
            {
                ("🔄 Refresh", new Color(0.3f, 0.8f, 1f), RefreshClusters),
                ("👁️ Show All", new Color(0.5f, 1f, 0.5f), ShowAllTiles),
                ("🙈 Hide All", new Color(1f, 0.5f, 0.5f), HideAllTiles),
                ("✅ Select All", new Color(0.8f, 0.8f, 0.5f), SelectAllTiles)
            };

            float maxWidth = 0f;
            foreach (var data in buttonData)
            {
                Vector2 size = GUI.skin.button.CalcSize(new GUIContent(data.text));
                if (size.x > maxWidth) maxWidth = size.x;
            }
            maxWidth += 24;

            for (int i = 0; i < buttonData.Length; i++)
            {
                var (text, color, action) = buttonData[i];
                GUI.backgroundColor = color;
                if (GUILayout.Button(text, GUILayout.Width(maxWidth)))
                {
                    action?.Invoke();
                }
                if (i < buttonData.Length - 1)
                {
                    GUILayout.Space(BUTTON_SPACING);
                }
            }

            GUI.backgroundColor = Color.white;
            GUILayout.FlexibleSpace();
            SirenixEditorGUI.EndHorizontalToolbar();
            GUILayout.Space(5);
        }

        private void RefreshClusters() => CollectTiles();
        private void ShowAllTiles() => SetAllTilesVisibility(true);
        private void HideAllTiles() => SetAllTilesVisibility(false);
        private void SelectAllTiles()
        {
            var allTiles = FindObjectsOfType<TileAuthoring>(true);
            Selection.objects = allTiles.Select(t => t.gameObject).ToArray();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CollectTiles();
            SubscribeToEvents();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            Undo.undoRedoPerformed += RefreshClusters;
            EditorApplication.hierarchyChanged += RefreshClusters;
            TileClusterChangeNotifier.OnClusterChanged += RefreshClusters;
        }

        private void UnsubscribeFromEvents()
        {
            Undo.undoRedoPerformed -= RefreshClusters;
            EditorApplication.hierarchyChanged -= RefreshClusters;
            TileClusterChangeNotifier.OnClusterChanged -= RefreshClusters;
        }

        private void CollectTiles()
        {
            var allTiles = FindObjectsOfType<TileAuthoring>(true);
            var newClusters = new Dictionary<int, ClusterGroup>();

            foreach (var tile in allTiles)
            {
                if (tile == null) continue;
                
                int clusterId = tile.cluster;
                if (!newClusters.ContainsKey(clusterId))
                {
                    newClusters[clusterId] = new ClusterGroup(clusterId);
                }
                newClusters[clusterId].AddTile(tile);
            }

            _clusters = newClusters.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            Repaint();
        }

        private void SetAllTilesVisibility(bool visible)
        {
            foreach (var cluster in _clusters.Values)
            {
                cluster.SetVisibility(visible);
            }
        }

        [System.Serializable]
        public class ClusterGroup
        {
            private readonly int _clusterId;
            private readonly List<TileAuthoring> _tiles = new List<TileAuthoring>();
            private bool _isExpanded = true;
            private int _currentPage = 0;

            public ClusterGroup(int clusterId)
            {
                _clusterId = clusterId;
            }

            public void AddTile(TileAuthoring tile)
            {
                if (tile != null)
                {
                    _tiles.Add(tile);
                }
            }

            public void DrawClusterGroup()
            {
                SirenixEditorGUI.BeginBox();
                
                // Заголовок с эмодзи и жирным текстом
                var headerRect = EditorGUILayout.GetControlRect();
                var headerStyle = new GUIStyle(SirenixGUIStyles.BoldLabel);
                _isExpanded = SirenixEditorGUI.Foldout(headerRect, _isExpanded, GetHeaderText(), headerStyle);

                if (_isExpanded)
                {
                    GUILayout.Space(5);
                    
                    SirenixEditorGUI.BeginHorizontalToolbar();

                    var buttonData = new (string text, Color color, System.Action action)[]
                    {
                        ("👁️ Show", new Color(0.5f, 1f, 0.5f), () => SetVisibility(true)),
                        ("🙈 Hide", new Color(1f, 0.5f, 0.5f), () => SetVisibility(false)),
                        ("✅ Select", new Color(0.8f, 0.8f, 0.5f), SelectTiles)
                    };

                    float maxWidth = 0f;
                    foreach (var data in buttonData)
                    {
                        Vector2 size = GUI.skin.button.CalcSize(new GUIContent(data.text));
                        if (size.x > maxWidth) maxWidth = size.x;
                    }
                    maxWidth += 24;

                    for (int i = 0; i < buttonData.Length; i++)
                    {
                        var (text, color, action) = buttonData[i];
                        GUI.backgroundColor = color;
                        if (GUILayout.Button(text, GUILayout.Width(maxWidth)))
                        {
                            action?.Invoke();
                        }
                        if (i < buttonData.Length - 1)
                        {
                            GUILayout.Space(6);
                        }
                    }

                    GUI.backgroundColor = Color.white;
                    SirenixEditorGUI.EndHorizontalToolbar();

                    GUILayout.Space(8);
                    DrawTilesList();
                }

                SirenixEditorGUI.EndBox();
            }

            private string GetHeaderText()
            {
                int active = _tiles.Count(t => t != null && t.gameObject.activeSelf);
                int hidden = _tiles.Count - active;
                return $"🗂️ Cluster {_clusterId} — {_tiles.Count} tiles — {active} active / {hidden} hidden";
            }

            private void DrawTilesList()
            {
                if (_tiles.Count == 0)
                {
                    EditorGUILayout.HelpBox("No tiles in this cluster", MessageType.Info);
                    return;
                }

                int totalPages = Mathf.CeilToInt((float)_tiles.Count / ITEMS_PER_PAGE);
                _currentPage = Mathf.Clamp(_currentPage, 0, totalPages - 1);

                int startIndex = _currentPage * ITEMS_PER_PAGE;
                int endIndex = Mathf.Min(startIndex + ITEMS_PER_PAGE, _tiles.Count);

                for (int i = startIndex; i < endIndex; i++)
                {
                    var tile = _tiles[i];
                    if (tile != null)
                    {
                        EditorGUILayout.ObjectField($"Tile {i}", tile, typeof(TileAuthoring), true);
                    }
                }

                if (totalPages > 1)
                {
                    GUILayout.Space(8);
                    SirenixEditorGUI.BeginHorizontalToolbar();
                    
                    if (GUILayout.Button("◀", GUILayout.Width(30)))
                    {
                        _currentPage = Mathf.Max(0, _currentPage - 1);
                    }
                    
                    GUILayout.FlexibleSpace();
                    GUILayout.Label($"Page {_currentPage + 1} / {totalPages}", SirenixGUIStyles.LabelCentered);
                    GUILayout.FlexibleSpace();
                    
                    if (GUILayout.Button("▶", GUILayout.Width(30)))
                    {
                        _currentPage = Mathf.Min(totalPages - 1, _currentPage + 1);
                    }
                    
                    SirenixEditorGUI.EndHorizontalToolbar();
                }
            }

            private void SelectTiles()
            {
                var validTiles = _tiles.Where(t => t != null).Select(t => t.gameObject).ToArray();
                if (validTiles.Length > 0)
                {
                    Selection.objects = validTiles;
                }
            }

            public void SetVisibility(bool visible)
            {
                var validTiles = _tiles.Where(t => t != null && t.gameObject != null).ToArray();
                if (validTiles.Length == 0) return;

                Undo.RecordObjects(
                    validTiles.Select(t => t.gameObject).ToArray(),
                    visible ? "Show Cluster Tiles" : "Hide Cluster Tiles"
                );

                foreach (var tile in validTiles)
                {
                    tile.gameObject.SetActive(visible);
                    EditorUtility.SetDirty(tile.gameObject);
                }
            }
        }
    }
}
#endif