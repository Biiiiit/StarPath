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
    [Header("Prefabs")]
    public GameObject nodePrefab;
    public GameObject connectionPrefab;

    [Header("References")]
    public MapManager mapManager;
    public SpriteRenderer backgroundBounds; // assign the background SpriteRenderer in Inspector-

    private MapNode[,] grid;
    private int totalRows;

    void Awake()
    {
        columns = 5;
        totalRows = normalRows + 2;
        grid = new MapNode[totalRows, columns];

        if (GameProgress.Get().mapSeed == -1)
        {
            // new run — pick and save a seed
            GameProgress.Get().mapSeed = Random.Range(0, int.MaxValue);
        }

        // always apply the saved seed so the map is identical on reload
        Random.InitState(GameProgress.Get().mapSeed);
        Generate();
    }

    void Generate()
    {
        MapNode.ResetUsedAnimators();
        SpawnNodes();
        ConnectNodes();
        AssignMapManagerReferences();
        mapManager.OnMapGenerated();
    }

    void SpawnNodes()
    {
        float totalHeight = (totalRows - 1) * rowSpacing;

        int third = normalRows / 3;
        int healRow = Random.Range(1, 1 + third);
        int itemRow = Random.Range(1 + third, 1 + third * 2);
        int shopRow = Random.Range(1 + third * 2, normalRows + 1);

        // guaranteed elite in the second half
        int eliteRow = Random.Range(normalRows / 2, normalRows + 1);

        // make sure eliteRow doesn't collide with other guaranteed rows
        while (eliteRow == healRow || eliteRow == itemRow || eliteRow == shopRow)
            eliteRow = Random.Range(normalRows / 2, normalRows + 1);

        NodeType[] specialTypes = { NodeType.Heal, NodeType.Item, NodeType.Shop };
        for (int i = specialTypes.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (specialTypes[i], specialTypes[j]) = (specialTypes[j], specialTypes[i]);
        }

        Dictionary<int, NodeType> guaranteedRows = new Dictionary<int, NodeType>
        {
            { healRow,  specialTypes[0] },
            { itemRow,  specialTypes[1] },
            { shopRow,  specialTypes[2] },
            { eliteRow, NodeType.Elite  },
        };

        Dictionary<int, int> guaranteedCols = new Dictionary<int, int>();

        // pre-calculate row column counts so we can compute total height correctly
        int[] rowColumnCounts = new int[totalRows];
        for (int row = 0; row < totalRows; row++)
        {
            bool isEdgeRow = (row == 0 || row == totalRows - 1);
            rowColumnCounts[row] = isEdgeRow ? 1 : Random.Range(2, 5);
        }

        // build cumulative y positions starting from bottom
        float[] rowYPositions = new float[totalRows];
        float currentY = -totalHeight / 2f;
        for (int row = 0; row < totalRows; row++)
        {
            rowYPositions[row] = currentY;
            float dynamicRowSpacing = rowColumnCounts[row] == 4 ? rowSpacing * 0.6f : rowSpacing;
            currentY += dynamicRowSpacing;
        }

        for (int row = 0; row < totalRows; row++)
        {
            bool isEdgeRow = (row == 0 || row == totalRows - 1);
            int rowColumns = rowColumnCounts[row];

            if (guaranteedRows.ContainsKey(row))
                guaranteedCols[row] = Random.Range(0, rowColumns);

            float dynamicSpacing = rowColumns == 4 ? colSpacing : colSpacing;
            float rowWidth = (rowColumns - 1) * dynamicSpacing;

            for (int col = 0; col < rowColumns; col++)
            {
                Vector2 jitter = Vector2.zero;
                if (!isEdgeRow)
                    jitter = new Vector2(Random.Range(-0.15f, 0.15f),
                                         Random.Range(-0.15f, 0.15f));

                Vector2 pos = new Vector2(
                    -rowWidth / 2f + col * dynamicSpacing + jitter.x,
                    rowYPositions[row] + jitter.y
                );

                pos = ClampToBounds(pos);

                NodeType type;
                if (guaranteedRows.ContainsKey(row) && guaranteedCols[row] == col)
                    type = guaranteedRows[row];
                else
                    type = PickType(row);

                GameObject go = Instantiate(nodePrefab, pos, Quaternion.identity,
                                            mapManager.mapRoot.transform);

                MapNode node = go.GetComponent<MapNode>();
                node.nodeID = $"node_{row}_{col}";
                node.nodeType = type;
                node.sceneName = SceneNameFor(type);
                node.InitVisual();

                grid[row, col] = node;
            }
        }
    }

    Vector2 ClampToBounds(Vector2 pos, float nodeRadius = 1.0f)
    {
        if (backgroundBounds == null) return pos;

        Bounds b = backgroundBounds.bounds;

        return new Vector2(
            Mathf.Clamp(pos.x, b.min.x + nodeRadius, b.max.x - nodeRadius),
            Mathf.Clamp(pos.y, b.min.y + nodeRadius, b.max.y - nodeRadius)
        );
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

                // optional second connection (70 % chance)
                if (Random.value < 0.70f)
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
        if (row == 0) return NodeType.Combat;
        if (row == totalRows - 1) return NodeType.Boss;

        // elites only appear randomly in the second half
        float effectiveElite = (row >= normalRows / 2) ? eliteChance : 0f;

        float r = Random.value;
        float acc = 0f;

        acc += combatChance; if (r < acc) return NodeType.Combat;
        acc += effectiveElite; if (r < acc) return NodeType.Elite;

        return NodeType.Combat;
    }

    string SceneNameFor(NodeType type) => type switch
    {
        NodeType.Elite => "EliteBossRoom",
        NodeType.Shop => "Truckstop",
        NodeType.Item => "SpaceCasino",
        NodeType.Heal => "Restroom",
        NodeType.Boss => "BossRoom",
        _ => "GridSystem",
    };
}