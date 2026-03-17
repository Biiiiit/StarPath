using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;

    private PlayerManager player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerManager>();
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnBecameInvisible()
    {
        if (player != null)
        {
            player.ClearBullet();
        }

        Destroy(gameObject);
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