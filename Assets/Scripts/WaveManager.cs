using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public WaveFormation[] formations;
    public GridSpawner spawner;

    void Start()
    {
        SpawnRandomWave();
    }

    public void SpawnRandomWave()
    {
        if (formations.Length == 0) return;
        if (spawner == null) return;

        WaveFormation randomWave =
            formations[Random.Range(0, formations.Length)];

        spawner.SpawnFormation(randomWave);
    }
}