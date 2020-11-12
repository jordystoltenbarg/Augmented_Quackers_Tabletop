using Mirror;
using TMPro;
using UnityEngine;

public class TTPlayer : NetworkBehaviour
{
    public static TTPlayer localPlayer;

    private string _playerName = "";
    public string playerName => _playerName;

    private void Awake()
    {
        TTPlayer[] ttps = FindObjectsOfType<TTPlayer>();
        if (ttps.Length > 1)
            Destroy(gameObject);

        localPlayer = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (isLocalPlayer)
            print("asd");
    }

    public void ChangePlayerName(string pNewName)
    {
        _playerName = pNewName;
    }
}
