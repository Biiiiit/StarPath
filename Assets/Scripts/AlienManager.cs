using UnityEngine;

public class AlienManager : MonoBehaviour
{
    public GameObject alienPrefab;
    public int rows = 3;
    public int cols = 6;
    public float spacing = 1.5f;

    public float baseSpeed = 1f;
    public float maxSpeed = 3f;
    private float speed; // CURRENT speed
    private Vector3 direction = Vector3.right;

    public SpriteRenderer background;

    private float leftBound;
    private float rightBound;

    private int totalAliens;
    private int aliveAliens;

    void Start()
    {
        totalAliens = rows * cols;
        aliveAliens = totalAliens;
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

        // Smooth speed increase
        float progress = 1f - (float)aliveAliens / totalAliens;
        float curved = progress * progress; // slow start, faster at the end
        speed = Mathf.Lerp(baseSpeed, maxSpeed, curved);
    }

    void SpawnFormation(Bounds bounds)
    {
        float totalWidth = (cols - 1) * spacing;
        float totalHeight = (rows - 1) * spacing;

        float startX = -totalWidth / 2f;
        float topY = bounds.max.y - 1f; // small margin from top
        float startY = topY - totalHeight;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 localPos = new Vector3(
                    startX + x * spacing,
                    startY + y * spacing,
                    0
                );

                GameObject alien = Instantiate(alienPrefab, transform);
                alien.transform.localPosition = localPos;
            }
        }
    }

    void MoveFormation()
    {
        float moveStep = speed * Time.deltaTime;

        // Predict next edge positions in world space
        float nextLeft = float.MaxValue;
        float nextRight = float.MinValue;

        foreach (Transform alien in transform)
        {
            float halfWidth = alien.GetComponent<SpriteRenderer>().bounds.extents.x;

            Vector3 worldPos = alien.position;
            float nextX = worldPos.x + direction.x * moveStep;

            nextLeft = Mathf.Min(nextLeft, nextX - halfWidth);
            nextRight = Mathf.Max(nextRight, nextX + halfWidth);
        }

        // Reverse direction if we hit the background bounds
        if (nextRight > rightBound || nextLeft < leftBound)
        {
            direction *= -1;
            transform.position += Vector3.down * 0.5f; // move down
            return; // skip movement this frame to prevent overshoot
        }

        // Move formation
        transform.Translate(direction * moveStep);
    }
}