using TMPro;
using UnityEngine;

public class PlayerNameInputfieldIdentifier : MonoBehaviour
{
    public TMP_InputField inputField;
    private string _lastInput = "";

    private void Start()
    {
        inputField.onEndEdit.AddListener((string pNewName) => onEndEdit(inputField.text));
        inputField.text = TTSettingsManager.Singleton.PlayerName;
        _lastInput = inputField.text;
    }

    private void onEndEdit(string pInputText)
    {
        inputField.text = _lastInput;

        if (!isValidName(pInputText)) return;

        TTSettingsManager.Singleton.ChangePlayerName(pInputText);
        inputField.text = pInputText;
        _lastInput = inputField.text;
    }

    private bool isValidName(string pInputText)
    {
        if (pInputText.Length < 3 || containsProfanity(pInputText))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool containsProfanity(string pInputText)
    {
        string[] strToReplace = { " ", "_", "-" };
        string check = pInputText;
        for (int i = 0; i < strToReplace.Length; i++)
            check = check.Replace(strToReplace[i], "");

        string[] bans = TTSettingsManager.Singleton.bannedWords;
        for (int i = 0; i < bans.Length; i++)
            if (check.ToLower().Contains(bans[i].ToLower()))
                return true;

        return false;
    }
}
