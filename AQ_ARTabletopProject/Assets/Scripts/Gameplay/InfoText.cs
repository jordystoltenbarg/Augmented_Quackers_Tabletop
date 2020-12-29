using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New InfoText", menuName = "InfoText")]
public class InfoText : ScriptableObject
{
    [SerializeField] private LocalizedString _localizedString;
    public LocalizedString LocalizedString => _localizedString;
}
