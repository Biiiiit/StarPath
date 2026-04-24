using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveFormation))]
public class WaveFormationEditor : Editor
{
    private const int cellSize = 70;

    private bool isPainting = false;
    private bool eraseMode = false;

    private int selectedIndex = 0;

    public override void OnInspectorGUI()
    {
        WaveFormation formation = (WaveFormation)target;

        formation.rows = Mathf.Max(
            1,
            EditorGUILayout.IntField("Rows", formation.rows <= 0 ? 4 : formation.rows)
        );

        formation.cols = Mathf.Max(
            1,
            EditorGUILayout.IntField("Cols", formation.cols <= 0 ? 11 : formation.cols)
        );

        formation.difficulty = (WaveDifficulty)EditorGUILayout.EnumPopup(
        "Difficulty",
        formation.difficulty
        );

        int size = formation.rows * formation.cols;

        if (formation.grid == null || formation.grid.Length != size)
        {
            GameObject[] newGrid = new GameObject[size];

            if (formation.grid != null)
            {
                for (int i = 0; i < Mathf.Min(size, formation.grid.Length); i++)
                    newGrid[i] = formation.grid[i];
            }

            formation.grid = newGrid;
        }

        EditorGUILayout.Space();

        SerializedObject so = serializedObject;
        so.Update();

        EditorGUILayout.PropertyField(
            so.FindProperty("availablePrefabs"),
            true
        );

        so.ApplyModifiedProperties();

        DrawPrefabDropdown(formation);

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

    void DrawPrefabDropdown(WaveFormation formation)
    {
        if (formation.availablePrefabs == null || formation.availablePrefabs.Length == 0)
        {
            EditorGUILayout.HelpBox(
                "Add prefabs to Available Prefabs in WaveFormation.",
                MessageType.Info
            );

            formation.selectedPrefab = null;
            return;
        }

        string[] names = new string[formation.availablePrefabs.Length];

        for (int i = 0; i < names.Length; i++)
        {
            names[i] = formation.availablePrefabs[i] != null
                ? formation.availablePrefabs[i].name
                : "Empty";
        }

        selectedIndex = Mathf.Clamp(
            selectedIndex,
            0,
            formation.availablePrefabs.Length - 1
        );

        selectedIndex = EditorGUILayout.Popup(
            "Paint Alien",
            selectedIndex,
            names
        );

        formation.selectedPrefab =
            formation.availablePrefabs[selectedIndex];
    }

    void DrawCell(WaveFormation formation, int index, Event e)
    {
        GameObject prefab = formation.grid[index];
        Texture preview = GetPreview(prefab);

        Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize);

        EditorGUI.DrawRect(rect, Color.gray * 0.3f);

        if (preview != null)
        {
            GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
        }

        Handles.DrawSolidRectangleWithOutline(
            rect,
            Color.clear,
            Color.black
        );

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