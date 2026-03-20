using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveFormation))]
public class WaveFormationEditor : Editor
{
    private const int cellSize = 70;
    private bool isPainting = false;
    private bool eraseMode = false;

    public override void OnInspectorGUI()
    {
        WaveFormation formation = (WaveFormation)target;

        formation.rows = EditorGUILayout.IntField("Rows", formation.rows);
        formation.cols = EditorGUILayout.IntField("Cols", formation.cols);

        int size = formation.rows * formation.cols;

        if (formation.grid == null || formation.grid.Length != size)
        {
            formation.grid = new GameObject[size];
        }

        EditorGUILayout.Space();

        // Paint settings
        formation.selectedPrefab = (GameObject)EditorGUILayout.ObjectField(
            "Paint Prefab",
            formation.selectedPrefab,
            typeof(GameObject),
            false
        );

        eraseMode = EditorGUILayout.Toggle("Erase Mode", eraseMode);

        EditorGUILayout.Space();

        Event e = Event.current;

        for (int y = 0; y < formation.rows; y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < formation.cols; x++)
            {
                int index = x + y * formation.cols;

                DrawCell(formation, index, e);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(formation);
        }
    }

    void DrawCell(WaveFormation formation, int index, Event e)
    {
        GameObject prefab = formation.grid[index];
        Texture preview = GetPreview(prefab);

        Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize);

        GUI.DrawTexture(rect, preview);

        // Mouse interactions
        if (rect.Contains(e.mousePosition))
        {
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                isPainting = true;
                Paint(formation, index);
                e.Use();
            }

            if (e.type == EventType.MouseDrag && isPainting)
            {
                Paint(formation, index);
                e.Use();
            }

            if (e.type == EventType.MouseDown && e.button == 1)
            {
                formation.grid[index] = null;
                e.Use();
            }
        }

        if (e.type == EventType.MouseUp)
        {
            isPainting = false;
        }
    }

    void Paint(WaveFormation formation, int index)
    {
        if (eraseMode)
        {
            formation.grid[index] = null;
        }
        else if (formation.selectedPrefab != null)
        {
            formation.grid[index] = formation.selectedPrefab;
        }
    }

    Texture GetPreview(GameObject prefab)
    {
        if (prefab == null)
            return Texture2D.grayTexture;

        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
            return sr.sprite.texture;

        Texture preview = AssetPreview.GetAssetPreview(prefab);
        if (preview != null)
            return preview;

        return AssetPreview.GetMiniThumbnail(prefab);
    }
}