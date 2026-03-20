using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public WaveFormation[] formations;
    public GridSpawner spawner;

    void Start()
    {
        if (formations.Length == 0 || spawner == null) return;

        WaveFormation formation = formations[Random.Range(0, formations.Length)];
        spawner.SpawnFormation(formation);
    }
}