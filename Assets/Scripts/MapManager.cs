using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public MapNode startNode;

    public GameObject mapRoot;

    public Transform playerMarker;
    public float moveSpeed = 1f;

    private Scene currentLevelScene;
    private bool isMoving = false;

    private Dictionary<string, MapNode> nodes = new Dictionary<string, MapNode>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BuildNodeDictionary();
        ApplyProgressToMap();
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
                    StartCoroutine(MoveToNode(node));
            }
        }
    }

    void BuildNodeDictionary()
    {
        nodes.Clear();

        MapNode[] allNodes = FindObjectsByType<MapNode>(FindObjectsSortMode.None);

        foreach (MapNode node in allNodes)
        {
            if (node == null || string.IsNullOrEmpty(node.nodeID))
                continue;

            nodes[node.nodeID] = node;
        }
    }

    void ApplyProgressToMap()
    {
        foreach (MapNode node in nodes.Values)
        {
            node.isCompleted = GameProgress.Instance.IsCompleted(node.nodeID);
            node.isUnlocked = false;
        }

        string currentID = GameProgress.Instance.currentNodeID;

        if (string.IsNullOrEmpty(currentID) || !nodes.ContainsKey(currentID))
        {
            currentID = startNode.nodeID;
            GameProgress.Instance.currentNodeID = currentID;
        }

        MapNode current = nodes[currentID];

        foreach (MapNode next in current.connectedNodes)
        {
            next.isUnlocked = true;
        }

        current.isUnlocked = true;

        foreach (MapNode node in nodes.Values)
        {
            node.RefreshVisual();
        }

        if (playerMarker != null)
        {
            playerMarker.position = current.transform.position;
            playerMarker.localScale = Vector3.zero;
        }

        RefreshConnections();
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
                scale = Mathf.Lerp(0f, 0.3f, progress / 0.3f);
            else if (progress < 0.7f)
                scale = 0.3f;
            else
                scale = Mathf.Lerp(0.3f, 0f, (progress - 0.7f) / 0.3f);

            playerMarker.localScale = Vector3.one * scale;

            yield return null;
        }

        playerMarker.position = targetPos;
        playerMarker.localScale = Vector3.zero;

        SelectNode(node);

        isMoving = false;
    }

    public void SelectNode(MapNode node)
    {
        GameProgress.Instance.currentNodeID = node.nodeID;
        GameProgress.Instance.selectedBackground = node.backgroundController;

        SceneManager.LoadScene(node.sceneName, LoadSceneMode.Single);
    }

    public void CompleteCurrentNode()
    {
        GameProgress.Instance.CompleteCurrentNode();
    }

    public void ReturnToMap()
    {
        if (currentLevelScene.isLoaded)
            SceneManager.UnloadSceneAsync(currentLevelScene);

        ApplyProgressToMap();
    }

    public void RefreshConnections()
    {
        MapConnection[] connections = FindObjectsByType<MapConnection>(FindObjectsSortMode.None);

        foreach (MapConnection conn in connections)
        {
            conn.SetActive(false);
            conn.SetBlinking(false);

            if (GameProgress.Instance.IsCompleted(conn.fromNode.nodeID) &&
                GameProgress.Instance.IsCompleted(conn.toNode.nodeID))
            {
                conn.SetActive(true);
            }

            if (conn.fromNode.nodeID == GameProgress.Instance.currentNodeID &&
                conn.toNode.isUnlocked)
            {
                conn.SetBlinking(true);
            }
        }
    }
}