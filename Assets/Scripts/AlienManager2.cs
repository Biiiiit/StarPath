using UnityEngine;

public class AlienManager2 : MonoBehaviour
{
    public GameObject alienPrefab;
    public int rows = 3;
    public int cols = 6;
    public float spacing = 1.5f;

    public float baseSpeed = 2f;
    public float maxSpeed = 6f;
    private float speed;

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

        float progress = 1f - (float)aliveAliens / totalAliens;
        float curved = progress * progress;
        speed = Mathf.Lerp(baseSpeed, maxSpeed, curved);
    }

    void SpawnFormation(Bounds bounds)
    {
        float startX = bounds.max.x + 2f;
        float startY = 0f;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 pos = new Vector3(
                    startX + x * spacing,
                    startY + y * spacing,
                    0
                );

                GameObject alien = Instantiate(alienPrefab, pos, Quaternion.identity);
                alien.transform.parent = transform;
            }
        }
    }

    void MoveFormation()
    {
        float moveStep = speed * Time.deltaTime;

        transform.Translate(Vector3.left * moveStep);

        foreach (Transform alien in transform)
        {
            if (alien.position.x < leftBound - 2f)
            {
                Destroy(alien.gameObject);
            }
        }
    }
}