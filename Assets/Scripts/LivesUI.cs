using UnityEngine;

public class LivesUI : MonoBehaviour
{
    public GameObject activeLifePrefab;
    public GameObject inactiveLifePrefab;

    public Transform healthParent;

    private GameObject[] lifeSlots;

    void Start()
    {
        GenerateLives();
    }

    void GenerateLives()
    {
        int max = GameManager.Instance.maxLives;
        int current = GameManager.Instance.lives;

        lifeSlots = new GameObject[max];

        for (int i = 0; i < max; i++)
        {
            GameObject prefab = i < current ? activeLifePrefab : inactiveLifePrefab;
            GameObject life = Instantiate(prefab, healthParent);
            lifeSlots[i] = life;
        }
    }

    public void LoseLife()
    {
        int current = GameManager.Instance.lives;

        if (current <= 0) return;

        int index = current - 1;

        Destroy(lifeSlots[index]);

        lifeSlots[index] = Instantiate(inactiveLifePrefab, healthParent);
        lifeSlots[index].transform.SetSiblingIndex(index);
    }

    public void GainLife()
    {
        int current = GameManager.Instance.lives;
        int max = GameManager.Instance.maxLives;

        if (current >= max) return;

        int index = current;

        Destroy(lifeSlots[index]);

        lifeSlots[index] = Instantiate(activeLifePrefab, healthParent);
        lifeSlots[index].transform.SetSiblingIndex(index);
    }
}