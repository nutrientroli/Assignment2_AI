using UnityEngine;
using Pathfinding;
using Steerings;

// Daniel Moreno
public class MARAUDER_BLACKBOARD : MonoBehaviour
{

    /* COMPLETE */
    // orb
    [Header("Tags")]
    public string freeOrb_Tag;
    public string claimedOrb_Tag;
    public string storeOrb_Tag;
    // bearer
    public string bearerWithoutOrbe;
    public string bearerWithoutOrbe_Tag;

    public float vanishTime = 3;

    [Header("Control")]
    public float bearerDetectionRadius = 100;
    public float bearerReachedRadius = 6;
    public float orbDetectionRadius = 50;
    public float orbReachedRadius = 2;
    public float deliveryPointReachedRadius = 3;
    public float loakAheadDistance = 10;

    // listas
    [Header("Waypoint Arrays")]
    public GameObject[] baseWayPoints_Arr;

         
    void Awake()
    {
        // Delete if not necesary
        if (baseWayPoints_Arr.Length == 0)
            Debug.LogError("There is no nodes set to the baseWaypoints of " + gameObject);
    }


    public void DropOrb (GameObject orb, bool _onBase = false)
    {
        orb.transform.parent = null;

        if (_onBase)
            orb.tag = storeOrb_Tag;
        else
            orb.tag = "FREE_ORB";

        Debug.LogError( this.gameObject.name + " " + orb.transform.tag);

        GraphNode node = AstarPath.active.GetNearest(orb.transform.position, NNConstraint.Default).node;
        orb.transform.position = (Vector3)node.position;
    }

    public void GrabOrb (GameObject _orb , Transform _parentObject)
    {
        _orb.transform.parent = _parentObject;
        _orb.tag = claimedOrb_Tag;
    }

    public GameObject GetRandomBasePoint ()
    {
       return GetRandomObjectOfArray(baseWayPoints_Arr);
    }

    private GameObject GetRandomObjectOfArray(GameObject[] _inputArray)
    {
        GameObject _object;
        int _randomIndex;

        _randomIndex = Random.Range(0, _inputArray.Length);

        _object = _inputArray[_randomIndex];

        return _object;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, bearerReachedRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, bearerDetectionRadius);
    }

}
