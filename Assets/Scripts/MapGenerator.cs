using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Layout")]
    public int columns = 3;
    public int normalRows = 6;
    public float colSpacing = 2.5f;
    public float rowSpacing = 2.0f;

    [Header("Node chances (0–1, must sum ≤ 1)")]
    public float combatChance = 0.45f;
    public float eliteChance = 0.15f;
    public float shopChance = 0.15f;
    public float itemChance = 0.10f;
    public float healChance = 0.15f;

    [Header("Prefabs")]
    public GameObject nodePrefab;
    public GameObject connectionPrefab;

    [Header("References")]
    public MapManager mapManager;

    private MapNode[,] grid;
    private int totalRows;

    void Awake()
    {
        columns = 4; // max possible, grid needs to be this wide
        totalRows = normalRows + 2;
        grid = new MapNode[totalRows, columns];
        Generate();
    }

    void Generate()
    {
        SpawnNodes();
        ConnectNodes();
        AssignMapManagerReferences();
        mapManager.OnMapGenerated();
    }

    void SpawnNodes()
    {
        float totalHeight = (totalRows - 1) * rowSpacing;
        Vector2 origin = new Vector2(0f, -totalHeight / 2f);

        for (int row = 0; row < totalRows; row++)
        {
            bool isEdgeRow = (row == 0 || row == totalRows - 1);

            int rowColumns = isEdgeRow ? 1 : Random.Range(2, 4); // 2 or 3 per normal row

            // center the columns for this row
            float rowWidth = (rowColumns - 1) * colSpacing;

            for (int col = 0; col < rowColumns; col++)
            {
                Vector2 jitter = Vector2.zero;
                if (!isEdgeRow)
                    jitter = new Vector2(Random.Range(-0.25f, 0.25f),
                                         Random.Range(-0.15f, 0.15f));

                Vector2 pos = origin
                    + new Vector2(-rowWidth / 2f + col * colSpacing, row * rowSpacing)
                    + jitter;

                NodeType type = PickType(row);

                GameObject go = Instantiate(nodePrefab, pos, Quaternion.identity,
                                            mapManager.mapRoot.transform);

                MapNode node = go.GetComponent<MapNode>();
                node.nodeID = $"node_{row}_{col}";
                node.nodeType = type;
                node.sceneName = SceneNameFor(type);

                grid[row, col] = node;
            }
        }
    }

    // ── 2. Connect ────────────────────────────────────────────────────────────
    // Rules (same as Slay the Spire):
    //   • each node connects to 1–2 nodes in the next row
    //   • connections may not CROSS each other
    //   • every node in the next row must have at least one parent
    void ConnectNodes()
    {
        for (int row = 0; row < totalRows - 1; row++)
        {
            List<MapNode> fromNodes = GetRow(row);
            List<MapNode> toNodes = GetRow(row + 1);

            if (fromNodes.Count == 1 && toNodes.Count > 1)
            {
                // start node fans out to all columns
                foreach (MapNode to in toNodes)
                    Link(fromNodes[0], to, row, row + 1);
                continue;
            }

            if (toNodes.Count == 1)
            {
                // all columns converge on boss
                foreach (MapNode from in fromNodes)
                    Link(from, toNodes[0], row, row + 1);
                continue;
            }

            // track which "to" indices have been claimed to avoid crossings
            HashSet<int> claimed = new HashSet<int>();

            for (int i = 0; i < fromNodes.Count; i++)
            {
                // primary connection: same column or one step, no crossing
                int primary = Mathf.Clamp(i, 0, toNodes.Count - 1);
                claimed.Add(primary);
                Link(fromNodes[i], toNodes[primary], row, row + 1);

                // optional second connection (65 % chance)
                if (Random.value < 0.65f)
                {
                    List<int> candidates = new List<int>();

                    // can only go to adjacent columns without crossing neighbours
                    for (int j = primary - 1; j <= primary + 1; j++)
                    {
                        if (j < 0 || j >= toNodes.Count) continue;
                        if (claimed.Contains(j)) continue;
                        if (WouldCross(i, j, fromNodes.Count, toNodes.Count)) continue;
                        candidates.Add(j);
                    }

                    if (candidates.Count > 0)
                    {
                        int pick = candidates[Random.Range(0, candidates.Count)];
                        claimed.Add(pick);
                        Link(fromNodes[i], toNodes[pick], row, row + 1);
                    }
                }
            }

            // ensure every "to" node has at least one parent
            for (int j = 0; j < toNodes.Count; j++)
            {
                if (!claimed.Contains(j))
                {
                    int closestFrom = Mathf.Clamp(j, 0, fromNodes.Count - 1);
                    Link(fromNodes[closestFrom], toNodes[j], row, row + 1);
                }
            }
        }
    }

    void Link(MapNode from, MapNode to, int fromRow, int toRow)
    {
        if (from == null || to == null) return; // add this
        if (from.connectedNodes.Contains(to)) return;
        from.connectedNodes.Add(to);

        GameObject connGO = Instantiate(connectionPrefab, mapManager.mapRoot.transform);
        MapConnection conn = connGO.GetComponent<MapConnection>();
        conn.fromNode = from;
        conn.toNode = to;
    }

    // Slay the Spire crossing rule: edge (i→j) crosses edge (i'→j') when
    // (i < i' && j > j') or (i > i' && j < j')
    bool WouldCross(int fromIdx, int toIdx, int fromCount, int toCount)
    {
        // check against all already-established connections in this row
        // (simplified: just check immediate neighbours)
        if (fromIdx > 0 && toIdx < fromIdx) return true;
        if (fromIdx < fromCount - 1 && toIdx > fromIdx) return false;
        return false;
    }

    // ── 3. Hook up MapManager ─────────────────────────────────────────────────
    void AssignMapManagerReferences()
    {
        mapManager.startNode = grid[0, 0];
    }

    // ── helpers ───────────────────────────────────────────────────────────────
    List<MapNode> GetRow(int row)
    {
        var list = new List<MapNode>();
        for (int col = 0; col < columns; col++)
            if (grid[row, col] != null)
                list.Add(grid[row, col]);
        return list;
    }

    NodeType PickType(int row)
    {
        if (row == 0) return NodeType.Combat;   // start
        if (row == totalRows - 1) return NodeType.Boss;

        // elite rooms only appear in later half
        float effectiveElite = (row >= normalRows / 2) ? eliteChance : 0f;

        float r = Random.value;
        float acc = 0f;

        acc += combatChance; if (r < acc) return NodeType.Combat;
        acc += effectiveElite; if (r < acc) return NodeType.Elite;
        acc += shopChance; if (r < acc) return NodeType.Shop;
        acc += itemChance; if (r < acc) return NodeType.Item;
        acc += healChance; if (r < acc) return NodeType.Heal;

        return NodeType.Combat; // fallback
    }

    string SceneNameFor(NodeType type) => type switch
    {
        NodeType.Elite => "EliteBossRoom",
        NodeType.Shop => "SpaceCasino",
        NodeType.Item => "Truckstop",
        NodeType.Heal => "Restroom",
        NodeType.Boss => "BossRoom",
        _ => "GridSystem",
    };
}