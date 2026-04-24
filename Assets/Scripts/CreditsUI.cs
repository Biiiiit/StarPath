using UnityEngine;
using TMPro;

public class CreditsUI : MonoBehaviour
{
    private TMP_Text text;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            text.text = GameManager.Instance.credits.ToString();
        }
    }
}