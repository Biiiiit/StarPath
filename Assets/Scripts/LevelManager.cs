using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public void CompleteLevel()
    {
        MapNode node = MapManager.Instance.currentNode;

        // markeer node als completed
        node.isCompleted = true;

        // unlock volgende nodes + zet lijnen op knipperen
        foreach (MapNode next in node.connectedNodes)
        {
            next.isUnlocked = true;

            MapConnection conn = FindConnection(node, next);
            if (conn != null)
                conn.SetBlinking(true);
        }

        // terug naar map
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene");
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