using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 75;
    private int currentHealth;
    public AudioClip deathSound;
    private int currentPhase = 1;

    [Header("Hit Effect")]
    public GameObject hitEffectPrefab;
    public Image healthBarFill;

    [Header("Phase Patterns")]
    public AttackPattern[] phase2Patterns;

    private AttackPattern[] currentPatterns;
    private int lastPatternIndex = -1;

    [Header("Attack Timing")]
    public float attackCooldown = 3f;
    private bool isAttacking = false;

    [Header("Shooting")]
    public GameObject bulletPrefab;
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

        healthBarFill.fillAmount = 1f;
        UpdateHealthBar();

        SetPhase(1);
    }

    void Update()
    {
        if (currentPhase == 3) return;

        if (currentPhase == 1)
        {
            if (!isAttacking)
                StartCoroutine(Phase1Attack());
            return;
        }

        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator Phase1Attack()
    {
        isAttacking = true;

        bool fireRows = Random.value > 0.7f;

        if (!fireRows)
        {
            // 🔹 Single bullet
            yield return StartCoroutine(FireSingleShot());
        }
        else
        {
            // 🔹 4 rows of single bullets
            for (int i = 0; i < 4; i++)
            {
                yield return StartCoroutine(FireSingleShot());
                yield return new WaitForSeconds(timePerRow);
            }
        }

        yield return new WaitForSeconds(0.25f);

        isAttacking = false;
    }

    IEnumerator FireSingleShot()
    {
        float xPos;

        xPos = Random.Range(leftBound, rightBound);


        Vector3 spawnPos = new Vector3(xPos, transform.position.y, 1);

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        bullet.GetComponent<BossBullet>().SetDirection(Vector2.down);

        yield return null;
    }

    void SetPhase(int phase)
    {
        StopAllCoroutines();

        currentPhase = phase;
        lastPatternIndex = -1;

        if (phase == 2)
        {
            currentPatterns = phase2Patterns;
            StartCoroutine(PhaseTransitionDelay());
        }
        else if (phase == 3)
        {
            StartCoroutine(StartPhase3());
        }
    }

    IEnumerator PhaseTransitionDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator StartPhase3()
    {
        isAttacking = true;

        yield return new WaitForSeconds(attackCooldown);

        StartCoroutine(Phase3Attack());
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

            yield return new WaitForSeconds(1f);
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

        Vector3 center = new Vector3(
        transform.position.x,
        transform.position.y,
        1
        );

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

        Vector3 center = new Vector3(
        transform.position.x,
        transform.position.y,
        1
        );

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

    public void TakeDamage(int dmg, Vector3 hitPosition)
    {
        currentHealth -= dmg;

        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);
            Destroy(effect, 0.5f);
        }

        StartCoroutine(HitFlash());
        UpdateHealthBar();

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

    IEnumerator HitFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    void Die()
    {
        Debug.Log("Boss defeated");

        BossBullet[] bullets = FindObjectsByType<BossBullet>(FindObjectsSortMode.None);

        Destroy(gameObject);

        foreach (BossBullet bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
    }

    public void PlayDeathSound()
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
    }

    void UpdateHealthBar()
    {
        float healthPercent = (float)currentHealth / maxHealth;

        healthBarFill.fillAmount = healthPercent;

        Color color;

        if (healthPercent > 0.5f)
        {
            float t = (healthPercent - 0.5f) / 0.5f;
            color = Color.Lerp(Color.yellow, Color.green, t);
        }
        else
        {
            float t = healthPercent / 0.5f;
            color = Color.Lerp(Color.red, Color.yellow, t);
        }

        healthBarFill.color = color;
    }
}