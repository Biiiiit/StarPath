using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;

    public ItemData itemData;

    public void Setup(ItemData item)
    {
        itemData = item;
        icon.sprite = item.icon;
    }
}