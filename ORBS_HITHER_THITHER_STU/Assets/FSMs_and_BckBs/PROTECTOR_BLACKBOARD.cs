using UnityEngine;
using Pathfinding;

public class PROTECTOR_BLACKBOARD : MonoBehaviour
{
    /* COMPLETE */

    public float orbDetectionRadius = 200;
    public float orbReachedRadius = 2;
    public float deliveryPointReachedRadius = 3;
    public float killingPointReachedRadius = 5;
    public float marauderDetectionRadius = 100;
    public float marauderReachedRadius = 4;
    public float lookAheadDistance = 15;


    void Awake()
    {

    }

  
    public void DropOrb(GameObject orb)
    {
        orb.transform.parent = null;
        orb.tag = "FREE_ORB";
        GraphNode node = AstarPath.active.GetNearest(orb.transform.position, NNConstraint.Default).node;
        orb.transform.position = (Vector3)node.position;
    }
}
