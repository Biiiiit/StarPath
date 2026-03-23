using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public List<MapNode> connectedNodes = new List<MapNode>();
    public bool isUnlocked = false;
    public bool isCompleted = false;
    public NodeType nodeType;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isCompleted)
            sr.color = Color.green;
        else if (isUnlocked)
            sr.color = Color.white;
        else
            sr.color = Color.gray;
    }
}

public enum NodeType
{
    Combat,
    Elite,
    Shop,
    Boss,
    Item,
    Heal
}