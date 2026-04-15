using UnityEngine;
using UnityEngine.UI;

public class EliteBoss : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Image healthBarFill;

    [Header("Effects")]
    public GameObject hitEffect;
    public GameObject deathEffect;
    public AudioSource audioSource;
    public AudioClip deathSound;

    [Header("Bounds")]
    public SpriteRenderer background;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 5f;
    public float fireRate = 1f;
    public float spreadAngle = 30f;

    private float nextShootTime;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        HandleShooting();
        CheckBounds();
    }

    // ---------------- SHOOTING ----------------
    void HandleShooting()
    {
        if (Time.time < nextShootTime) return;

        Shoot();
        nextShootTime = Time.time + fireRate;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Transform player = FindFirstObjectByType<PlayerManager>()?.transform;
        if (player == null) return;

        Vector2 baseDir = (player.position - firePoint.position).normalized;

        // 50% straight, 50% spread
        Vector2 finalDir;

        if (Random.value < 0.5f)
        {
            finalDir = baseDir;
        }
        else
        {
            float angle = Random.Range(-spreadAngle, spreadAngle);
            finalDir = Rotate(baseDir, angle);
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        BossBullet bb = bullet.GetComponent<BossBullet>();
        if (bb != null)
        {
            bb.speed = bulletSpeed;
            bb.SetDirection(finalDir);
        }
    }

    Vector2 Rotate(Vector2 v, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(
            cos * v.x - sin * v.y,
            sin * v.x + cos * v.y
        );
    }

    // ---------------- DAMAGE ----------------
    public void TakeDamage(float damage, Vector3 hitPos)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (hitEffect != null)
        {
            Instantiate(hitEffect, hitPos, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        Destroy(gameObject);
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    // ---------------- BOUNDS ----------------
    void CheckBounds()
    {
        if (background == null) return;

        Vector3 pos = transform.position;

        if (pos.x < background.bounds.min.x - 5f ||
            pos.x > background.bounds.max.x + 5f ||
            pos.y < background.bounds.min.y - 5f ||
            pos.y > background.bounds.max.y + 5f)
        {
            Destroy(gameObject);
        }
    }
}