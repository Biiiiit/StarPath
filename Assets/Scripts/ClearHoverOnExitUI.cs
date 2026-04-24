using UnityEngine;
using UnityEngine.EventSystems;

public class ClearHoverOnExitUI : MonoBehaviour, IPointerExitHandler
{
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI.CurrentHoverItem = null;
    }
}