using UnityEngine;

public class Alien : MonoBehaviour
{
    private AlienManager2 manager;

    void Start()
    {
        manager = GetComponentInParent<AlienManager2>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (manager != null)
            {
                manager.AlienKilled();
            }

            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}