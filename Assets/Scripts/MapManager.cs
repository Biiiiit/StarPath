using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public MapNode startNode;
    public MapNode currentNode;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
        if (SceneManager.GetActiveScene().name != "MapScene")
            return;

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
        foreach (MapNode next in currentNode.connectedNodes)
        {
            MapConnection conn = FindConnection(currentNode, next);
            if (conn != null)
                conn.SetBlinking(false);
        }

        MapConnection chosenConn = FindConnection(currentNode, node);
        if (chosenConn != null)
            chosenConn.SetActive(true);

        currentNode = node;

        MapNode[] nodes = FindObjectsOfType<MapNode>();
        foreach (MapNode n in nodes)
            n.gameObject.SetActive(false);

        MapConnection[] connections = FindObjectsOfType<MapConnection>();
        foreach (MapConnection c in connections)
            c.gameObject.SetActive(false);

        SceneManager.LoadScene(node.sceneName, LoadSceneMode.Additive);
    }

    public void CompleteCurrentNode()
    {
        currentNode.isCompleted = true;
        UnlockNextNodes(currentNode);
    }

    public void RefreshConnections()
    {
        MapConnection[] connections = FindObjectsOfType<MapConnection>(true);

        foreach (MapConnection conn in connections)
        {
            conn.SetActive(false);
            conn.SetBlinking(false);

            if (conn.fromNode == currentNode && conn.toNode.isUnlocked)
            {
                conn.SetBlinking(true);
            }
            else if (conn.fromNode.isCompleted && conn.toNode.isCompleted)
            {
                conn.SetActive(true);
            }
        }
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