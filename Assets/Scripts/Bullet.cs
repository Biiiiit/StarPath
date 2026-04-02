using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public SpriteRenderer background;

    private int hitsRemaining;
    private PlayerManager player;

    // Track aliens already hit
    private HashSet<Alien> hitAliens = new HashSet<Alien>();

    void Start()
    {
        player = FindFirstObjectByType<PlayerManager>();
        hitsRemaining = GameManager.Instance.bulletPierce;
    }

    void Update()
    {
        float moveStep = GameManager.Instance.bulletSpeed * Time.deltaTime;

        // Raycast forward BEFORE moving
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, moveStep);

        if (hit.collider != null)
        {
            // 🔹 ALIEN HIT
            if (hit.collider.CompareTag("Alien"))
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
            else if (hit.collider.CompareTag("Boss"))
            {
                Boss boss = hit.collider.GetComponent<Boss>();

                if (boss != null)
                {
                    Vector2 offset = Random.insideUnitCircle * 0.2f;
                    boss.TakeDamage(1, (Vector3)(hit.point + offset));

                    hitsRemaining--;

                    if (hitsRemaining <= 0)
                    {
                        ClearAndDestroy();
                        return;
                    }
                }
            }
        }

        // Move bullet
        transform.Translate(Vector2.up * moveStep);

        CheckBounds();

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