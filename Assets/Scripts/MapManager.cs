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

    void Start() { }

    public void OnMapGenerated()
    {
        BuildNodeDictionary();
        // only reset to start if there's no saved progress
        if (string.IsNullOrEmpty(GameProgress.Get().currentNodeID))
            GameProgress.Get().currentNodeID = startNode.nodeID;
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

        MapNode[] allNodes = FindObjectsByType<MapNode>(FindObjectsInactive.Include, FindObjectsSortMode.None);

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
            node.isCompleted = GameProgress.Get().IsCompleted(node.nodeID);
            node.isUnlocked = false;
        }

        // on first load use startNode, on return use saved currentID
        string currentID = GameProgress.Get().currentNodeID;
        if (string.IsNullOrEmpty(currentID) || !nodes.ContainsKey(currentID))
            currentID = startNode.nodeID;

        GameProgress.Get().currentNodeID = currentID;

        MapNode current = nodes[currentID];
        current.isUnlocked = true;

        if (current.isCompleted)
        {
            foreach (MapNode next in current.connectedNodes)
            {
                if (next == null) continue;
                next.isUnlocked = true;
            }
        }

        foreach (MapNode node in nodes.Values)
            node.RefreshVisual();

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
        GameProgress.Get().currentNodeID = node.nodeID;
        GameProgress.Get().selectedBackground = node.backgroundController;

        FadeUI.Instance.LoadScene(node.sceneName); ;
    }

    public void CompleteCurrentNode()
    {
        GameProgress.Get().CompleteCurrentNode();
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
            if (conn.fromNode == null || conn.toNode == null) continue;
            conn.SetActive(false);
            conn.SetBlinking(false);

            if (GameProgress.Get().IsCompleted(conn.fromNode.nodeID) &&
                GameProgress.Get().IsCompleted(conn.toNode.nodeID))
            {
                conn.SetActive(true);
            }

            if (conn.fromNode.nodeID == GameProgress.Get().currentNodeID &&
                conn.toNode.isUnlocked)
            {
                conn.SetBlinking(true);
            }
        }
    }
}