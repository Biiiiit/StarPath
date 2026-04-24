using UnityEngine;
using System.Collections.Generic;

public class EliteTracker : MonoBehaviour
{
    private static int totalCredits = 0;
    private static List<ItemData> droppedItems = new List<ItemData>();

    public void Reset()
    {
        totalCredits = 0;
        droppedItems.Clear();
    }

    public void Register(int credits, ItemData item)
    {
        totalCredits += credits;

        if (item != null)
            droppedItems.Add(item);
    }

    public int TotalCredits => totalCredits;
    public List<ItemData> DroppedItems => droppedItems;
}