using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        private const string DefaultLevelFolder = "Assets/Project/ScriptableObjects/Levels";
        private const float MinimumCellSize = 8f;
        private const float MaximumCellSize = 32f;

        private static readonly Color AliveColor = new(0.76f, 0.76f, 0.79f);
        private static readonly Color DeadColor = new(0.14f, 0.14f, 0.17f);
        private static readonly Color GridLineColor = new(0.32f, 0.32f, 0.35f);

        private LevelConfig _level;
        private Vector2 _scroll;
        private bool _isPainting;
        private bool _paintsDead;
        private int _undoGroup;

        [MenuItem("Gazeus/Level Editor")]
        public static void Open()
        {
            GetWindow<LevelEditorWindow>("Level Editor").minSize = new Vector2(320f, 320f);
        }

        #region Unity
        private void OnSelectionChange()
        {
            if (Selection.activeObject is not LevelConfig selected) return;

            _level = selected;
            Repaint();
        }

        private void OnGUI()
        {
            DrawHeader();

            if (_level == null)
            {
                EditorGUILayout.HelpBox("Select a LevelConfig asset or create a new one.", MessageType.Info);
                return;
            }

            DrawSizeFields();
            DrawFillButtons();

            EditorGUILayout.Space();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            DrawGrid();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.LabelField($"{_level.Width} x {_level.Height} - " +
                                       $"{_level.DeadCellCount()} dead cells");
        }
        #endregion

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            _level = (LevelConfig)EditorGUILayout.ObjectField("Level", _level, typeof(LevelConfig), false);
            if (GUILayout.Button("New...", GUILayout.Width(60f))) CreateLevel();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSizeFields()
        {
            EditorGUI.BeginChangeCheck();
            int width = EditorGUILayout.IntField("Width", _level.Width);
            int height = EditorGUILayout.IntField("Height", _level.Height);

            if (!EditorGUI.EndChangeCheck()) return;

            Undo.RecordObject(_level, "Resize Level");
            _level.Resize(width, height);
            Save();
        }

        private void DrawFillButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Clear all"))
            {
                Undo.RecordObject(_level, "Clear Level");
                _level.SetAllDead(false);
                Save();
            }

            if (GUILayout.Button("Fill all"))
            {
                Undo.RecordObject(_level, "Fill Level");
                _level.SetAllDead(true);
                Save();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawGrid()
        {
            float cellSize = CalculateCellSize();
            Rect grid = GUILayoutUtility.GetRect(_level.Width * cellSize, _level.Height * cellSize,
                GUILayout.ExpandWidth(false));

            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawRect(grid, GridLineColor);

                for (int y = 0; y < _level.Height; y++)
                {
                    for (int x = 0; x < _level.Width; x++)
                    {
                        Rect cell = new(grid.x + x * cellSize + 1f, grid.y + y * cellSize + 1f,
                            cellSize - 1f, cellSize - 1f);

                        EditorGUI.DrawRect(cell, _level.IsDead(x, y) ? DeadColor : AliveColor);
                    }
                }
            }

            HandlePainting(grid, cellSize);
        }
        
        private float CalculateCellSize()
        {
            float available = position.width - 32f;

            return Mathf.Clamp(Mathf.Floor(available / _level.Width), MinimumCellSize, MaximumCellSize);
        }

        private void HandlePainting(Rect grid, float cellSize)
        {
            Event current = Event.current;

            if (current.type == EventType.MouseUp && _isPainting)
            {
                _isPainting = false;
                Undo.CollapseUndoOperations(_undoGroup);
                Save();
                current.Use();

                return;
            }

            if (!grid.Contains(current.mousePosition)) return;

            int x = Mathf.Clamp(Mathf.FloorToInt((current.mousePosition.x - grid.x) / cellSize), 0, _level.Width - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt((current.mousePosition.y - grid.y) / cellSize), 0, _level.Height - 1);

            // The first cell decides whether the whole stroke paints or erases.
            if (current.type == EventType.MouseDown && current.button == 0)
            {
                _isPainting = true;
                _paintsDead = !_level.IsDead(x, y);
                _undoGroup = Undo.GetCurrentGroup();
                Paint(x, y);
                current.Use();
            }
            else if (current.type == EventType.MouseDrag && _isPainting)
            {
                Paint(x, y);
                current.Use();
            }
        }

        private void Paint(int x, int y)
        {
            if (_level.IsDead(x, y) == _paintsDead) return;

            Undo.RecordObject(_level, "Paint Dead Cells");
            _level.SetDead(x, y, _paintsDead);
            Repaint();
        }

        private void CreateLevel()
        {
            string path = EditorUtility.SaveFilePanelInProject("New Level", "Level01", "asset",
                "Where should the level be saved?", DefaultLevelFolder);
            if (string.IsNullOrEmpty(path)) return;

            LevelConfig level = CreateInstance<LevelConfig>();
            AssetDatabase.CreateAsset(level, path);
            AssetDatabase.SaveAssets();

            _level = level;
        }

        private void Save()
        {
            EditorUtility.SetDirty(_level);
            AssetDatabase.SaveAssets();
        }
    }
}
