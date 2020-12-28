using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SmoothenSlider : MonoBehaviour, IBeginDragHandler, IDropHandler, IDragHandler
{
    [SerializeField] private AudioMixer _audioMixer = null;
    private Slider _slider = null;
    private AudioManager _audioManager = null;
    private int _previousValue = 0;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.value = PlayerPrefs.GetInt(transform.parent.name);
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnEnable()
    {
        _slider.wholeNumbers = false;
    }

    private void OnDisable()
    {
        _slider.value = Mathf.FloorToInt(_slider.value);
        _slider.wholeNumbers = true;
    }

    public void OnBeginDrag(PointerEventData pEventData)
    {
        if (pEventData.pointerDrag != null)
        {
            _slider.wholeNumbers = false;
        }
    }

    public void OnDrag(PointerEventData pEventData)
    {
        if (pEventData.pointerDrag != null)
        {
            int currentValue = Mathf.FloorToInt(_slider.value);

            if (_audioManager && currentValue != _previousValue)
            {
                float volume = Mathf.Clamp(currentValue * 0.1f, 0.0001f, 1f);
                volume = Mathf.Log10(volume) * TTSettingsManager.Singleton.AudioVolumeModifier;
                switch (_slider.transform.parent.name)
                {
                    case "SFX":
                        _audioMixer.SetFloat("sfxVolume", volume);
                        break;
                    case "Music":
                        _audioMixer.SetFloat("musicVolume", volume);
                        break;
                    case "Dialogue":
                        _audioMixer.SetFloat("dialogueVolume", volume);
                        break;
                }
                if (currentValue != 0)
                    _audioManager.Play("buttonsound");
                _previousValue = currentValue;
            }
        }
    }

    public void OnDrop(PointerEventData pEventData)
    {
        if (pEventData.pointerDrag != null)
        {
            _slider.value = Mathf.FloorToInt(_slider.value);
            _slider.wholeNumbers = true;
            PlayerPrefs.SetInt(transform.parent.name, (int)_slider.value);
        }
    }
}
