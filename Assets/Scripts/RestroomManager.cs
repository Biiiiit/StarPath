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

        Debug.Log("add life");

        speechManager.ShowBubble(1); // show heal dialogue
        CloseUI();
    }

    public void ChooseStat()
    {
        if (chosen) return;
        chosen = true;

        Debug.Log("add stat");

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