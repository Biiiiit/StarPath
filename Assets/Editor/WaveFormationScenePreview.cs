using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSpawner))]
public class GridSpawnerEditor : Editor
{
    void OnSceneGUI()
    {
        GridSpawner spawner = (GridSpawner)target;

        if (spawner.previewFormation == null) return;

        var formation = spawner.previewFormation;

        for (int y = 0; y < formation.rows; y++)
        {
            for (int x = 0; x < formation.cols; x++)
            {
                int index = x + y * formation.cols;

                if (formation.grid[index] == null) continue;

                Vector3 pos = spawner.transform.position +
                              new Vector3(x * spawner.spacing, -y * spawner.spacing, 0);

                Handles.DrawWireCube(pos, Vector3.one * 0.8f);
            }
        }
    }
}