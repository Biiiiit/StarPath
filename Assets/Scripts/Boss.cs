using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 50;
    private int currentHealth;
    public AudioClip deathSound;
    private int currentPhase = 1;

    [Header("Phase Patterns")]
    public AttackPattern[] phase1Patterns;
    public AttackPattern[] phase2Patterns;
    public AttackPattern[] phase3Patterns;

    private AttackPattern[] currentPatterns;
    private int lastPatternIndex = -1;

    [Header("Attack Timing")]
    public float attackCooldown = 3f;
    private bool isAttacking = false;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public float cellSize = 1f;
    private float timePerRow;
    private Transform playerTransform;

    [Header("Bounds")]
    public SpriteRenderer background;

    private float leftBound;
    private float rightBound;

    private PlayerBossManager player;

    private int currentRow = 0;

    void Start()
    {
        player = FindFirstObjectByType<PlayerBossManager>();
        playerTransform = player.transform;
        currentHealth = maxHealth;

        Bounds bounds = background.bounds;
        leftBound = bounds.min.x;
        rightBound = bounds.max.x;

        timePerRow = cellSize / bulletPrefab.GetComponent<BossBullet>().speed;

        StartCoroutine(AttackRoutine()); // start once
    }

    void SetPhase(int phase)
    {
        currentPhase = phase;

        StopAllCoroutines(); // stop old attacks

        if (phase == 3)
        {
            StartCoroutine(Phase3Attack());
            return;
        }

        switch (phase)
        {
            case 1:
                currentPatterns = phase1Patterns;
                break;
            case 2:
                currentPatterns = phase2Patterns;
                break;
        }

        lastPatternIndex = -1;
        isAttacking = false;
    }

    int GetRandomPatternIndex()
    {
        if (currentPatterns.Length <= 1)
            return 0;

        int index;

        do
        {
            index = Random.Range(0, currentPatterns.Length);
        }
        while (index == lastPatternIndex);

        return index;
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (currentPhase == 3)
            {
                yield return new WaitForSeconds(attackCooldown);
                continue;
            }

            isAttacking = true;

            int patternIndex = GetRandomPatternIndex();
            lastPatternIndex = patternIndex;

            AttackPattern pattern = currentPatterns[patternIndex];

            currentRow = 0;

            while (currentRow < pattern.height)
            {
                yield return StartCoroutine(FireRow(pattern));
                currentRow++;
                yield return new WaitForSeconds(timePerRow);
            }

            yield return new WaitForSeconds(attackCooldown);

            isAttacking = false;
        }
    }

    IEnumerator FireRow(AttackPattern pattern)
    {
        int y = currentRow;
        int flippedY = pattern.height - 1 - y;

        float step = (rightBound - leftBound) / pattern.width;

        for (int x = 0; x < pattern.width; x++)
        {
            int index = flippedY * pattern.width + x;

            if (!pattern.grid[index]) continue;

            float xPos = leftBound + step * (x + 0.5f);

            Vector3 spawnPos = new Vector3(xPos, transform.position.y, 1);

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            bullet.GetComponent<BossBullet>().SetDirection(Vector2.down);
        }

        yield return null;
    }

    IEnumerator Phase3Attack()
    {
        while (currentPhase == 3)
        {
            int attackType = Random.Range(0, 3);

            switch (attackType)
            {
                case 0:
                    yield return StartCoroutine(AimedBurst());
                    break;

                case 1:
                    yield return StartCoroutine(SpiralAttack());
                    break;

                case 2:
                    yield return StartCoroutine(RadialBurst());
                    break;
            }

            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator AimedBurst()
    {
        int shots = 5;

        for (int i = 0; i < shots; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(leftBound, rightBound),
                transform.position.y,
                1
            );

            Vector2 baseDir = (playerTransform.position - spawnPos).normalized;

            for (int spread = -1; spread <= 1; spread++)
            {
                Vector2 dir = Quaternion.Euler(0, 0, spread * 10f) * baseDir;

                GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                bullet.GetComponent<BossBullet>().SetDirection(dir);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SpiralAttack()
    {
        int bullets = 20;
        float angleStep = 20f;
        float currentAngle = Random.Range(0f, 360f);

        Vector3 center = transform.position;

        for (int i = 0; i < bullets; i++)
        {
            float angle = currentAngle + i * angleStep;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject bullet = Instantiate(bulletPrefab, center, Quaternion.identity);
            bullet.GetComponent<BossBullet>().SetDirection(dir);

            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator RadialBurst()
    {
        int bullets = 24;

        Vector3 center = transform.position;

        for (int i = 0; i < bullets; i++)
        {
            float angle = (360f / bullets) * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject bullet = Instantiate(bulletPrefab, center, Quaternion.identity);
            bullet.GetComponent<BossBullet>().SetDirection(dir);
        }

        yield return new WaitForSeconds(0.3f);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        float healthPercent = (float)currentHealth / maxHealth;

        if (healthPercent <= 0.33f && currentPhase < 3)
        {
            SetPhase(3);
        }
        else if (healthPercent <= 0.66f && currentPhase < 2)
        {
            SetPhase(2);
        }

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

    public void PlayDeathSound()
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
    }
}