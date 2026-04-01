using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 1f;
    public Vector2 direction = Vector2.down;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerBossManager>();
            if (player != null)
            {
                player.TakeDamage();
            }

            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}