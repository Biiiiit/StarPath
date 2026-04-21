using UnityEngine;
using System.Collections;

public enum AlienType
{
    Basic,
    ZigZag,
    Shooter,
    Tank
}

public class Alien : MonoBehaviour
{
    public AlienType type = AlienType.Basic;

    public AlienManager manager;
    public AudioClip deathSound;

    [Header("Stats")]
    public int maxHealth = 1;
    private int health = 1;
    private static float globalShootCooldown = 3f;
    private static float nextShootTime = 0f;
    [Header("Shooting")]
    public GameObject enemyBulletPrefab;

    [Header("Visual")]
    public Animator animator;

    private bool dying = false;

    private Vector3 baseLocalPos;
    AlienManager alienManager;

    void Start()
    {
        alienManager = FindFirstObjectByType<AlienManager>();
        baseLocalPos = transform.localPosition;

        SetupAlien();
    }

    void SetupAlien()
    {
        switch (type)
        {
            case AlienType.Basic:
                maxHealth = 1;
                break;

            case AlienType.ZigZag:
                maxHealth = 1;
                break;

            case AlienType.Shooter:
                maxHealth = 1;
                break;

            case AlienType.Tank:
                maxHealth = 4;
                transform.localScale = Vector3.one * 2f;
                break;
        }

        health = maxHealth;
    }

    void Update()
    {
        if (type == AlienType.ZigZag)
        {
            float speedFactor = 6f;

            // direction-aware phase shift
            float directionOffset = (alienManager.Direction.x >= 0f) ? 0f : Mathf.PI;

            float wave =
                Mathf.Sin((Time.time * speedFactor) + directionOffset) * 0.1f;

            transform.localPosition = new Vector3(
                baseLocalPos.x + wave,
                baseLocalPos.y,
                0f
            );
        }
        TryShoot();
    }

    void TryShoot()
    {
        if (type != AlienType.Shooter) return;
        if (enemyBulletPrefab == null) return;
        if (Time.time < nextShootTime) return;

        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.6f;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 200f);

        if (hit.collider == null) return;

        Debug.Log("Hit: " + hit.collider.name);

        if (hit.collider.CompareTag("PlayerHit"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        nextShootTime = Time.time + globalShootCooldown;

        Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Hit();
        }

        if (other.CompareTag("Death"))
        {
            manager.OnAliensReachedPlayer();
        }
    }

    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
    }

    IEnumerator HitFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null) yield break;

        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    public void Hit()
    {
        if (dying) return;

        health--;

        if (type == AlienType.Tank)
        {
            StartCoroutine(HitFlash());
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        dying = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (manager != null)
            manager.AlienKilled();

        if (animator != null)
            animator.SetTrigger("Die");
        else
            Destroy(gameObject);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}