using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public SpriteRenderer background;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public AudioClip shootSound;

    private List<GameObject> currentBullets = new List<GameObject>();

    public LivesUI livesUI;
    public GameObject gameOverUI;

    private int activeBullets = 0;
    private float lastShotTime = 0f;

    private bool isReloading = false;
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
                    activeBullets = 0;
                }
            }
            else
            {
                bool canShootBySpeed = Time.time >= lastShotTime + GameManager.Instance.shootingSpeed;
                bool canShootByAmmo = activeBullets < GameManager.Instance.maxBullets;

                if (canShootBySpeed && canShootByAmmo)
                {
                    GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
                    currentBullets.Add(bullet);

                    activeBullets++;
                    lastShotTime = Time.time;

                    if (shootSound != null)
                        AudioSource.PlayClipAtPoint(shootSound, transform.position);

                    // trigger reload when magazine is full
                    if (activeBullets >= GameManager.Instance.maxBullets)
                    {
                        isReloading = true;
                        reloadEndTime = Time.time + GameManager.Instance.reloadTime;
                    }
                }
            }
        }

        ClampToBackground();
    }

    public void ClearBullet(GameObject bullet)
    {
        currentBullets.Remove(bullet);
        activeBullets = Mathf.Max(0, activeBullets - 1);
    }

    public void ClearAllBullets()
    {
        foreach (GameObject bullet in new List<GameObject>(currentBullets))
        {
            if (bullet != null)
            {
                Destroy(bullet);
            }
        }

        currentBullets.Clear();
        activeBullets = 0;
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
        livesUI.LoseLife();
        GameManager.Instance.lives--;

        if (GameManager.Instance.lives <= 0)
        {
            Time.timeScale = 0f;
            gameOverUI.SetActive(true);
        }
    }

    public void GainLife()
    {
        livesUI.GainLife();
        GameManager.Instance.lives++;
    }
}