using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (MapManager.Instance != null)
        {
            Destroy(MapManager.Instance.gameObject);
        }
        GameManager.Instance.lives = GameManager.Instance.maxLives;
        SceneManager.LoadScene("MapScene");
    }
}