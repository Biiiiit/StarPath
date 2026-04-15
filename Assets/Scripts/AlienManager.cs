using System;
using System.Collections;
using UnityEngine;

public class AlienManager : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 1f;
    [Tooltip("Exponent for speed curve: >1 = slow start, <1 = fast start")]
    public float speedExponent = 1f;

    private float speed;
    private Vector3 direction = Vector3.right;

    [Header("References")]
    public SpriteRenderer background;

    private float leftBound;
    private float rightBound;

    [Header("Internal Counts")]
    [HideInInspector] public int totalAliens;
    [HideInInspector] public int aliveAliens;

    [Header("Player Collision")]
    public PlayerManager player;
    private bool isResetting = false;
    private Rigidbody2D rb;

    [Header("Game Systems")]
    public CreditManager creditManager;
    public LevelManager levelManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = baseSpeed;

        if (background != null)
        {
            Bounds bounds = background.bounds;
            leftBound = bounds.min.x;
            rightBound = bounds.max.x;
        }

        ResetAliens();
    }

    void Update()
    {
        if (isResetting) return;

        MoveFormation();
    }

    void UpdateSpeed()
    {
        if (totalAliens <= 0) return;

        // Exponential curve: slow start, fast near the end
        float progress = 1f - ((float)aliveAliens / (float)totalAliens);
        float curved = Mathf.Pow(progress, speedExponent);

        speed = Mathf.Lerp(baseSpeed, GameManager.Instance.maxAlienSpeed, curved);
        Debug.Log("Alien Speed " + speed);
    }

    public void OnAliensReachedPlayer()
    {
        if (isResetting) return;
        isResetting = true;

        player.DestroyBullet();
        player.TakeDamage();

        StartCoroutine(ResetAfterHit());
    }

    IEnumerator ResetAfterHit()
    {
        enabled = false;
        player.enabled = false;

        // Move aliens up 5 rows
        transform.position += Vector3.up * (5f * 0.5f);

        // Blink for 1 second
        float duration = 1f;
        float timer = 0f;
        while (timer < duration)
        {
            ToggleRenderers(false);
            yield return new WaitForSeconds(0.1f);
            ToggleRenderers(true);
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }

        player.enabled = true;
        enabled = true;
        isResetting = false;
    }

    void ToggleRenderers(bool state)
    {
        foreach (Transform alien in transform)
        {
            if (alien == null) continue;

            SpriteRenderer[] alienRenderers =
                alien.GetComponentsInChildren<SpriteRenderer>(true);

            foreach (SpriteRenderer sr in alienRenderers)
            {
                sr.enabled = state;
            }
        }

        SpriteRenderer[] playerRenderers =
            player.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in playerRenderers)
        {
            sr.enabled = state;
        }
    }

    public void AlienKilled()
    {
        if (totalAliens <= 0) return;

        aliveAliens = Mathf.Max(aliveAliens - 1, 0);

        UpdateSpeed(); // update speed only here

        if (aliveAliens <= 0)
        {
            OnAllAliensKilled();
        }
    }

    public void ResetAliens()
    {
        totalAliens = 0;
        foreach (Transform t in transform)
        {
            if (t.gameObject.activeSelf)
                totalAliens++;
        }

        aliveAliens = totalAliens;
        speed = baseSpeed;
        UpdateSpeed(); // also update speed on reset
    }
    void OnAllAliensKilled()
    {
        float multiplier = 0.5f;

        int reward = Mathf.Max(
            10,
            Mathf.RoundToInt(Mathf.Pow(totalAliens, 1.2f) * multiplier)
        );

        creditManager.AddCredits(reward);
        levelManager.CompleteLevel();
    }

    void MoveFormation()
    {
        float moveStep = speed * Time.deltaTime;
        float nextLeft = float.MaxValue;
        float nextRight = float.MinValue;

        // Find predicted horizontal edges
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

        Vector2 horizontalMove = new Vector2(direction.x * moveStep, 0);
        Vector3 verticalMove = Vector3.zero;

        // Check edge and move down once if needed
        if (nextRight > rightBound || nextLeft < leftBound)
        {
            verticalMove = Vector3.down * 0.5f; // move down
            direction *= -1;                     // flip direction
        }

        // Apply movement every frame
        rb.MovePosition(rb.position + horizontalMove);
        transform.position += verticalMove;
    }
}