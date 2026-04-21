using UnityEngine;

public class AlienBullet : MonoBehaviour
{
    public float speed = 60f;

    private Vector2 direction = Vector2.down;

    public GameObject hitEffectPrefab;

    private bool dying = false;

    private PlayerManager player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerManager>();
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        if (dying) return;

        transform.Translate(direction * speed * Time.deltaTime);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (dying) return;

        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            player.TakeDamage();
            Destroy(gameObject);
            return;
        }
    }

    public void HitByPlayerBullet()
    {
        SpawnHitEffect();
        DestroySelf();
    }

    void SpawnHitEffect()
    {
        if (hitEffectPrefab == null) return;

        GameObject effect = Instantiate(
            hitEffectPrefab,
            transform.position,
            Quaternion.identity
        );

        Destroy(effect, 0.5f);
    }

    void DestroySelf()
    {
        if (dying) return;

        dying = true;
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        if (!dying)
            Destroy(gameObject);
    }
}