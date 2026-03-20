using UnityEngine;

public class AlienManager : MonoBehaviour
{
    public WaveFormation formation;
    public float spacing = 1.5f;

    public float baseSpeed = 1f;
    public float maxSpeed = 3f;
    private float speed;
    private Vector3 direction = Vector3.right;

    public SpriteRenderer background;

    private float leftBound;
    private float rightBound;

    private int totalAliens;
    private int aliveAliens;

    void Start()
    {
        speed = baseSpeed;

        Bounds bounds = background.bounds;
        leftBound = bounds.min.x;
        rightBound = bounds.max.x;

        SpawnFormation(bounds);
    }

    void Update()
    {
        MoveFormation();
    }

    public void AlienKilled()
    {
        aliveAliens--;

        float progress = 1f - (float)aliveAliens / totalAliens;
        float curved = progress * progress;

        speed = Mathf.Lerp(baseSpeed, maxSpeed, curved);
    }

    void SpawnFormation(Bounds bounds)
    {
        if (formation == null)
        {
            Debug.LogWarning("No formation assigned!");
            return;
        }

        int rows = formation.rows;
        int cols = formation.cols;

        float totalWidth = (cols - 1) * spacing;
        float totalHeight = (rows - 1) * spacing;

        float startX = -totalWidth / 2f;
        float alienHeight = 0f;
        for (int i = 0; i < formation.grid.Length; i++)
        {
            if (formation.grid[i] != null)
            {
                SpriteRenderer sr = formation.grid[i].GetComponent<SpriteRenderer>();
                if (sr != null) alienHeight = sr.bounds.extents.y;
                break;
            }
        }

        float startY = bounds.max.y - alienHeight; // top row aligns with top of background

        totalAliens = 0;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = x + y * cols;

                GameObject prefab = formation.grid[index];
                if (prefab == null) continue;

                Vector3 localPos = new Vector3(
                    startX + x * spacing,
                    startY - y * spacing,
                    0
                );

                GameObject alien = Instantiate(prefab, transform);
                alien.transform.localPosition = localPos;

                totalAliens++;
            }
        }

        aliveAliens = totalAliens;
    }

    void MoveFormation()
    {
        if (aliveAliens <= 0) return;

        float moveStep = speed * Time.deltaTime;

        float nextLeft = float.MaxValue;
        float nextRight = float.MinValue;

        foreach (Transform alien in transform)
        {
            if (alien == null) continue;

            SpriteRenderer sr = alien.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            float halfWidth = sr.bounds.extents.x;

            float nextX = alien.position.x + direction.x * moveStep;

            nextLeft = Mathf.Min(nextLeft, nextX - halfWidth);
            nextRight = Mathf.Max(nextRight, nextX + halfWidth);
        }

        if (nextRight > rightBound || nextLeft < leftBound)
        {
            direction *= -1;
            transform.position += Vector3.down * 0.5f;
            return;
        }

        transform.Translate(direction * moveStep);
    }
}