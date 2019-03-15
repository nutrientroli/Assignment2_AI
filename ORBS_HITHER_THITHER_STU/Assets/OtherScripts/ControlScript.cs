using Pathfinding;
using UnityEngine;

public class ControlScript : MonoBehaviour
{
    private Camera cam;
    private GameObject bearerPrefab;
    private GameObject orbPrefab;
    private GameObject marauderPrefab;
    private GameObject protectorPrefab;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        bearerPrefab = Resources.Load<GameObject>("BEARER");
        orbPrefab = Resources.Load<GameObject>("ORB");
        marauderPrefab = Resources.Load<GameObject>("MARAUDER");
        protectorPrefab = Resources.Load<GameObject>("PROTECTOR");

    }

    // Update is called once per frame
    void Update()
    {
        

        
        if (Input.GetMouseButtonDown(0) && Input.GetKey("o"))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;

            GraphNode node = AstarPath.active.GetNearest(position, NNConstraint.Default).node;
            position = (Vector3)node.position;

            GameObject orb = GameObject.Instantiate(orbPrefab);
            orb.transform.position = position;
            // orb.transform.Rotate(0, 0, Random.value * 360);
        }
        

 
        if (Input.GetMouseButtonDown(0) && Input.GetKey("m"))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;

            GraphNode node = AstarPath.active.GetNearest(position, NNConstraint.Default).node;
            position = (Vector3)node.position;

            GameObject marauder = GameObject.Instantiate(marauderPrefab);
            marauder.transform.position = position;
        }

        if (Input.GetMouseButtonDown(0)  && Input.GetKey("b"))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;

            GraphNode node = AstarPath.active.GetNearest(position, NNConstraint.Default).node;
            position = (Vector3)node.position;

            GameObject bearer = GameObject.Instantiate(bearerPrefab);
            bearer.transform.position = position;
            bearer.transform.Rotate(0, 0, Random.value * 360);
        }

        if (Input.GetMouseButtonDown(0) && Input.GetKey("p"))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;

            GraphNode node = AstarPath.active.GetNearest(position, NNConstraint.Default).node;
            position = (Vector3)node.position;

            GameObject protector = GameObject.Instantiate(protectorPrefab);
            protector.transform.position = position;
            protector.transform.Rotate(0, 0, Random.value * 360);
        }

    }
}
