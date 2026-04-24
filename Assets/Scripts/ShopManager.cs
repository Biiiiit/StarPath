using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<ItemData> availableItems = new List<ItemData>();

    public ShopItem[] shopItems;

    void Start()
    {
        RollShop();
    }

    public void RollShop()
    {
        List<ItemData> pool = new List<ItemData>(availableItems);

        for (int i = 0; i < shopItems.Length; i++)
        {
            if (pool.Count == 0) return;

            int index = Random.Range(0, pool.Count);

            ItemData chosen = pool[index];
            pool.RemoveAt(index);

            shopItems[i].SetItem(chosen);
        }
    }
}