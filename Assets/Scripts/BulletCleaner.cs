using UnityEngine;

public class BulletCleaner : MonoBehaviour
{
    void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x < -0.1f || pos.x > 1.1f || pos.y < -0.1f || pos.y > 1.1f)
        {
            Destroy(gameObject);
        }
    }
}