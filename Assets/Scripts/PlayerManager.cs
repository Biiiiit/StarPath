using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public SpriteRenderer background;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public AudioClip shootSound;
    public Animator animator;

    private GameObject currentBullet;

    public LivesUI livesUI;
    public GameObject gameOverUI;
    private bool isInvulnerable = false;
    public float invulnerableDuration = 1f;


    void Update()
    {
        float move = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            move = -1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            move = 1f;

        transform.Translate(Vector2.right * move * GameManager.Instance.moveSpeed * Time.deltaTime);

        if (Keyboard.current.spaceKey.isPressed && currentBullet == null)
        {
            currentBullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

            if (animator != null)
            {
                animator.SetTrigger("Shoot");
            }

            if (shootSound != null)
            {
                AudioSource.PlayClipAtPoint(shootSound, transform.position);
            }
        }

        ClampToBackground();
    }

    public void ClearBullet()
    {
        currentBullet = null;
    }

    public void DestroyBullet()
    {
        if (currentBullet != null)
        {
            Destroy(currentBullet);
            currentBullet = null;
        }
    }

    void ClampToBackground()
    {
        Bounds bounds = background.bounds;

        float halfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;

        float clampedX = Mathf.Clamp(
            transform.position.x,
            bounds.min.x + halfWidth,
            bounds.max.x - halfWidth
        );

        transform.position = new Vector3(
            clampedX,
            transform.position.y,
            transform.position.z
        );
    }

    public void TakeDamage()
    {
        if (isInvulnerable) return;

        livesUI.LoseLife();
        GameManager.Instance.lives--;

        DestroyBullet();

        if (GameManager.Instance.lives <= 0)
        {
            Time.timeScale = 0f;
            gameOverUI.SetActive(true);
            return;
        }

        StartCoroutine(Invulnerability());
    }

    IEnumerator Invulnerability()
    {
        isInvulnerable = true;

        SpriteRenderer[] renderers =
            GetComponentsInChildren<SpriteRenderer>();

        float timer = 0f;

        while (timer < invulnerableDuration)
        {
            foreach (SpriteRenderer sr in renderers)
                sr.enabled = false;

            yield return new WaitForSeconds(0.1f);

            foreach (SpriteRenderer sr in renderers)
                sr.enabled = true;

            yield return new WaitForSeconds(0.1f);

            timer += 0.2f;
        }

        isInvulnerable = false;
    }

    public void GainLife()
    {
        livesUI.GainLife();
        GameManager.Instance.lives++;
    }
}