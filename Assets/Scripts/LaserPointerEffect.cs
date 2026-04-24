using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effects/Laser Pointer")]
public class LaserPointerEffect : ItemEffect
{
    public override void Apply(GameObject player)
    {
        if (player == null) return;

        GameManager.Instance.hasLaserPointer = true;
    }
}