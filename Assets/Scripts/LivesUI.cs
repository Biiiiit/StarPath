using UnityEngine;

public class LivesUI : MonoBehaviour
{
    public GameObject activeLifePrefab;
    public GameObject inactiveLifePrefab;

    public Transform healthParent;

    public int maxLives = 7;
    public int currentLives = 3;

    private GameObject[] lifeSlots;

    void Start()
    {
        GenerateLives();
    }

    void GenerateLives()
    {
        lifeSlots = new GameObject[maxLives];

        // spawn ONLY active lives at start
        for (int i = 0; i < currentLives; i++)
        {
            GameObject life = Instantiate(activeLifePrefab, healthParent);
            lifeSlots[i] = life;
        }
    }

    public void LoseLife()
    {
        if (currentLives <= 0) return;

        currentLives--;

        // destroy active icon
        Destroy(lifeSlots[currentLives]);

        // replace with inactive icon at same position
        GameObject inactive = Instantiate(inactiveLifePrefab, healthParent);
        inactive.transform.SetSiblingIndex(currentLives);

        lifeSlots[currentLives] = inactive;
    }

    public void GainLife()
    {
        if (currentLives >= maxLives) return;

        // remove inactive if exists
        if (lifeSlots[currentLives] != null)
            Destroy(lifeSlots[currentLives]);

        GameObject active = Instantiate(activeLifePrefab, healthParent);
        active.transform.SetSiblingIndex(currentLives);

        lifeSlots[currentLives] = active;

        currentLives++;
    }
}