using UnityEngine;

public class AlienManager : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 1f;
    public float maxSpeed = 3f;
    [Tooltip("Exponent for speed curve: >1 = slow start, <1 = fast start")]
    public float speedExponent = 2f;

    private float speed;
    private Vector3 direction = Vector3.right;

    [Header("References")]
    public SpriteRenderer background;

    private float leftBound;
    private float rightBound;

    [Header("Internal Counts")]
    [HideInInspector] public int totalAliens;
    [HideInInspector] public int aliveAliens;

    void Start()
    {
        speed = baseSpeed;

        if (background != null)
        {
            Bounds bounds = background.bounds;
            leftBound = bounds.min.x;
            rightBound = bounds.max.x;
        }

        ResetAliens(); // initialize counts & speed
    }

    void Update()
    {
        MoveFormation();
    }

    public void ResetAliens()
    {
        totalAliens = transform.childCount;
        aliveAliens = totalAliens;
        speed = baseSpeed;
    }

    public void AlienKilled()
    {
        if (totalAliens <= 0) return;

        aliveAliens = Mathf.Max(aliveAliens - 1, 0);

        // Exponential curve: slow start, fast near the end
        float progress = 1f - ((float)aliveAliens / (float)totalAliens);
        float curved = Mathf.Pow(progress, speedExponent);

        speed = Mathf.Lerp(baseSpeed, maxSpeed, curved);
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