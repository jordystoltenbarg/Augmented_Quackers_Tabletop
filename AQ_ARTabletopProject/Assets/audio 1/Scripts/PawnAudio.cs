using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAudio : MonoBehaviour

{    public GameObject moving;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tile")
        { 
            print("Something gone wrong!");
            FindObjectOfType<AudioManager>().Play("RubberDuck");
        } 
    }
}
