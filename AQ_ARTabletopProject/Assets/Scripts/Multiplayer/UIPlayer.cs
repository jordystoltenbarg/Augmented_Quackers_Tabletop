using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    Player player;


    public void SetPlayer (Player player)
    {
        this.player = player;
        text.text = "Player " + player.playerIndex.ToString();
    }
}
