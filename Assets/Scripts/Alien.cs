using UnityEngine;

public class Alien : MonoBehaviour
{
    public AlienManager manager;

    public Animator animator;
    public AudioClip deathSound;

    private bool isDying = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            OnHit();
        }
        if (other.CompareTag("Death"))
        {
            manager.OnAliensReachedPlayer();
        }
    }

    public void OnHit()
    {
        if (isDying) return;
        isDying = true;

        if (manager != null)
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