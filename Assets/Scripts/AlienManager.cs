using UnityEngine;

public class AlienManager : MonoBehaviour
{
    public float baseSpeed = 1f;
    public float maxSpeed = 3f;
    private float speed;
    private Vector3 direction = Vector3.right;

    public SpriteRenderer background;

    private float leftBound;
    private float rightBound;


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

        totalAliens = transform.childCount;
        aliveAliens = totalAliens;
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