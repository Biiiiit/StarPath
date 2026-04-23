using System.Collections;
using UnityEngine;

public class AlienManager : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 1f;
    public float maxSpeed = 5f;
    public float stepDown = 0.5f;
    public float speedExponent = 1f;

    private float speed;
    private Vector3 direction = Vector3.right;

    public Vector3 Direction => direction;
    private bool hasFlippedThisEdge = false;

    [Header("Bounds")]
    public SpriteRenderer background;
    private float leftBound;
    private float rightBound;

    [Header("Systems")]
    public PlayerManager player;
    public CreditManager creditManager;
    public LevelManager levelManager;

    [Header("Counts")]
    public int totalAliens;
    public int aliveAliens;

    private Rigidbody2D rb;
    private bool isResetting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = baseSpeed;

        if (background != null)
        {
            Bounds b = background.bounds;
            leftBound = b.min.x;
            rightBound = b.max.x;
        }

        ResetAliens();
    }

    void Update()
    {
        if (isResetting) return;

        MoveFormation();
    }

    // ==========================
    // MOVEMENT
    // ==========================
    void MoveFormation()
    {
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

        Vector2 horizontalMove = new Vector2(direction.x * moveStep, 0f);
        Vector3 verticalMove = Vector3.zero;

        if (nextRight > rightBound || nextLeft < leftBound)
        {
            if (!hasFlippedThisEdge)
            {
                direction *= -1f;
                transform.position += Vector3.down * stepDown;
                hasFlippedThisEdge = true;
            }
        }
        else
        {
            hasFlippedThisEdge = false;
        }

        rb.MovePosition(rb.position + horizontalMove);
        transform.position += verticalMove;
    }

    void UpdateSpeed()
    {
        if (totalAliens <= 0) return;

        float progress = 1f - ((float)aliveAliens / totalAliens);
        float curved = Mathf.Pow(progress, speedExponent);

        speed = Mathf.Lerp(
            baseSpeed,
            GameManager.Instance.maxAlienSpeed,
            curved
        );
    }

    // ==========================
    // GAME FLOW
    // ==========================
    public void AlienKilled()
    {
        aliveAliens = Mathf.Max(0, aliveAliens - 1);

        UpdateSpeed();

        if (aliveAliens <= 0)
        {
            AllAliensKilled();
        }
    }

    void AllAliensKilled()
    {
        int reward = Mathf.Max(
            10,
            Mathf.RoundToInt(Mathf.Pow(totalAliens, 1.2f) * 0.5f)
        );

        creditManager.AddCredits(reward);

        // 20% chance to drop a random item
        ItemData droppedItem = null;
        if (Random.value < 0.20f)
        {
            ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
            if (allItems.Length > 0)
            {
                droppedItem = allItems[Random.Range(0, allItems.Length)];
                GameManager.Instance.AddItem(droppedItem);
            }
        }

        levelManager.CompleteLevel(reward, droppedItem);
    }

    public void ResetAliens()
    {
        totalAliens = transform.childCount;
        aliveAliens = totalAliens;

        speed = baseSpeed;
        UpdateSpeed();
    }

    public void OnAliensReachedPlayer()
    {
        if (isResetting) return;

        StartCoroutine(ResetAfterHit());
    }

    IEnumerator ResetAfterHit()
    {
        isResetting = true;

        enabled = false;
        player.enabled = false;

        player.DestroyBullet();
        player.TakeDamage();

        transform.position += Vector3.up * (5f * stepDown);

        yield return new WaitForSeconds(1f);

        player.enabled = true;
        enabled = true;
        isResetting = false;
    }
}