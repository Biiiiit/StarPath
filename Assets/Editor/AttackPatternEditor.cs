using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AttackPattern))]
public class AttackPatternEditor : Editor
{
    private bool isPainting = false;
    private bool paintValue = true;

    public override void OnInspectorGUI()
    {
        AttackPattern pattern = (AttackPattern)target;

        if (pattern.width == 0 && pattern.height == 0)
        {
            pattern.width = 15;
            pattern.height = 30;
        }

        EditorGUI.BeginChangeCheck();

        int newWidth = EditorGUILayout.IntField("Width", pattern.width);
        int newHeight = EditorGUILayout.IntField("Height", pattern.height);

        if (EditorGUI.EndChangeCheck())
        {
            pattern.width = Mathf.Max(1, newWidth);
            pattern.height = Mathf.Max(1, newHeight);

            pattern.grid = new bool[pattern.width * pattern.height];
        }

        int size = pattern.width * pattern.height;

        if (pattern.grid == null || pattern.grid.Length != size)
        {
            bool[] newGrid = new bool[size];

            if (pattern.grid != null)
            {
                for (int i = 0; i < Mathf.Min(pattern.grid.Length, size); i++)
                {
                    newGrid[i] = pattern.grid[i];
                }
            }

            pattern.grid = newGrid;
        }

        EditorGUILayout.Space();

        Event e = Event.current;

        for (int y = 0; y < pattern.height; y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < pattern.width; x++)
            {
                int index = y * pattern.width + x;

                Rect rect = GUILayoutUtility.GetRect(25, 25);

                bool value = pattern.grid[index];

                // Draw cell
                EditorGUI.DrawRect(rect, value ? Color.green : Color.gray);

                // Draw border
                Handles.color = Color.black;
                Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.black);

                // Start painting
                if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
                {
                    isPainting = true;

                    if (e.button == 0) paintValue = true;
                    if (e.button == 1) paintValue = false;

                    pattern.grid[index] = paintValue;
                    e.Use();
                }

                // Drag painting (fixed)
                if (isPainting && (e.type == EventType.MouseDrag || e.type == EventType.MouseDown))
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        pattern.grid[index] = paintValue;
                        e.Use();
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        // Stop painting
        if (e.type == EventType.MouseUp)
        {
            isPainting = false;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(pattern);
        }
    }
}