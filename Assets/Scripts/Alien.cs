using UnityEngine;

public class Alien : MonoBehaviour
{
    private AlienManager manager;
    public Animator animator;
    public AudioClip deathSound;

    private bool isDying = false;

    void Start()
    {
        manager = GetComponentInParent<AlienManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            OnHit();
        }
    }

    public void OnHit()
    {
        if (isDying) return;
        isDying = true;
        manager.AlienKilled();
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("Die");
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void PlayDeathSound()
    {
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
    }
}