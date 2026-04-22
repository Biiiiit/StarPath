using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public string nodeID;

    public List<MapNode> connectedNodes = new List<MapNode>();

    public bool isUnlocked = false;
    public bool isCompleted = false;

    public NodeType nodeType;
    public string sceneName;

    public GameObject checkmarkSprite;
    public RuntimeAnimatorController[] combatAnimators;
    public RuntimeAnimatorController bossAnimator;
    public RuntimeAnimatorController startAnimator;
    public RuntimeAnimatorController backgroundController;
    private static HashSet<RuntimeAnimatorController> usedAnimators = new HashSet<RuntimeAnimatorController>();

    public Sprite shopSprite;
    public Sprite itemSprite;
    public Sprite healSprite;

    private SpriteRenderer sr;
    private Animator anim;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public static void ResetUsedAnimators()
    {
        usedAnimators.Clear();
    }

    public void RefreshVisual()
    {
        // static sprite types — disable animator
        if (nodeType == NodeType.Shop || nodeType == NodeType.Item || nodeType == NodeType.Heal)
        {
            if (anim != null) anim.enabled = false;

            sr.sprite = nodeType switch
            {
                NodeType.Shop => shopSprite,
                NodeType.Item => itemSprite,
                _ => healSprite,
            };
        }
        else
        {
            // animated types
            if (anim != null)
            {
                anim.enabled = true;

                if (nodeType == NodeType.Boss)
                {
                    anim.runtimeAnimatorController = bossAnimator;
                }
                else if (nodeID == "node_0_0") // start node always uses startAnimator
                {
                    anim.runtimeAnimatorController = startAnimator;
                }
                else
                {
                    // pick a random unused animator
                    List<RuntimeAnimatorController> available = new List<RuntimeAnimatorController>();
                    foreach (var a in combatAnimators)
                        if (a != null && !usedAnimators.Contains(a))
                            available.Add(a);

                    // if all are used, reset the pool (fallback)
                    if (available.Count == 0)
                    {
                        usedAnimators.Clear();
                        available.AddRange(combatAnimators);
                    }

                    var picked = available[Random.Range(0, available.Count)];
                    usedAnimators.Add(picked);
                    anim.runtimeAnimatorController = picked;
                }

                backgroundController = anim.runtimeAnimatorController;
            }
        }

        // tint by state
        if (isCompleted)
        {
            sr.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            if (checkmarkSprite != null) checkmarkSprite.SetActive(true);
        }
        else if (isUnlocked)
        {
            sr.color = Color.white;
            if (checkmarkSprite != null) checkmarkSprite.SetActive(false);
        }
        else
        {
            sr.color = Color.gray;
            if (checkmarkSprite != null) checkmarkSprite.SetActive(false);
        }
    }
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