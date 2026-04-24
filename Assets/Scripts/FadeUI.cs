using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeUI : MonoBehaviour
{
    public static FadeUI Instance;

    public Image overlay;
    public float fadeDuration = 0.5f;
    public float holdDuration = 0.3f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        overlay.color = new Color(0, 0, 0, 1f);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(HoldThenFadeIn());
    }

    IEnumerator HoldThenFadeIn()
    {
        yield return new WaitForSecondsRealtime(holdDuration);
        yield return StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOutThenLoad(sceneName));
    }

    IEnumerator FadeOutThenLoad(string sceneName)
    {
        Time.timeScale = 0f;
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    IEnumerator FadeOut()
    {
        yield return Fade(0f, 1f);
    }

    IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f);
        Time.timeScale = 1f;
    }

    IEnumerator Fade(float from, float to)
    {
        overlay.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            overlay.color = new Color(0, 0, 0, Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / fadeDuration)));
            yield return null;
        }

        overlay.color = new Color(0, 0, 0, to);

        if (to == 0f)
            overlay.gameObject.SetActive(false);
    }
}