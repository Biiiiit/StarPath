using UnityEngine;

[ExecuteAlways]
public class BossGridGizmo : MonoBehaviour
{
    public AttackPattern pattern;
    public SpriteRenderer background;

    void OnDrawGizmos()
    {
        if (pattern == null || background == null || pattern.grid == null)
            return;

        Bounds bounds = background.bounds;

        float cellWidth = bounds.size.x / pattern.width;
        float cellHeight = bounds.size.y / pattern.height;

        for (int y = 0; y < pattern.height; y++)
        {
            for (int x = 0; x < pattern.width; x++)
            {
                float worldX = bounds.min.x + cellWidth * x + cellWidth / 2;
                float flippedY = (pattern.height - 1 - y);
                float worldY = bounds.min.y + cellHeight * flippedY + cellHeight / 2;

                Vector3 pos = new Vector3(worldX, worldY, 0);

                bool active = pattern.Get(x, y);

                Gizmos.color = active ? Color.red : new Color(1, 1, 1, 0.2f);

                Gizmos.DrawCube(pos, new Vector3(cellWidth, cellHeight, 0.01f));
            }
        }
    }
}