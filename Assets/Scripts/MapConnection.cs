using UnityEngine;

public class MapConnection : MonoBehaviour
{
    public MapNode fromNode;
    public MapNode toNode;

    private LineRenderer lr;

    public bool isActive = false;
    public bool isBlinking = false;

    public float blinkSpeed = 2f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Start()
    {
        lr.positionCount = 2;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
    }

    void OnEnable()
    {
        UpdateColor();
    }

    void Update()
    {
        lr.SetPosition(0, fromNode.transform.position);
        lr.SetPosition(1, toNode.transform.position);
        UpdateColor();
    }

    public void SetActive(bool active)
    {
        isActive = active;
        isBlinking = false;
    }

    public void SetBlinking(bool blinking)
    {
        isBlinking = blinking;
        if (blinking)
            isActive = false;
    }

    void UpdateColor()
    {
        if (lr == null) return;

        if (isActive)
        {
            lr.startColor = Color.white;
            lr.endColor = Color.white;
        }
        else if (isBlinking)
        {
            float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            Color blink = Color.Lerp(Color.white, Color.gray, t);

            lr.startColor = blink;
            lr.endColor = blink;
        }
        else
        {
            lr.startColor = Color.gray;
            lr.endColor = Color.gray;
        }
    }
}