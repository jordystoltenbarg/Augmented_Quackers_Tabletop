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
        _tm.text = $"{Mathf.FloorToInt(1 / Time.deltaTime)}fps\n{(int)(NetworkTime.rtt * 1000)}ms";
    }
}
