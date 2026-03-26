using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CreditManager : MonoBehaviour
{
    public static CreditManager Instance;

    public int credits = 0;

    private List<TMP_Text> creditsTexts = new List<TMP_Text>();

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

    public void RegisterText(TMP_Text text)
    {
        if (text == null) return;

        if (!creditsTexts.Contains(text))
        {
            creditsTexts.Add(text);
        }

        UpdateUI(); // update immediately
    }

    public void AddCredits(int amount)
    {
        credits += amount;
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = creditsTexts.Count - 1; i >= 0; i--)
        {
            if (creditsTexts[i] == null)
            {
                creditsTexts.RemoveAt(i); // cleanup destroyed UI
                continue;
            }

            creditsTexts[i].text = credits.ToString("0");
        }
    }
}