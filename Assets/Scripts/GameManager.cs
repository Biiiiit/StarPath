using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Player stats
    public int lives = 3;
    public int maxLives = 3;
    public int credits = 0;
    public float maxAlienSpeed = 5.5f;

    public float moveSpeed = 5f;
    public float shotSpeed = 8f;
    public float bulletSpeed = 8f;
    public float reloadSpeed = 1f;
    public int maxBullets = 1;
    public int bulletPierce = 1;

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
}