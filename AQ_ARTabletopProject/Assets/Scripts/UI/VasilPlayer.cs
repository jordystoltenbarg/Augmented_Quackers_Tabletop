using UnityEngine;
using UnityEngine.UI;

public class VasilPlayer : MonoBehaviour
{
    [SerializeField]
    private Sprite _characterImage = null;
    public Sprite CharacterImage { get { return _characterImage; } }
    [SerializeField]
    private GameObject _turnOrderPositionPrefab = null;
    public GameObject TurnOrderPositionPrefab { get { return _turnOrderPositionPrefab; } }
    [SerializeField]
    private string _name = "";
    public string Name { get { return _name; } }

    [HideInInspector]
    public int turnsToSkip = 0;
    [HideInInspector]
    public int extraTurns = 0;
    [HideInInspector]
    public bool hasActedThisTurn = false;

    private void OnEnable()
    {
        _turnOrderPositionPrefab.GetComponentInChildren<TurnOrderPlayerAvatar>().SetPlayer(this);
    }

    public int RollDie(int pSides = 6)
    {
        int rand = Random.Range(1, pSides + 1);
        return rand;
    }
}
