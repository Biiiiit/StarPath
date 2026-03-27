using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public MapNode startNode;
    public MapNode currentNode;

    public GameObject mapRoot;

    private Scene currentLevelScene;

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
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null)
            {
                MapNode node = hit.GetComponent<MapNode>();

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

        mapRoot.SetActive(false);

        SceneManager.LoadScene(node.sceneName, LoadSceneMode.Additive);

        currentLevelScene = SceneManager.GetSceneByName(node.sceneName);
    }

    public void CompleteCurrentNode()
    {
        currentNode.isCompleted = true;
        UnlockNextNodes(currentNode);
    }

    public void ReturnToMap()
    {
        if (currentLevelScene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(currentLevelScene);
        }

        mapRoot.SetActive(true);

        RefreshConnections();
    }

    public void RefreshConnections()
    {
        MapConnection[] connections = FindObjectsByType<MapConnection>(FindObjectsSortMode.None);

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
        MapConnection[] connections = FindObjectsByType<MapConnection>(FindObjectsSortMode.None);

        foreach (MapConnection conn in connections)
        {
            if (conn.fromNode == from && conn.toNode == to)
                return conn;
        }

        return null;
    }
}