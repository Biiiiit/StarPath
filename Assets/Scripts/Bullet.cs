using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public SpriteRenderer background;

    private PlayerManager player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerManager>();
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        CheckBounds();
    }

    void CheckBounds()
    {
        if (background == null) return;

        Bounds bounds = background.bounds;

        // If bullet goes above the background
        if (transform.position.y > bounds.max.y)
        {
            if (player != null)
            {
                player.ClearBullet();
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Alien"))
        {
            if (player != null)
            {
                player.ClearBullet();
            }

            Destroy(gameObject);
        }
    }
}