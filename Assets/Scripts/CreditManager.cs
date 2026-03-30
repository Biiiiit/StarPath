using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CreditManager : MonoBehaviour
{
    public static CreditManager Instance;

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
    }

    public void AddCredits(int amount)
    {
        GameManager.Instance.credits += amount;
    }
}