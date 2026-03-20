using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public WaveFormation[] formations;
    public GridSpawner spawner;

    void Start()
    {
        WaveFormation formation = formations[Random.Range(0, formations.Length)];
        spawner.SpawnFormation(formation);
    }
}