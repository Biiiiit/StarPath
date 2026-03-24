using UnityEngine;
using TMPro;

public class CreditsUI : MonoBehaviour
{
    void OnEnable()
    {
        if (CreditManager.Instance != null)
            CreditManager.Instance.SetText(GetComponent<TMP_Text>());
    }
}