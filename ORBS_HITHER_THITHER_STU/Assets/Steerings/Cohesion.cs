/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class Cohesion : SteeringBehaviour
	{
		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		public string idTag = "ATTRACTIVE";
		public float cohesionThreshold = 20f;
	
		public override SteeringOutput GetSteering ()
		{
			if (this.ownKS == null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = Cohesion.GetSteering (this.ownKS, this.idTag, this.cohesionThreshold);
			base.applyRotationalPolicy (rotationalPolicy, result, null);
			return result;
		}


		public static SteeringOutput GetSteering (KinematicState ownKS, string tag, float cohesionThreshold=20f) {
			Vector3 centerOfMass = Vector3.zero;
			int count = 0;
			float distanceToMate;

			// get all your mates (potential targets) 
			GameObject [] mates = GameObject.FindGameObjectsWithTag(tag);

			// iterate to compute center of mass
			foreach(GameObject mate in mates) {
				// skip yourself...
				//if (mate == ownKS.gameObject)
					//continue;

				// Only consider close mates. Disregard far ones
				distanceToMate = (mate.transform.position - ownKS.position).magnitude;
				if (distanceToMate < cohesionThreshold) {
					centerOfMass = centerOfMass + mate.transform.position;
					count++;
				}
			}
				

			if (count == 0)
				return NULL_STEERING;

			centerOfMass = centerOfMass / count;

			// generate a surrogate target and delegate to seek or arrive...
			SURROGATE_TARGET.transform.position = centerOfMass;


			return Seek.GetSteering (ownKS, SURROGATE_TARGET);
		}

	}
}