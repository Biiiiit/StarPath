using UnityEngine;

public class Alien : MonoBehaviour
{
    private AlienManager manager;

    void Start()
    {
        manager = GetComponentInParent<AlienManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            manager.AlienKilled();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}