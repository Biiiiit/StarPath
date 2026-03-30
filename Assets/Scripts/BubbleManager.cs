using UnityEngine;

public class SpeechBubbleManager : MonoBehaviour
{
    public GameObject[] bubbles; // 0 = first, 1 = heal, 2 = stat

    public void ShowBubble(int index)
    {
        // Disable all bubbles
        for (int i = 0; i < bubbles.Length; i++)
        {
            bubbles[i].SetActive(false);
        }

        // Enable only the selected one
        if (index >= 0 && index < bubbles.Length)
        {
            bubbles[index].SetActive(true);
        }
    }
}