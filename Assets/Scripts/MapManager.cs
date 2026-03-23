using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public MapNode currentNode;
    public MapNode startNode;

    private void Awake()
    {
        // Singleton (zorgt dat er maar 1 is)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Start node unlocken
        if (startNode != null)
        {
            startNode.isUnlocked = true;
            currentNode = startNode;

            Debug.Log("Start node unlocked");
        }
    }

    public void SelectNode(MapNode node)
    {
        if (!node.isUnlocked)
        {
            Debug.Log("Node is locked!");
            return;
        }

        currentNode = node;

        Debug.Log("Selected node: " + node.name);

        // 👉 HIER zou je later scenes laden
        // Voor nu testen we alleen
        SimulateLevel(node);
    }

    void SimulateLevel(MapNode node)
    {
        Debug.Log("Starting level: " + node.nodeType);

        // Simuleer dat je level klaar is
        CompleteNode(node);
    }

    void CompleteNode(MapNode node)
    {
        node.isCompleted = true;

        Debug.Log("Completed node: " + node.name);

        // Unlock volgende nodes
        foreach (MapNode next in node.connectedNodes)
        {
            next.isUnlocked = true;
            Debug.Log("Unlocked: " + next.name);
        }
    }
}