using UnityEngine;
using System.Collections.Generic;

public class Bullet2 : MonoBehaviour
{
    public float speed = 8f;
    public SpriteRenderer background;

    public int maxHits = 1;
    private int hitsRemaining;

    private PlayerManager2 player;

    // Track aliens already hit (prevents duplicate hits)
    private HashSet<Alien> hitAliens = new HashSet<Alien>();

    void Start()
    {
        player = FindFirstObjectByType<PlayerManager>();
        hitsRemaining = maxHits;
    }

    void Update()
    {
        float moveStep = speed * Time.deltaTime;

        // Raycast forward BEFORE moving
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, moveStep);

        if (hit.collider != null && hit.collider.CompareTag("Alien"))
        {
            Alien alien = hit.collider.GetComponent<Alien>();

            if (alien != null && !hitAliens.Contains(alien))
            {
                hitAliens.Add(alien);

                alien.OnHit();

                hitsRemaining--;

                if (hitsRemaining <= 0)
                {
                    ClearAndDestroy();
                    return;
                }
            }
        }

        // Move bullet
        transform.Translate(Vector2.up * moveStep);

        CheckBounds();

        // Debug (optional)
        Debug.DrawRay(transform.position, Vector2.up * moveStep, Color.red);
    }

    void CheckBounds()
    {
        if (background == null) return;

        if (transform.position.y > background.bounds.max.y)
        {
            ClearAndDestroy();
        }
    }

    void ClearAndDestroy()
    {
        if (player != null)
        {
            player.ClearBullet();
        }

        Destroy(gameObject);
    }
}