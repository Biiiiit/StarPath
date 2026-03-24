using UnityEngine;

public class CreditManager : MonoBehaviour
{
    public int credits = 0;

    public void AddCredits(int amount)
    {
        credits += amount;
        Debug.Log("Credits: " + credits);
    }
}