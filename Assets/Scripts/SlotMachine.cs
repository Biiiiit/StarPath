using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    public List<ItemData> allItems;
    public ItemButton itemButton;

    private ItemData currentItem;
    private bool itemTaken = false;

    void Start()
    {
        Roll();
    }

    public void Roll()
    {
        if (itemTaken)
        {
            Debug.Log("Item already taken, cannot reroll");
            return;
        }

        List<ItemData> pool = new List<ItemData>();

        foreach (ItemData item in allItems)
        {
            if (!GameManager.Instance.removedItems.Contains(item))
            {
                pool.Add(item);
            }
        }

        if (pool.Count == 0)
        {
            Debug.Log("No more items to roll!");
            return;
        }

        int index = Random.Range(0, pool.Count);
        currentItem = pool[index];

        itemButton.SetItem(currentItem, this);
    }

    public void RemoveCurrentItem()
    {
        if (currentItem == null) return;

        GameManager.Instance.removedItems.Add(currentItem);

        itemTaken = true; 

        currentItem = null;
    }
}