using UnityEngine;

public class Bullet2 : MonoBehaviour
{
    public float speed = 8f;
    public Vector2 direction = Vector2.right;

    private PlayerManager2 player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerManager2>();
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
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