using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public float spacing = 1.5f;
    public SpriteRenderer background;
    public Transform alienParent; // assign the parent with AlienManager

    public void SpawnFormation(WaveFormation formation)
    {
        if (formation == null || background == null || alienParent == null) return;

        // Clear previous runtime aliens
        ClearChildren(alienParent);

        Bounds bounds = background.bounds;

        int rows = formation.rows;
        int cols = formation.cols;

        float totalWidth = (cols - 1) * spacing;
        float startX = -totalWidth / 2f;
        float startY = bounds.max.y - 0.5f; // start at top

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

                // Assign manager to the alien
                Alien alienScript = alien.GetComponent<Alien>();
                if (alienScript != null)
                    alienScript.manager = alienParent.GetComponent<AlienManager>();
            }
        }

        // Update AlienManager counts
        AlienManager manager = alienParent.GetComponent<AlienManager>();
        if (manager != null)
            manager.ResetAliens();
    }

    void ClearChildren(Transform parent)
    {
        if (parent == null) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}