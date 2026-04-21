using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public float spacing = 1.5f;
    public SpriteRenderer background;
    public Transform alienParent;

    public void SpawnFormation(WaveFormation formation)
    {
        if (formation == null) return;
        if (background == null) return;
        if (alienParent == null) return;

        ClearChildren();

        Bounds bounds = background.bounds;

        int rows = formation.rows;
        int cols = formation.cols;

        float totalWidth = (cols - 1) * spacing;
        float startX = -totalWidth / 2f;
        float startY = bounds.max.y - 1f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = x + y * cols;

                if (index >= formation.grid.Length)
                    continue;

                GameObject prefab = formation.grid[index];

                if (prefab == null)
                    continue;

                Vector3 pos = new Vector3(
                    startX + x * spacing,
                    startY - y * spacing,
                    0f
                );

                GameObject alienObj =
                    Instantiate(prefab, alienParent);

                alienObj.transform.localPosition = pos;

                Alien alien =
                    alienObj.GetComponent<Alien>();

                if (alien != null)
                {
                    alien.manager =
                        alienParent.GetComponent<AlienManager>();
                }
            }
        }

        AlienManager manager =
            alienParent.GetComponent<AlienManager>();

        if (manager != null)
            manager.ResetAliens();
    }

    void ClearChildren()
    {
        for (int i = alienParent.childCount - 1; i >= 0; i--)
        {
            Destroy(alienParent.GetChild(i).gameObject);
        }
    }
}