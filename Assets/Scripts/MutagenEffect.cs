using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effects/Mutagen")]
public class MutagenEffect : ItemEffect
{
    public override void Apply(GameObject player)
    {
        GameManager.Instance.hasMutagen = true;
    }
}