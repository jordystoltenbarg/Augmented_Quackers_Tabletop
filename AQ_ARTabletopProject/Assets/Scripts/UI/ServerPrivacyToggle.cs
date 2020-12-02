using UnityEngine;
using UnityEngine.UI;

public class ServerPrivacyToggle : MonoBehaviour
{
    [SerializeField] private Color _publicCol;
    [SerializeField] private Color _privateCol;

    [SerializeField] private Sprite _public;
    [SerializeField] private Sprite _private;

    private Image _image;
    private static bool _isPrivate = false;

    void Start()
    {
        _image = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(toggle);

        _image.color = (_isPrivate) ? _privateCol : _publicCol;
        TTSettingsManager.Singleton.ToggleServerPrivacySetting(_isPrivate);
    }

    private void toggle()
    {
        _isPrivate = !_isPrivate;
        _image.color = (_isPrivate) ? _privateCol : _publicCol;
        TTSettingsManager.Singleton.ToggleServerPrivacySetting(_isPrivate);
    }
}