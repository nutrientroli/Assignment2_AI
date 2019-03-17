using UnityEngine;
using Pathfinding;

public class PROTECTOR_BLACKBOARD : MonoBehaviour
{
    [Header("Tags")]
    public string freeOrb_Tag;
    public string claimedOrb_Tag;
    public string storeOrb_Tag;
    public string marauder_Tag;
    public string marauderCaught_Tag;

    [Header("Control")]
    public float orbDetectionRadius = 200;
    public float orbReachedRadius = 2;
    public float deliveryPointReachedRadius = 3;
    public float killingPointReachedRadius = 5;
    public float marauderDetectionRadius = 100;
    public float marauderReachedRadius = 4;
    public float lookAheadDistance = 15;

    [Header("Waypoint Arrays")]
    public GameObject[] baseWayPoints_Arr;
    public GameObject[] killingWayPoints_Arr;


    void Awake()
    {

    }


    public void DropOrb(GameObject orb, bool _onBase = false)
    {
        orb.transform.parent = null;

        if (_onBase)
            orb.tag = storeOrb_Tag;
        else
            orb.tag = "FREE_ORB";

        GraphNode node = AstarPath.active.GetNearest(orb.transform.position, NNConstraint.Default).node;
        orb.transform.position = (Vector3)node.position;
    }

    public void DropMarauder(GameObject marauder)
    {
        marauder.transform.parent = null;
        GraphNode node = AstarPath.active.GetNearest(marauder.transform.position, NNConstraint.Default).node;
        marauder.transform.position = (Vector3)node.position;
    }

    public void GrabOrb(GameObject _orb, Transform _parentObject)
    {
        _orb.transform.parent = _parentObject;
        _orb.tag = claimedOrb_Tag;
    }

    public void GrabMarauder(GameObject _marauder, Transform _parentObject)
    {
        _marauder.transform.parent = _parentObject;
        _marauder.tag = marauderCaught_Tag;
    }

    public GameObject GetRandomBasePoint()
    {
        return GetRandomObjectOfArray(baseWayPoints_Arr);
    }

    public GameObject GetRandomKillingPoint()
    {
        return GetRandomObjectOfArray(killingWayPoints_Arr);
    }

    private GameObject GetRandomObjectOfArray(GameObject[] _inputArray)
    {
        GameObject _object;
        int _randomIndex;

        _randomIndex = Random.Range(0, _inputArray.Length);

        _object = _inputArray[_randomIndex];

        return _object;
    }
}
