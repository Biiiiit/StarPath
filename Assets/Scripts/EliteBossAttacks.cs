using UnityEngine;

public class EliteBossAttacks : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;

    public Transform player;

    [Header("Spread Settings")]
    public float maxSpreadAngle = 45f; // hoe ver kogels kunnen afwijken

    [Header("Fire Rate")]
    public float fireRate = 1f;
    private float nextFireTime;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }

    }

    void Shoot()
    {
        // Richting naar speler
        Vector2 directionToPlayer = (player.position - firePoint.position).normalized;

        Vector2 finalDirection;

        // 50% kans
        if (Random.value < 0.5f)
        {
            // DIRECT op speler
            finalDirection = directionToPlayer;
        }
        else
        {
            // RANDOM afwijking
            float randomAngle = Random.Range(-maxSpreadAngle, maxSpreadAngle);
            finalDirection = RotateVector(directionToPlayer, randomAngle);
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        BossBullet bullet = projectile.GetComponent<BossBullet>();
        bullet.SetDirection(finalDirection);
    }

    Vector2 RotateVector(Vector2 v, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;

        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
}