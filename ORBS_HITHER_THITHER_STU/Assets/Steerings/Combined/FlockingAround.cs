/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class FlockingAround : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		// the parameters for flocking
		public string idTag = "BOID";
		public float cohesionThreshold = 40f;
		public float repulsionThreshold = 10f;
		public float wanderRate = 10f;

		public float vmWeight = 0.08f;
		public float rpWeight = 0.46f;
		public float coWeight = 0.23f;
		public float wdWeight = 0.23f;

		// the target for seek
		public GameObject attractor;

		public float seekWeight = 0.2f; // weight of the seek behaviour

		public override SteeringOutput GetSteering ()
		{
			
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = FlockingAround.GetSteering (this.ownKS, attractor, seekWeight, idTag, cohesionThreshold, repulsionThreshold, wanderRate,
				                                                vmWeight, rpWeight, coWeight, wdWeight);
			base.applyRotationalPolicy (rotationalPolicy, result, attractor);
			return result;

		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject attractor, float seekWeight=0.2f, string idTag="BOID", 
			float cohesionThreshold = 40f, float repulsionThreshold = 10f,
			float wanderRate = 10f, 
			float vmWeight = 0.08f, float rpWeight = 0.46f,  float coWeight = 0.23f, float wdWeight = 023f) {

			SteeringOutput seekOutput = Seek.GetSteering (ownKS, attractor); 
			SteeringOutput result = Flocking.GetSteering (ownKS, idTag, cohesionThreshold, repulsionThreshold, wanderRate); 

			// beware, Flocking may return NULL_STEERING. In that case, just apply seek
			if (result == NULL_STEERING) {
				return  seekOutput;
			}

			result.linearAcceleration = result.linearAcceleration * (1 - seekWeight) + seekOutput.linearAcceleration * seekWeight;
			result.angularAcceleration = result.angularAcceleration * (1 - seekWeight) + seekOutput.angularAcceleration * seekWeight;

			return result;
		}


	}
}
