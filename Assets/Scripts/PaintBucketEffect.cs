using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effects/PaintBucket")]
public class PaintBucketEffect : ItemEffect
{
    public override void Apply(GameObject player)
    {
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.color = Color.green;
        }
    }
}