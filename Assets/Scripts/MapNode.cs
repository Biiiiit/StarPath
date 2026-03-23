using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public List<MapNode> connectedNodes; // waar je naartoe kan
    public bool isCompleted = false;
    public bool isUnlocked = false;

    public NodeType nodeType;
}

public enum NodeType
{
    Combat,
    Elite,
    Shop,
    Boss,
    Item,
    Heal
}