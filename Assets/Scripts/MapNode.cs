using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public string nodeID;

    public List<MapNode> connectedNodes = new List<MapNode>();

    public bool isUnlocked = false;
    public bool isCompleted = false;

    public NodeType nodeType;
    public string sceneName;

    public GameObject checkmarkSprite;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void RefreshVisual()
    {
        if (isCompleted)
        {
            sr.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            if (checkmarkSprite != null)
                checkmarkSprite.SetActive(true);
        }
        else if (isUnlocked)
        {
            sr.color = Color.white;

            if (checkmarkSprite != null)
                checkmarkSprite.SetActive(false);
        }
        else
        {
            sr.color = Color.gray;

            if (checkmarkSprite != null)
                checkmarkSprite.SetActive(false);
        }
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