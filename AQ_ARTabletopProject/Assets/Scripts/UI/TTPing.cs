using Mirror;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TTPing : MonoBehaviour
{
    private TextMeshProUGUI _tm;

    private void Start()
    {
        _tm = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _tm.text = $"{(int)(NetworkTime.rtt * 1000)}ms";
    }
}
