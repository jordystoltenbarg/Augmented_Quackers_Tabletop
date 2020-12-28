using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;

public class InfoDisplay : MonoBehaviour
{
    //public TMP_Text infoText;
    [SerializeField] private LocalizeStringEvent _localizedStringEvent;

    void Start()
    {
        //RandomizeInfoText();
        RandomizeLocalizedString();
    }

    public void RandomizeLocalizedString()
    {
        InfoText[] infoTexts = InfoManager.singleton.infoTexts;
        int random = Random.Range(0, infoTexts.Length);
        _localizedStringEvent.StringReference = infoTexts[random].LocalizedString;

        InfoManager.singleton.SetInfoText(random);
    }

    //public void RandomizeInfoText()
    //{
    //    InfoText[] infoTexts = InfoManager.singleton.infoTexts;
    //    int random = Random.Range(0, infoTexts.Length);
    //    infoText.text = infoTexts[random].text.ToString();

    //    InfoManager.singleton.SetInfoText(random);
    //}
}
