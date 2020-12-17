using UnityEngine;

public class Tile : MonoBehaviour
{
    
    
    private Vector3 _waypoint = Vector3.zero;

    public Vector3 Waypoint
    {
        get
        {
            Vector3 colliderHalfSize = GetComponent<MeshCollider>().bounds.extents;
            return _waypoint + new Vector3(Random.Range(-colliderHalfSize.x, colliderHalfSize.x), 0, Random.Range(-colliderHalfSize.z, colliderHalfSize.z)) * 0.5f;
        }
    }

    void Start()
    {
        _waypoint = new Vector3(transform.position.x, 0, transform.position.z);       
    }



}
