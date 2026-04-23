using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void GoToMapScene()
    {
        FadeUI.Instance.LoadScene("MapScene");
    }

    public void QuitGame()
    {
        Application.Quit();

        // Stops play mode when testing in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}