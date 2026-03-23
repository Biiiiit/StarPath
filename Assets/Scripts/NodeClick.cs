using UnityEngine;

public class NodeClick : MonoBehaviour
{
    private MapNode node;

    void Start()
    {
        node = GetComponent<MapNode>();

        if (node == null)
        {
            Debug.LogError("No MapNode found on " + gameObject.name);
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked on: " + gameObject.name);

        if (node == null)
        {
            Debug.LogError("Node is NULL!");
            return;
        }

        if (node.isUnlocked)
        {
            Debug.Log("Node is unlocked, selecting...");

            if (MapManager.Instance != null)
            {
                MapManager.Instance.SelectNode(node);
            }
            else
            {
                Debug.LogError("MapManager instance is NULL!");
            }
        }
        else
        {
            Debug.Log("Node is locked!");
        }
    }
}