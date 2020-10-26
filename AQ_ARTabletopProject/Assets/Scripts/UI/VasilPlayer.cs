using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VasilPlayer : MonoBehaviour
{
    [SerializeField]
    private string _name;
    public string Name { get { return _name; } }

    [HideInInspector]
    public int turnsToSkip;
    [HideInInspector]
    public int extraTurns;

    void Start()
    {

    }

    void Update()
    {

    }
}
