/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class FlockingPlusFlee : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		// the parameters for flocking
		public string idTag = "BOID";
		public float cohesionThreshold = 40f;
		public float repulsionThreshold = 10f;
		public float wanderRate = 10f;

		// the target for flee
		public GameObject repulsor;
		public float scareRadius = 40f;

		public float fleeWeight = 0.2f; 

		public override SteeringOutput GetSteering ()
		{

			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = FlockingPlusFlee.GetSteering (this.ownKS, repulsor, scareRadius, fleeWeight, idTag, cohesionThreshold, repulsionThreshold, wanderRate);
			base.applyRotationalPolicy (rotationalPolicy, result, repulsor);
			return result;

		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject repulsor, float scareRadius = 40f, float fleeWeight=0.2f, string idTag="BOID", 
			float cohesionThreshold = 40f, float repulsionThreshold = 10f,
			float wanderRate = 10f) {

			SteeringOutput fleeOutput;
			if ((ownKS.position - repulsor.transform.position).magnitude <= scareRadius) {
				fleeOutput = Flee.GetSteering (ownKS, repulsor); 
			} else {
				fleeOutput = NULL_STEERING;
			}

			SteeringOutput result = Flocking.GetSteering (ownKS, idTag, cohesionThreshold, repulsionThreshold, wanderRate); 

			// beware, Flocking may return NULL_STEERING. In that case, just apply flee
			if (result == NULL_STEERING) {
				return  fleeOutput;
			}

			result.linearAcceleration = result.linearAcceleration * (1 - fleeWeight) + fleeOutput.linearAcceleration * fleeWeight;
			result.angularAcceleration = result.angularAcceleration * (1 - fleeWeight) + fleeOutput.angularAcceleration * fleeWeight;

			return result;
		}


	}
}
