using UnityEngine;

public class InstanciateGameboard : MonoBehaviour
{
    public GameObject gameboardPrefab = null;
    public Transform parent = null;

    public bool instanciateOnStart = false;
    public bool instanciate = false;

    private void Start()
    {
        if (instanciateOnStart)
        {
            if (parent != null)
                Instantiate(gameboardPrefab, parent);
            else
                Instantiate(gameboardPrefab);
        }
    }

    private void Update()
    {
        if (instanciate)
        {
            if (parent != null)
                Instantiate(gameboardPrefab, parent);
            else
                Instantiate(gameboardPrefab);
            instanciate = false;
        }
    }
}
