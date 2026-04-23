using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardScreenUI : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public TextMeshProUGUI creditsValue;

    [Header("Item Reward")]
    public GameObject itemRewardPanel;       // parent container, hidden when no drop
    public Transform itemSlotParent;         // where the slot prefab gets instantiated
    public InventorySlotUI inventorySlotPrefab;

    [Header("Fade")]
    public float fadeDuration = 0.5f;
    [Header("Level")]
    public LevelManager levelManager;

    private CanvasGroup canvasGroup;
    private Action onContinue;

    void Awake()
    {
        canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panel.AddComponent<CanvasGroup>();

        panel.SetActive(false);
    }

    public void Show(int credits, ItemData item)
    {
        creditsValue.text = $"{credits}";

        if (item != null)
        {
            itemRewardPanel.SetActive(true);

            // Clear any previous slot and spawn a fresh one
            foreach (Transform child in itemSlotParent)
                Destroy(child.gameObject);

            InventorySlotUI slot = Instantiate(inventorySlotPrefab, itemSlotParent);
            slot.Setup(item);
        }
        else
        {
            itemRewardPanel.SetActive(false);
        }

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
        levelManager.CompleteLevel();
    }
}