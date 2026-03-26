using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject gameCanvas;
    public GameObject shopCanvas;

    [Header("Level")]
    public LevelManager levelManager;

    // Open shop
    public void OpenShop()
    {
        gameCanvas.SetActive(false);
        shopCanvas.SetActive(true);
    }

    // Close shop
    public void CloseShop()
    {
        shopCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }

    // Continue to next level
    public void Continue()
    {
        levelManager.CompleteLevel();
    }
}