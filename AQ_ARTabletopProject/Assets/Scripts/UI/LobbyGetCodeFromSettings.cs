using UnityEngine;
using TMPro;

public class LobbyGetCodeFromSettings : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = TTSettingsManager.Singleton.ServerCode;
    }
}
