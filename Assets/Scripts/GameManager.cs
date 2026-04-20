using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Player stats
    public int lives = 3;
    public int maxLives = 3;
    public int credits = 0;
    public float maxAlienSpeed = 5.2f;

    public float moveSpeed = 5f;
    public float shotSpeed = 8f;
    public float bulletSpeed = 8f;
    public float reloadSpeed = 1f;
    public int maxBullets = 1;
    public int bulletPierce = 1;
    public List<ItemData> inventory = new List<ItemData>();


    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }
    }

    public void ApplyItem(ItemData item)
    {
        lives += item.livesBonus;
        moveSpeed += item.moveSpeedBonus;
        bulletSpeed += item.bulletSpeedBonus;
        maxBullets += item.maxBulletsBonus;
        bulletPierce += item.bulletPierceBonus;

        Debug.Log("Picked up: " + item.itemName);
    }

    public void AddItem(ItemData item)
    {
        inventory.Add(item);
    }

    public void RemoveItem(ItemData item)
    {
        inventory.Remove(item);
    }
}