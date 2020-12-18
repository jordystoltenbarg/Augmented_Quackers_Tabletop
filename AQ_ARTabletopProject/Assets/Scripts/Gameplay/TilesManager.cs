using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public static List<Tile> ListOfTiles = new List<Tile>();

    void Start()
    {
        //ListOfTiles = GetComponentsInChildren<Tile>().ToList();
        for (int i = 0; i < transform.childCount; i++)
            ListOfTiles.Add(transform.GetChild(i).GetComponent<Tile>());
    }
}
