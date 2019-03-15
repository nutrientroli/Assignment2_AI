using UnityEngine;
using Pathfinding;

namespace Steerings
{
	public class PathFollowing : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;
		// target-based rotational policies make little sense for this behaviour
        // (actually there's no "target" attribute)

		public Path path; // path being public can be "setted" from the outside... (e.g by pathFeeder)
		public float wayPointReachedRadius = 1f;
		public int currentWaypointIndex = 0;

		public override SteeringOutput GetSteering ()
		{ 
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = PathFollowing.GetSteering (ownKS, path, ref currentWaypointIndex, wayPointReachedRadius);

			base.applyRotationalPolicy (rotationalPolicy, result, null);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, Path path, ref int currentWaypointIndex, float wayPointReachedRadius) {
			// path shouldn't be neither null nor erroneous
			if (path == null) {
				Debug.LogError ("PathFollowing invoked with null path");
				return NULL_STEERING;
			}
			if (path.error) {
				Debug.LogError ("PathFollowing invoked with null path");
				return NULL_STEERING;
			}

			// if currentWaypoint is not valid, end of path has been reached
			if (path.vectorPath.Count == currentWaypointIndex)
				return NULL_STEERING;

			// if we're "close" to the current waypoint try going to the next one
			float distance = (ownKS.position - path.vectorPath[currentWaypointIndex]).magnitude;
			if (distance <= wayPointReachedRadius)
				currentWaypointIndex++;

            if (path.vectorPath.Count == currentWaypointIndex)
            {
                return NULL_STEERING;
                // consider using a delegate to inform that the end of the path has been reached
            }

			SURROGATE_TARGET.transform.position = path.vectorPath [currentWaypointIndex];

            return Seek.GetSteering(ownKS, SURROGATE_TARGET);

            /*
            if (currentWaypointIndex == path.vectorPath.Count - 1)
                // use arrive for the last waypoint
                return Arrive.GetSteering(ownKS, SURROGATE_TARGET, wayPointReachedRadius/2, wayPointReachedRadius*2);
            else 
			   
			*/
        }
	}
}
