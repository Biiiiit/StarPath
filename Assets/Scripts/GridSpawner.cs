using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public float spacing = 1.5f;
    public SpriteRenderer background;

    public WaveFormation previewFormation; // editor preview
    public Transform alienParent; // assign the parent with AlienManager in inspector

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying && previewFormation != null)
        {
            // Clear previous preview children safely
            ClearChildren(true);
            SpawnFormation(previewFormation);
        }
    }
#endif

    public void SpawnFormation(WaveFormation formation)
    {
        if (formation == null || background == null || alienParent == null) return;

        // Clear previous runtime aliens
        if (Application.isPlaying)
            ClearChildren(false);

        Bounds bounds = background.bounds;

        int rows = formation.rows;
        int cols = formation.cols;

        float totalWidth = (cols - 1) * spacing;
        float totalHeight = (rows - 1) * spacing;

        float startX = -totalWidth / 2f;
        float startY = bounds.max.y - 1f; // start at top

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

                GameObject alien = Instantiate(alienPrefab, alienParent);
                alien.transform.localPosition = localPos;
            }
        }

        // Update AlienManager counts if runtime
        if (Application.isPlaying)
        {
            AlienManager manager = alienParent.GetComponent<AlienManager>();
            if (manager != null)
            {
                manager.totalAliens = alienParent.childCount;
                manager.aliveAliens = manager.totalAliens;
            }
        }
    }

    private void ClearChildren(bool editorMode)
    {
        if (alienParent == null) return;

        // iterate backwards to safely remove all children
        for (int i = alienParent.childCount - 1; i >= 0; i--)
        {
            Transform child = alienParent.GetChild(i);

            if (editorMode)
                DestroyImmediate(child.gameObject); // preview removal in editor
            else if (Application.isPlaying)
                Destroy(child.gameObject); // runtime removal
        }
    }
}