using UnityEngine;
using Pathfinding;
using Steerings;

// Daniel Moreno
public class MARAUDER_BLACKBOARD : MonoBehaviour
{
    // orb
    [Header("Tags")]
    public string freeOrb_Tag;
    public string claimedOrb_Tag;
    public string storeOrb_Tag;
    // bearer
    public string bearerWithoutOrbe_Tag;
    public string bearerWithOrbe_Tag;
    //Points
    public string basePoints_Tag;
    public string wanderPoints_Tag;
    public string surrogate_Tag;


    [Header("Control")]
    public float bearerDetectionRadius = 100;
    public float bearerReachedRadius = 6;
    public float orbDetectionRadius = 50;
    public float orbReachedRadius = 2;
    public float deliveryPointReachedRadius = 3;
    public float loakAheadDistance = 10;
    public float vanishTime = 3;

    // listas
    [Header("Waypoint Arrays")]
    public GameObject[] baseWayPoints_Arr;
         
    public void DropOrb(GameObject orb, bool _onBase = false, GameObject basepoint = null)
    {
        orb.transform.parent = null;

        if (_onBase) {
            orb.tag = storeOrb_Tag;
            if (basepoint == null || SensingUtils.DistanceToTarget(orb, basepoint) > deliveryPointReachedRadius + 1) orb.tag = freeOrb_Tag;
        }
        else orb.tag = freeOrb_Tag;

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
