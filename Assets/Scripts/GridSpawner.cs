using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public float spacing = 1.5f;
    public SpriteRenderer background;

    public WaveFormation previewFormation; // for editor preview

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying && previewFormation != null)
        {
            // spawn editor preview ONLY
            SpawnFormation(previewFormation);
        }
    }
#endif
    public void SpawnFormation(WaveFormation formation)
    {
        if (formation == null || background == null) return;

        Bounds bounds = background.bounds;

        int rows = formation.rows;
        int cols = formation.cols;

        float totalWidth = (cols - 1) * spacing;
        float totalHeight = (rows - 1) * spacing;

        // CENTER horizontally
        float startX = -totalWidth / 2f;

        // START FROM TOP of background
        float topY = bounds.max.y - 1f;
        float startY = topY - totalHeight;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = x + y * cols;

                GameObject alienPrefab = formation.grid[index];
                if (alienPrefab == null) continue;

                Vector3 localPos = new Vector3(
                    startX + x * spacing,
                    startY - y * spacing,
                    0
                );

                Instantiate(alienPrefab, transform);
                transform.GetChild(transform.childCount - 1).localPosition = localPos;
            }
        }
    }

}