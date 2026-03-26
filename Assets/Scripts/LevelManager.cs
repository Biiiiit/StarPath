using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public void CompleteLevel()
    {
        MapManager.Instance.CompleteCurrentNode();
        MapManager.Instance.ReturnToMap();
    }
}