using UnityEngine;

public class MapConnection : MonoBehaviour
{
    public MapNode fromNode;
    public MapNode toNode;

    public bool isActive = false;
    public bool isBlinking = false;

    public float blinkSpeed = 2f;

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Start()
    {
        lr.positionCount = 2;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;

        RefreshLine();
        RefreshColor();
    }

    void Update()
    {
        if (isBlinking)
            RefreshColor();
    }

    public void RefreshLine()
    {
        lr.SetPosition(0, fromNode.transform.position);
        lr.SetPosition(1, toNode.transform.position);
    }

    public void SetActive(bool active)
    {
        isActive = active;
        isBlinking = false;
        RefreshColor();
    }

    public void SetBlinking(bool blinking)
    {
        isBlinking = blinking;

        if (blinking)
            isActive = false;

        RefreshColor();
    }

    public void RefreshColor()
    {
        if (isActive)
        {
            lr.startColor = Color.white;
            lr.endColor = Color.white;
        }
        else if (isBlinking)
        {
            float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            Color c = Color.Lerp(Color.white, Color.gray, t);

            lr.startColor = c;
            lr.endColor = c;
        }
        else
        {
            lr.startColor = Color.gray;
            lr.endColor = Color.gray;
        }
    }
}