using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float fireRate = 1f;

    [Header("Bounds")]
    public SpriteRenderer background;

    private float leftBound;
    private float rightBound;

    private float fireTimer;
    private PlayerBossManager player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerBossManager>();
        currentHealth = maxHealth;

        Bounds bounds = background.bounds;
        leftBound = bounds.min.x;
        rightBound = bounds.max.x;
    }

    void Update()
    {
        HandleShooting();
    }

    void HandleShooting()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            StartCoroutine(FireBurst());
        }
    }

    IEnumerator FireBurst()
    {
        int burstCount = Random.Range(3, 6);

        for (int i = 0; i < burstCount; i++)
        {
            float x;

            if (Random.value > 0.5f && player != null)
            {
                x = player.transform.position.x;
            }
            else
            {
                x = Random.Range(leftBound, rightBound);
            }

            Vector3 pos = new Vector3(x, transform.position.y, -1);

            GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);

            bullet.GetComponent<BossBullet>().SetDirection(Vector2.down);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss defeated");
        Destroy(gameObject);
    }
}