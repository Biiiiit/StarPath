using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    private bool inspectMode = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inspectMode = !inspectMode;

            if (inspectMode)
            {
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    void Start()
    {
        Refresh();
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