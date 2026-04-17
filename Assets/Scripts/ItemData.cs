using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    public int livesBonus;
    public float moveSpeedBonus;
    public float shotSpeedBonus;
    public float bulletSpeedBonus;
    public float reloadSpeedBonus;
    public int maxBulletsBonus;
    public int bulletPierceBonus;
}