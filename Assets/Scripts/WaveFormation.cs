using UnityEngine;

[CreateAssetMenu(fileName = "WaveFormation", menuName = "Waves/Formation")]
public class WaveFormation : ScriptableObject
{
    public int rows = 3;
    public int cols = 5;

    public GameObject[] grid;

    // Editor tool
    public GameObject selectedPrefab;
}