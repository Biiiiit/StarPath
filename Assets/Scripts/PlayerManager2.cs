using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager2 : MonoBehaviour
{
    public float speed = 5f;
    public SpriteRenderer background;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public AudioClip shootSound;

    private GameObject currentBullet;

    void Update()
    {
        Vector2 move = Vector2.zero;

        // Horizontaal
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            move.x = -1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            move.x = 1f;

        // Verticaal
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            move.y = 1f;

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            move.y = -1f;

        transform.Translate(move.normalized * speed * Time.deltaTime);

        // Schieten
        if (Keyboard.current.spaceKey.wasPressedThisFrame && currentBullet == null)
        {
            currentBullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

            Bullet2 bulletScript = currentBullet.GetComponent<Bullet2>();
            if (bulletScript != null)
            {
                bulletScript.direction = Vector2.right;
            }
        }

        ClampToBackground();
    }

    public void ClearBullet()
    {
        currentBullet = null;
    }

    void ClampToBackground()
    {
        Bounds bounds = background.bounds;

        float halfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        float halfHeight = GetComponent<SpriteRenderer>().bounds.extents.y;

        float clampedX = Mathf.Clamp(
            transform.position.x,
            bounds.min.x + halfWidth,
            bounds.max.x - halfWidth
        );

        float clampedY = Mathf.Clamp(
            transform.position.y,
            bounds.min.y + halfHeight,
            bounds.max.y - halfHeight
        );

        transform.position = new Vector3(
            clampedX,
            clampedY,
            transform.position.z
        );
    }
}