using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSettingValue : MonoBehaviour
{
    [SerializeField] private Slider _settingSlider = null;
    [SerializeField] private TextMeshProUGUI _settingValue = null;

    private void OnEnable()
    {
        _settingValue.text = Mathf.FloorToInt(_settingSlider.value).ToString();
    }

    public void OnSliderValueChange()
    {
        _settingValue.text = Mathf.FloorToInt(_settingSlider.value).ToString();
    }
}
