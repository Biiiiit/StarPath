using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    void Start()
    {
        foreach (ItemData item in GameManager.Instance.inventory)
        {
            if (item.customEffect != null)
            {
                item.customEffect.Apply(gameObject);
            }
        }
    }
}