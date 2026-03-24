using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void CompleteLevel()
    {
        // markeer node als completed + unlock volgende
        MapManager.Instance.CompleteCurrentNode();

        // terug naar map
        SceneManager.LoadScene("MapScene");
    }
}