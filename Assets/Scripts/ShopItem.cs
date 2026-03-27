using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text priceText;
    public GameObject priceContainer;
    public int price = 10;

    public void OnPointerEnter(PointerEventData eventData)
    {
        priceContainer.SetActive(true);
        priceText.text = price.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        priceContainer.SetActive(false);
    }
}