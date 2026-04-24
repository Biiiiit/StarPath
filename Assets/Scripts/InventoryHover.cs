using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventorySlotUI slot;

    void Awake()
    {
        slot = GetComponent<InventorySlotUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!InventoryUI.IsInspecting) return;

        InventoryUI.CurrentHoverItem = slot.itemData;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InventoryUI.CurrentHoverItem == slot.itemData)
        {
            InventoryUI.CurrentHoverItem = null;
        }
    }
}