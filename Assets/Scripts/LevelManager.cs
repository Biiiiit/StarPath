using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("UI")]
    public RewardScreenUI rewardScreenUI;

    public void CompleteLevel(int credits, ItemData item)
    {
        rewardScreenUI.Show(credits, item);
    }

    public void CompleteLevel()
    {
        StartCoroutine(GoToMap());
    }

    IEnumerator GoToMap()
    {
        GameProgress.Get().CompleteCurrentNode();
        yield return null;
        FadeUI.Instance.LoadScene("MapScene");
    }
}