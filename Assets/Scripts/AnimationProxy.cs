using UnityEngine;

public class AnimationProxy : MonoBehaviour
{
    public HealingRoomUI ui;

    public void ContinueAfterChoice()
    {
        ui.ContinueAfterChoice();
    }

    public void OpenChoice()
    {
        ui.OpenChoice();
    }
}