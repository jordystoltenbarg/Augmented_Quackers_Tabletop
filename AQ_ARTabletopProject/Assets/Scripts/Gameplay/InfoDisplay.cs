using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoDisplay : MonoBehaviour
{
    public TMP_Text infoText;



    void Start()
    {
        RandomizeInfoText();       
    }

    public void RandomizeInfoText()
    {
        InfoText[] infoTexts = InfoManager.singleton.infoTexts;
        int random = Random.Range(0, infoTexts.Length);
        infoText.text = infoTexts[random].text.ToString();

        InfoManager.singleton.SetInfoText(random);
    }
}
