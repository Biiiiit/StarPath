using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 50;
    private int currentHealth;

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
    private float timer;

    [Header("Bounds")]
    public SpriteRenderer background;

    private float leftBound;
    private float rightBound;

    private PlayerBossManager player;

    private int currentRow = 0;

    void Start()
    {
        player = FindFirstObjectByType<PlayerBossManager>();
        currentHealth = maxHealth;

        Bounds bounds = background.bounds;
        leftBound = bounds.min.x;
        rightBound = bounds.max.x;

        timePerRow = cellSize / bulletPrefab.GetComponent<BossBullet>().speed;

        SetPhase(1);
    }

    void Update()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    void SetPhase(int phase)
    {
        currentPhase = phase;

        switch (phase)
        {
            case 1:
                currentPatterns = phase1Patterns;
                break;
            case 2:
                currentPatterns = phase2Patterns;
                break;
            case 3:
                currentPatterns = phase3Patterns;
                break;
        }

        lastPatternIndex = -1;
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

    IEnumerator FireRow(AttackPattern pattern)
    {
        int y = currentRow;
        int flippedY = pattern.height - 1 - y;

        for (int x = 0; x < pattern.width; x++)
        {
            int index = flippedY * pattern.width + x;

            if (!pattern.grid[index]) continue;

            float step = (rightBound - leftBound) / pattern.width;
            float xPos = leftBound + step * (x + 0.5f);

            Vector3 spawnPos = new Vector3(xPos, transform.position.y, 1);

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            bullet.GetComponent<BossBullet>().SetDirection(Vector2.down);

            yield return null;
        }
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
}