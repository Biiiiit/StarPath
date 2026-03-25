using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void CompleteLevel()
    {
        
        MapManager.Instance.CompleteCurrentNode();

        
        SceneManager.LoadScene("MapScene");
    }
}