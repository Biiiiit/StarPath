using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void CompleteLevel()
    {
        MapManager.Instance.CompleteCurrentNode();

        Scene levelScene = gameObject.scene;
        SceneManager.UnloadSceneAsync(levelScene);

        MapNode[] nodes = FindObjectsOfType<MapNode>(true);
        foreach (MapNode n in nodes)
            n.gameObject.SetActive(true);

        MapConnection[] connections = FindObjectsOfType<MapConnection>(true);
        foreach (MapConnection c in connections)
            c.gameObject.SetActive(true);

        MapManager.Instance.RefreshConnections();
    }
}