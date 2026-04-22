using System.Collections.Generic;
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
        if (formations.Length == 0 || spawner == null) return;

        int completed = GameProgress.Instance.completedNodes.Count;

        WaveDifficulty targetDifficulty;

        if (completed <= 1)
            targetDifficulty = WaveDifficulty.Easy;
        else if (completed <= 3)
            targetDifficulty = WaveDifficulty.Medium;
        else
            targetDifficulty = WaveDifficulty.Hard;

        List<WaveFormation> possible = new List<WaveFormation>();

        foreach (var wave in formations)
        {
            if (wave.difficulty == targetDifficulty)
                possible.Add(wave);
        }

        if (possible.Count == 0) return;

        WaveFormation selected =
            possible[Random.Range(0, possible.Count)];

        spawner.SpawnFormation(selected);
    }
}