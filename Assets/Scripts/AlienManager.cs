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
    private Rigidbody2D rb;

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

        UpdateSpeed();

        MoveFormation();
    }

    void UpdateSpeed()
    {
        if (totalAliens <= 0) return;

        // Exponential curve: slow start, fast near the end
        float progress = 1f - ((float)aliveAliens / (float)totalAliens);
        float curved = Mathf.Pow(progress, speedExponent);

        speed = Mathf.Lerp(baseSpeed, maxSpeed, curved);
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
            SpriteRenderer sr = alien.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = state;
        }

        SpriteRenderer playerSR = player.GetComponent<SpriteRenderer>();
        if (playerSR != null) playerSR.enabled = state;
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

        Debug.Log($"ResetAliens -> Total: {totalAliens}, Alive: {aliveAliens}");
    }

    public void AlienKilled()
    {
        if (totalAliens <= 0) return;

        aliveAliens = Mathf.Max(aliveAliens - 1, 0);
    }

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

        if (nextRight > rightBound || nextLeft < leftBound)
        {
            direction *= -1;
            transform.position += Vector3.down * 0.5f;
            return;
        }

        rb.MovePosition(rb.position + new Vector2(direction.x * moveStep, 0));
    }
}