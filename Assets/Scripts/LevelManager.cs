using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public void CompleteLevel()
    {
        StartCoroutine(ReturnToMap());
    }

    IEnumerator ReturnToMap()
    {
        GameProgress.Instance.CompleteCurrentNode();

        yield return null;

        SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
    }
}