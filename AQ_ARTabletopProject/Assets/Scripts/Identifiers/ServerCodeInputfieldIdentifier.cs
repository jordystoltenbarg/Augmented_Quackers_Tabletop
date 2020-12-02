using TMPro;
using UnityEngine;

public class ServerCodeInputfieldIdentifier : MonoBehaviour
{
    public TMP_InputField inputField;

    private void Start()
    {
        inputField.onEndEdit.AddListener((string pNewName) => onEndEdit(inputField.text));
    }

    private void onEndEdit(string pInputText)
    {
        inputField.text = string.Empty;

        if (pInputText.Length < inputField.characterLimit) return;

        inputField.text = pInputText;
    }
}