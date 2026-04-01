using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public Image iconImage;
    private Button button;
    private ItemData item;
    private SlotMachine slotMachine;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    // Zet item + link naar slot machine
    public void SetItem(ItemData newItem, SlotMachine machine)
    {
        item = newItem;
        slotMachine = machine;

        if (iconImage != null && item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }

        button.interactable = true;
    }

    void OnClick()
    {
        if (item == null) return;

        GameManager.Instance.ApplyItem(item);

        // verberg button
        iconImage.enabled = false;
        button.interactable = false;

        // verwijder uit pool
        slotMachine.RemoveCurrentItem();
    }
}