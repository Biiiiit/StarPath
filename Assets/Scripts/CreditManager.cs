using UnityEngine;
using TMPro;

public class CreditManager : MonoBehaviour
{
    public static CreditManager Instance;

    public int credits = 0;
    public TMP_Text creditsText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetText(TMP_Text text)
    {
        creditsText = text;
        UpdateUI(); // update immediately when assigned
    }

    public void AddCredits(int amount)
    {
        credits += amount;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (creditsText != null)
            creditsText.text = credits.ToString();
    }
}