/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class LinearRepulsion : SteeringBehaviour
	{
		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;
		// FT and FTI make no sense in this behaviour

		public string idTag = "REPULSIVE";
		public float repulsionThreshold = 20f;   // at which distance does repulsion start?

		public override SteeringOutput GetSteering ()
		{
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = LinearRepulsion.GetSteering (this.ownKS, this.idTag, this.repulsionThreshold);
			base.applyRotationalPolicy (rotationalPolicy, result, null);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, string tag, float repulsionThreshold) {
			Vector3 directionToTarget;
			float distanceToTarget;
			float repulsionStrength = 0;
			int activeTargets = 0;  // number of targets inside the repulsive threshold

			SteeringOutput result = new SteeringOutput ();

			// get all potential "targets" (all the repulsive objects)
			GameObject[] targets = GameObject.FindGameObjectsWithTag (tag);

			// iterate over all repulsive targets
			foreach (GameObject target in targets) {
				// do not take yourself into account
				if (target== ownKS.gameObject) 
					continue;

				directionToTarget = target.transform.position - ownKS.position;
				distanceToTarget = directionToTarget.magnitude;
				if (distanceToTarget <= repulsionThreshold) {
					// a repulsive object is too close. Do someting
					activeTargets++;
					repulsionStrength = ownKS.maxAcceleration * (repulsionThreshold - distanceToTarget) / repulsionThreshold;
					result.linearAcceleration = result.linearAcceleration - directionToTarget.normalized * repulsionStrength;
				}

				
			} // end of iteration over all repulsive targets 

			if (activeTargets > 0) {
				// clip (if necessary) and return 
				if (result.linearAcceleration.magnitude > ownKS.maxAcceleration)
					result.linearAcceleration = result.linearAcceleration.normalized * ownKS.maxAcceleration;
				return result;
			} else
				return NULL_STEERING;
		}

	}
}