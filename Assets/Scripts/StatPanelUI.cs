using UnityEngine;
using TMPro;

public class StatPanelUI : MonoBehaviour
{
    [Header("UI Text References")]
    public TMP_Text livesText;
    public TMP_Text moveSpeedText;
    public TMP_Text shotSpeedText;
    public TMP_Text bulletSpeedText;
    public TMP_Text maxBulletsText;
    public TMP_Text bulletPierceText;

    void Update()
    {
        if (GameManager.Instance == null) return;

        var gm = GameManager.Instance;

        livesText.text = $"Lives: {gm.lives}/{gm.maxLives}";
        moveSpeedText.text = $"Move Speed: {gm.moveSpeed:0.00}";
        shotSpeedText.text = $"Fire Rate: {gm.shotSpeed:0.00}";
        bulletSpeedText.text = $"Bullet Speed: {gm.bulletSpeed:0.00}";
        maxBulletsText.text = $"Max Bullets: {gm.maxBullets}";
        bulletPierceText.text = $"Bullet Pierce: {gm.bulletPierce}";
    }
}