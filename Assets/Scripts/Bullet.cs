using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public SpriteRenderer background;

    private int hitsRemaining;
    private PlayerManager player;

    private HashSet<Alien> hitAliens = new HashSet<Alien>();

    void Start()
    {
        player = FindFirstObjectByType<PlayerManager>();
        hitsRemaining = GameManager.Instance.bulletPierce;
    }

    void Update()
    {
        float moveStep = GameManager.Instance.bulletSpeed * Time.deltaTime;

        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position,
            0.1f,
            Vector2.up,
            moveStep + 0.2f
        );

        Debug.DrawRay(transform.position, Vector2.up * (moveStep + 0.2f), Color.green);

        if (hit.collider != null)
        {

            Alien alien = hit.collider.GetComponent<Alien>();
            if (alien != null)
            {
                if (!hitAliens.Contains(alien))
                {
                    hitAliens.Add(alien);

                    alien.Hit();
                    HandleHit();
                    return;
                }
            }
            AlienBullet bullet = hit.collider.GetComponent<AlienBullet>();

            if (bullet != null)
            {
                bullet.HitByPlayerBullet();
                HandleHit();
                return;
            }

            EliteBoss eliteBoss = hit.collider.GetComponentInParent<EliteBoss>();
            if (eliteBoss != null)
            {

                Vector2 offset = Random.insideUnitCircle * 0.2f;
                eliteBoss.TakeDamage(1, (Vector3)(hit.point + offset));

                HandleHit();
                return;
            }

            Boss boss = hit.collider.GetComponentInParent<Boss>();
            if (boss != null)
            {

                Vector2 offset = Random.insideUnitCircle * 0.2f;
                boss.TakeDamage(1, (Vector3)(hit.point + offset));

                HandleHit();
                return;
            }

            CoverHealth cover = hit.collider.GetComponent<CoverHealth>();
            if (cover != null)
            {
                cover.TakeDamage(1f);

                HandleHit();
                return;
            }
        }

        transform.Translate(Vector2.up * moveStep);

        CheckBounds();
    }

    void HandleHit()
    {
        hitsRemaining--;

        if (hitsRemaining <= 0)
        {
            ClearAndDestroy();
        }
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