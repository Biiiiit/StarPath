using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public MapNode startNode;
    public MapNode currentNode;

    public GameObject mapRoot;

    public Transform playerMarker; // sprite that moves between nodes
    public float moveSpeed = 3f;

    private Scene currentLevelScene;
    private bool isMoving = false;

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
            SetNodeCompleted(startNode);
            UnlockNextNodes(startNode);

            if (playerMarker != null)
                playerMarker.position = startNode.transform.position;
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MapScene")
            return;

        if (isMoving)
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
                    StartCoroutine(MoveToNode(node));
                }
            }
        }
    }

    IEnumerator MoveToNode(MapNode node)
    {
        isMoving = true;

        Vector3 targetPos = node.transform.position;

        float totalDistance = Vector3.Distance(playerMarker.position, targetPos);

        while (Vector3.Distance(playerMarker.position, targetPos) > 0.02f)
        {
            Vector3 direction = targetPos - playerMarker.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            playerMarker.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

            playerMarker.position = Vector3.MoveTowards(
                playerMarker.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            float remaining = Vector3.Distance(playerMarker.position, targetPos);
            float progress = 1f - (remaining / totalDistance);

            float scale;

            if (progress < 0.3f)
            {
                // grow
                scale = Mathf.Lerp(0f, 0.3f, progress / 0.3f);
            }
            else if (progress < 0.7f)
            {
                // hold
                scale = 0.3f;
            }
            else
            {
                // shrink
                scale = Mathf.Lerp(0.3f, 0f, (progress - 0.7f) / 0.3f);
            }

            playerMarker.localScale = Vector3.one * scale;

            yield return null;
        }

        playerMarker.position = targetPos;
        playerMarker.localScale = Vector3.zero;

        SelectNode(node);

        isMoving = false;
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
        SetNodeCompleted(currentNode);
        UnlockNextNodes(currentNode);
    }

    void SetNodeCompleted(MapNode node)
    {
        node.isCompleted = true;

        SpriteRenderer sr = node.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(0.7f, 0.7f, 0.7f, 1f);

        if (node.checkmarkSprite != null)
            node.checkmarkSprite.SetActive(true);
    }

    public void ReturnToMap()
    {
        if (currentLevelScene.isLoaded)
            SceneManager.UnloadSceneAsync(currentLevelScene);

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