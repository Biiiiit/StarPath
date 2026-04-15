using UnityEngine;
using UnityEngine.UI;

public class CoverHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Image healthBarFill;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check of het een boss bullet is
        BossBullet bullet = other.GetComponent<BossBullet>();

        if (bullet != null)
        {
            TakeDamage(10f); // of eigen damage waarde
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }
}