using UnityEngine;

public class LevelBackgroundLoader : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        if (GameProgress.Instance.selectedBackground != null)
        {
            animator.runtimeAnimatorController =
                GameProgress.Instance.selectedBackground;
        }
    }
}