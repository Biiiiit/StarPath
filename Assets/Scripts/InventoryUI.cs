using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    public static bool IsInspecting = false;
    public static ItemData CurrentHoverItem;
    public GameObject pausePanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            IsInspecting = !IsInspecting;

            Time.timeScale = IsInspecting ? 0f : 1f;

            pausePanel.SetActive(IsInspecting);

            if (!IsInspecting)
            {
                CurrentHoverItem = null;
                ItemTooltip.Instance.Hide();
            }
        }

        if (IsInspecting)
        {
            if (CurrentHoverItem != null)
                ItemTooltip.Instance.Show(CurrentHoverItem);
            else
                ItemTooltip.Instance.Hide();
        }
    }

    void Start()
    {
        Refresh();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Refresh()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        foreach (ItemData item in GameManager.Instance.inventory)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            slot.GetComponent<InventorySlotUI>().Setup(item);
        }
    }
}