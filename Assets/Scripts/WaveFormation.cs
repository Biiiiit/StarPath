using UnityEngine;

public enum WaveDifficulty
{
    Easy,
    Medium,
    Hard
}

[CreateAssetMenu(fileName = "WaveFormation", menuName = "Waves/Formation")]
public class WaveFormation : ScriptableObject
{
    public int rows = 3;
    public int cols = 5;

    public GameObject[] grid;

    [Header("Painting")]
    public GameObject[] availablePrefabs;
    public GameObject selectedPrefab;

    [Header("Difficulty")]
    public WaveDifficulty difficulty;
}