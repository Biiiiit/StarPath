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

    // Add this:
    public static GameProgress Get()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("GameProgress");
            Instance = go.AddComponent<GameProgress>();
            DontDestroyOnLoad(go);
        }
        return Instance;
    }

    public bool IsCompleted(string nodeID)
    {
        return completedNodes.Contains(nodeID);
    }

    public void ResetProgress()
    {
        completedNodes.Clear();
        currentNodeID = "";
    }

    public void CompleteCurrentNode()
    {
        if (!completedNodes.Contains(currentNodeID))
            completedNodes.Add(currentNodeID);
    }
}