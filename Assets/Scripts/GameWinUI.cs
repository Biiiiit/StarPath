using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameWinUI : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;

    [Header("Fade")]
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panel.AddComponent<CanvasGroup>();

        panel.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);
        StartCoroutine(FadeIn());

        Time.timeScale = 0f;
    }

    IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public void HandleContinue()
    {
        Time.timeScale = 1f;
        FadeUI.Instance.LoadScene("MainMenu");
    }
}