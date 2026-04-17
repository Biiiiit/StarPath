using System.Collections.Generic;
using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance;

    public string currentNodeID;
    public RuntimeAnimatorController selectedBackground;
    public List<string> completedNodes = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsCompleted(string nodeID)
    {
        return completedNodes.Contains(nodeID);
    }

    public void CompleteCurrentNode()
    {
        if (!completedNodes.Contains(currentNodeID))
            completedNodes.Add(currentNodeID);
    }
}