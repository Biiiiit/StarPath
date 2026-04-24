using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text priceText;
    public GameObject priceContainer;
    public int price = 20;

    public Image iconImage;

    private ItemData item;
    private bool bought = false;

    public void SetItem(ItemData newItem)
    {
        item = newItem;
        bought = false;

        price = Random.Range(10, 21); // 10 t/m 20

        if (iconImage != null && item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null || bought) return;

        if (!GameManager.Instance.CanAfford(price))
        {
            Debug.Log("Not enough money");
            return;
        }

        // money afhalen
        GameManager.Instance.SpendCredits(price);

        // item toepassen
        GameManager.Instance.ApplyItem(item);

        // UI blokkeren
        bought = true;
        iconImage.enabled = false;

        priceContainer.SetActive(false);

        Debug.Log("Bought: " + item.itemName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bought) return;

        priceContainer.SetActive(true);
        priceText.text = price.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        priceContainer.SetActive(false);
    }
}