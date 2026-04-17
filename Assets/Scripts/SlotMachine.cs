using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [Header("Item Pool")]
    public List<ItemData> allItems; // List instead of array
    public ItemButton itemButton;

    private ItemData currentItem;

    void Start()
    {
        Roll(); // kiest meteen een item voor de button
    }

    public void Roll()
    {
        if (allItems == null || allItems.Count == 0)
        {
            Debug.Log("No more items to roll!");
            return;
        }

        // kies random item
        int index = Random.Range(0, allItems.Count);
        currentItem = allItems[index];

        // toon item op button
        itemButton.SetItem(currentItem, this);

        Debug.Log("Rolled: " + currentItem.itemName);
    }

    // Haal het huidige item uit de lijst zodra het gepakt wordt
    public void RemoveCurrentItem()
    {
        if (currentItem != null)
        {
            allItems.Remove(currentItem);
            Debug.Log("Removed item from pool: " + currentItem.itemName);
            currentItem = null;
        }
    }
}