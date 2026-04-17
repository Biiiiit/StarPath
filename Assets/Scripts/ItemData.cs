using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    public int livesBonus;
    public float moveSpeedBonus;
    public float fireRate;
    public float bulletSpeedBonus;
    public int maxBulletsBonus;
    public int bulletPierceBonus;
}