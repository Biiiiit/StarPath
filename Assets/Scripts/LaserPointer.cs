using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
    private LineRenderer lr;
    public float length = 20f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    void Update()
    {
        if (!GameManager.Instance.hasLaserPointer)
        {
            lr.enabled = false;
            return;
        }

        lr.enabled = true;

        Vector3 start = transform.position;
        Vector3 end = start + transform.up * length;

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}