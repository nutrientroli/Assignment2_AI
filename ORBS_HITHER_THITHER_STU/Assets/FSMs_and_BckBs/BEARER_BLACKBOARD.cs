using UnityEngine;
using Pathfinding;

public class BEARER_BLACKBOARD : MonoBehaviour
{

    private GameObject[] dropPoints;
    private GameObject[] exitPoints;

    public GameObject theOrb; // the orb this bearer is transporting (if any)
    public float marauderDetectionRadius = 50;
    public float pointReachedRadius = 3; // for exit and delivery points
    public float vanishTime = 3; // seconds until gameObject destroyed

    void Awake()
    {
        dropPoints = GameObject.FindGameObjectsWithTag("DROP");
        exitPoints = GameObject.FindGameObjectsWithTag("EXIT");
        theOrb = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetRandomDropPoint()
    {
        return dropPoints[Random.Range(0, dropPoints.Length)];
    }

    public GameObject GetRandomExitPoint()
    {
        return exitPoints[Random.Range(0, exitPoints.Length)];
    }

    public void DropOrb (GameObject position)
    {
        theOrb.transform.parent = null;

        GraphNode node = AstarPath.active.GetNearest(position.transform.position, NNConstraint.Default).node;
        theOrb.transform.position = (Vector3)node.position;
        theOrb.tag = "FREE_ORB";
        theOrb = null; 
    }

   

}
