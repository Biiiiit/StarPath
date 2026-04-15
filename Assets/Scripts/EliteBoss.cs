using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EliteBoss : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 60f;
    private float currentHealth;

    [Header("Effects")]
    public AudioClip deathSound;
    public GameObject hitEffectPrefab;

    [Header("UI")]
    public Image healthBarFill;

    [Header("Attack Timing")]
    public float attackCooldown = 3f;
    private float nextAttackTime;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public float spreadAngle = 30f;

    [Header("Bounds")]
    public SpriteRenderer background;

    private SpriteRenderer[] renderers;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        renderers = GetComponentsInChildren<SpriteRenderer>();

        Debug.Log("ELITE BOSS SPAWNED");
    }

    void Update()
    {
        HandleShooting();
        CheckBounds();
    }

    public void TakeDamage(int dmg, Vector3 hitPosition)
    {
        Debug.Log("ELITE BOSS TAKE DAMAGE CALLED");

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("CURRENT HP: " + currentHealth);

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);
        }

        StartCoroutine(HitFlash());
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator HitFlash()
    {
        Debug.Log("FLASH START");

        if (renderers == null || renderers.Length == 0)
        {
            Debug.Log("NO RENDERERS FOUND");
            yield break;
        }

        Color[] originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;

            originalColors[i] = renderers[i].color;

            renderers[i].material = new Material(Shader.Find("Sprites/Default"));
            renderers[i].color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;

            renderers[i].color = originalColors[i];
        }

        Debug.Log("FLASH END");
    }

    void Die()
    {
        Debug.Log("ELITE BOSS DIED");

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
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

    void HandleShooting()
    {
        if (Time.time < nextAttackTime) return;

        Shoot();
        nextAttackTime = Time.time + attackCooldown;
    }

    void Shoot()
    {
        Transform player = FindFirstObjectByType<PlayerManager>()?.transform;
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        BossBullet bb = bullet.GetComponent<BossBullet>();
        if (bb != null)
        {
            bb.speed = bulletSpeed;
            bb.SetDirection(dir);
        }
    }

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