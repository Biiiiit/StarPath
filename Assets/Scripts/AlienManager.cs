using System;
using System.Collections;
using UnityEngine;

public class AlienManager : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 1f;
    public float maxSpeed = 3f;
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
        if (isResetting) return;

        MoveFormation();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isResetting) return;

        if (other.CompareTag("Death"))
        {
            Debug.Log("player hit");
            OnAliensReachedPlayer();
        }
    }

    public void OnAliensReachedPlayer()
    {
        if (isResetting) return;

        isResetting = true;

        player.TakeDamage();
        StartCoroutine(ResetAfterHit());
    }

    IEnumerator ResetAfterHit()
    {
        // Disable movement
        enabled = false;

        // Move aliens UP 5 steps (adjust with grid spacing)
        transform.position += Vector3.up * (5f * 0.5f);

        // Disable player movement
        player.enabled = false;

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

        // Re-enable movement
        player.enabled = true;
        enabled = true;

        isResetting = false; // allow triggering again
    }

    void ToggleRenderers(bool state)
    {
        // Toggle aliens
        foreach (Transform alien in transform)
        {
            if (alien == null) continue;

            SpriteRenderer sr = alien.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.enabled = state;
        }

        // Toggle player
        SpriteRenderer playerSR = player.GetComponent<SpriteRenderer>();
        if (playerSR != null)
            playerSR.enabled = state;
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

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.MovePosition(rb.position + new Vector2(direction.x * moveStep, 0));
    }
}