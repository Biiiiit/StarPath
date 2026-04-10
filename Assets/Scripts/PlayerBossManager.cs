using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerBossManager : MonoBehaviour
{
    public SpriteRenderer background;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public AudioClip shootSound;

    private List<GameObject> currentBullets = new List<GameObject>();

    public LivesUI livesUI;
    public GameObject gameOverUI;
    private bool isInvulnerable = false;
    public float invulnerableDuration = 1f;

    private int bulletsFired = 0;
    private bool isReloading = false;

    private float lastShotTime = 0f;
    private float reloadEndTime = 0f;

    void Update()
    {
        float move = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            move = -1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            move = 1f;

        transform.Translate(Vector2.right * move * GameManager.Instance.moveSpeed * Time.deltaTime);

        if (Keyboard.current.spaceKey.isPressed)
        {
            if (isReloading)
            {
                if (Time.time >= reloadEndTime)
                {
                    isReloading = false;
                    bulletsFired = 0;
                }
                else
                {
                    return;
                }
            }

            bool canShootBySpeed = Time.time >= lastShotTime + GameManager.Instance.shootingSpeed;
            bool canShootByMagazine = bulletsFired < GameManager.Instance.maxBullets;

            if (canShootBySpeed && canShootByMagazine)
            {
                Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

                bulletsFired++;
                lastShotTime = Time.time;

                if (shootSound != null)
                    AudioSource.PlayClipAtPoint(shootSound, transform.position);

                if (bulletsFired >= GameManager.Instance.maxBullets)
                {
                    isReloading = true;
                    reloadEndTime = Time.time + GameManager.Instance.reloadTime;
                }
            }
        }

        ClampToBackground();
    }

    public void ClearBullet(GameObject bullet)
    {
        currentBullets.Remove(bullet);
    }

    public void ClearAllBullets()
    {
        foreach (GameObject bullet in currentBullets)
        {
            if (bullet != null)
                Destroy(bullet);
        }

        currentBullets.Clear();
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

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float timer = 0f;

        while (timer < invulnerableDuration)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.1f);

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