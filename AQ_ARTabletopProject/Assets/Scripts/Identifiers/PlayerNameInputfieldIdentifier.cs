using System;
using TMPro;
using UnityEngine;

public class PlayerNameInputfieldIdentifier : MonoBehaviour
{
    public TMP_InputField inputField;
    private string _lastInput = "";

    private void Start()
    {
        inputField.onEndEdit.AddListener((string pNewName) => OnEndEdit(inputField.text));
    }

    public void OnEndEdit(string pInputText)
    {
        inputField.text = _lastInput;

        if (!isValidName(pInputText)) return;

        TTPlayer.localPlayer.ChangePlayerName(pInputText);
        _lastInput = pInputText;
    }

    private bool isValidName(string pInputText)
    {
        if (pInputText == "" || pInputText.Length < 3)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
