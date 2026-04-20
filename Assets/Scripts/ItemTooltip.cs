using System.Collections;
using UnityEngine;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text nameText;
    public TMP_Text descText;

    public static ItemTooltip Instance;

    private RectTransform rectTransform;
    private Coroutine hideRoutine;

    void Awake()
    {
        Instance = this;
        rectTransform = panel.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(1f, 1f);
        Hide();
    }

    void Update()
    {
        if (!panel.activeSelf) return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            Input.mousePosition,
            Camera.main,
            out Vector3 worldPos
        );

        rectTransform.position = worldPos;
    }
    public void Show(ItemData item)
    {
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        panel.SetActive(true);
        nameText.text = item.itemName;
        descText.text = item.description;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}