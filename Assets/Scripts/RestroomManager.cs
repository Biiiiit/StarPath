using UnityEngine;

public class HealingRoomUI : MonoBehaviour
{
    public GameObject choiceUI;
    public LevelManager levelManager;

    public SpeechBubbleManager speechManager;

    private bool chosen = false;

    public void Start()
    {
        // Show first speech bubble automatically
        speechManager.ShowBubble(0);
    }

    public void OpenChoice()
    {
        choiceUI.SetActive(true);
        chosen = false;
    }

    public void ChooseHeal()
    {
        if (chosen) return;
        chosen = true;

        GameManager.Instance.lives++;

        speechManager.ShowBubble(1); // show heal dialogue
        CloseUI();
    }

    public void ChooseStat()
    {
        if (chosen) return;
        chosen = true;

        Debug.Log("add stat");

        int randomStat = Random.Range(0, 9); // 9 stats total

        switch (randomStat)
        {
            case 0:
                GameManager.Instance.maxLives += 1;
                break;

            case 1:
                GameManager.Instance.credits += 1;
                break;

            case 2:
                GameManager.Instance.moveSpeed += 1f;
                break;

            case 3:
                GameManager.Instance.shotSpeed += 1f;
                break;

            case 4:
                GameManager.Instance.bulletSpeed += 1f;
                break;

            case 5:
                GameManager.Instance.reloadSpeed += 1f;
                break;

            case 6:
                GameManager.Instance.maxBullets += 1;
                break;

            case 7:
                GameManager.Instance.bulletPierce += 1;
                break;
        }

        speechManager.ShowBubble(2); // show stat dialogue
        CloseUI();
    }

    public void CloseUI()
    {
        choiceUI.SetActive(false);
    }

    public void ContinueAfterChoice()
    {
        levelManager.CompleteLevel();
    }
}