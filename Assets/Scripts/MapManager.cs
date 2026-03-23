using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public MapNode startNode;
    public MapNode currentNode;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (startNode != null)
        {
            currentNode = startNode;
            startNode.isCompleted = true;
            UnlockNextNodes(startNode);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                MapNode node = hit.collider.GetComponent<MapNode>();
                if (node != null && node.isUnlocked)
                {
                    SelectNode(node);
                }
            }
        }
    }

    void UnlockNextNodes(MapNode node)
    {
        // zet alle mogelijke lijnen knipperend
        foreach (MapNode next in node.connectedNodes)
        {
            next.isUnlocked = true;
            MapConnection conn = FindConnection(node, next);
            if (conn != null)
                conn.SetBlinking(true);
        }
    }

    public void SelectNode(MapNode node)
    {
        // zet alle lijnen van huidige node terug op grijs
        foreach (MapNode next in currentNode.connectedNodes)
        {
            MapConnection conn = FindConnection(currentNode, next);
            if (conn != null)
                conn.SetActive(false);
        }

        // zet de gekozen lijn wit en constant
        MapConnection chosenConn = FindConnection(currentNode, node);
        if (chosenConn != null)
            chosenConn.SetActive(true);

        currentNode = node;
        node.isCompleted = true;

        // unlock volgende nodes
        UnlockNextNodes(node);
    }

    MapConnection FindConnection(MapNode from, MapNode to)
    {
        MapConnection[] connections = FindObjectsOfType<MapConnection>();
        foreach (MapConnection conn in connections)
        {
            if (conn.fromNode == from && conn.toNode == to)
                return conn;
        }
        return null;
    }
}