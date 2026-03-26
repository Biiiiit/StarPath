using UnityEngine;
using TMPro;

public class CreditsUI : MonoBehaviour
{
    void OnEnable()
    {
        if (CreditManager.Instance != null)
        {
            CreditManager.Instance.RegisterText(GetComponent<TMP_Text>());
        }
    }
}