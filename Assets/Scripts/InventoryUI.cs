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
        AddTestItem();
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

    public void AddTestItem()
    {
        ItemData item = Resources.Load<ItemData>("Items/Speed Module");
        ItemData item2 = Resources.Load<ItemData>("Items/RefillCanister");

        if (item != null)
        {
            GameManager.Instance.inventory.Add(item);
            Debug.Log("Added test item: " + item.itemName);
            GameManager.Instance.inventory.Add(item2);
            Debug.Log("Added test item: " + item2.itemName);
        }
        else
        {
            Debug.LogWarning("Speed Module not found in Resources folder.");
        }
    }
}