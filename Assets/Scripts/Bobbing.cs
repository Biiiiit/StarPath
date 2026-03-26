using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public float amplitude = 0.2f;
    public float frequency = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }
}