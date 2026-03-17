using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public float speed = 5f;
    public SpriteRenderer background;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    private GameObject currentBullet;

    void Update()
    {
        float move = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            move = -1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            move = 1f;

        transform.Translate(Vector2.right * move * speed * Time.deltaTime);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && currentBullet == null)
        {
            currentBullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
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
}