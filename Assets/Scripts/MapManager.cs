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
            DontDestroyOnLoad(gameObject); // 👈 BELANGRIJK
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
        // alleen input als we in map scene zitten
        if (!SceneManager.GetActiveScene().name.Contains("Map"))
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
        // stop knipperen van huidige opties
        foreach (MapNode next in currentNode.connectedNodes)
        {
            MapConnection conn = FindConnection(currentNode, next);
            if (conn != null)
                conn.SetBlinking(false);
        }

        // gekozen lijn wordt vast wit
        MapConnection chosenConn = FindConnection(currentNode, node);
        if (chosenConn != null)
            chosenConn.SetActive(true);

        currentNode = node;

        // 👇 laad scene van node
        SceneManager.LoadScene(node.sceneName);
    }

    public void CompleteCurrentNode()
    {
        currentNode.isCompleted = true;
        UnlockNextNodes(currentNode);
    }

    MapConnection FindConnection(MapNode from, MapNode to)
    {
        MapConnection[] connections = FindObjectsByType<MapConnection>(FindObjectsSortMode.None);
        foreach (MapConnection conn in connections)
        {
            if (conn.fromNode == from && conn.toNode == to)
                return conn;
        }
        return null;
    }
}