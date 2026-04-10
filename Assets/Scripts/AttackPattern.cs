using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Attack Pattern")]
public class AttackPattern : ScriptableObject
{
    public int width = 10;
    public int height = 10;

    public bool[] grid;

    public void Init()
    {
        if (grid == null || grid.Length != width * height)
            grid = new bool[width * height];
    }

    public bool Get(int x, int y)
    {
        return grid[y * width + x];
    }

    public void Set(int x, int y, bool value)
    {
        grid[y * width + x] = value;
    }
}