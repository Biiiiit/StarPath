using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public void CompleteLevel()
    {
        MapNode node = MapManager.Instance.currentNode;
        node.isCompleted = true;

        foreach (MapNode next in node.connectedNodes)
        {
            next.isUnlocked = true;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene");
    }
}