/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class FlockingAroundPlusAvoid : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		// parameters required by FlockingAround

		public GameObject attractor;
		public string idTag = "BOID";
		public float cohesionThreshold = 40f;
		public float repulsionThreshold = 10f;
		public float wanderRate = 10f;
		public float seekWeight = 0.2f; 

		public float vmWeight = 0.08f;
		public float rpWeight = 0.46f;
		public float coWeight = 0.23f;
		public float wdWeight = 0.23f;

		// parameters required by obstacle avoidance...
		public bool showWhisker = true;
		public float lookAheadLength = 10f;
		public float avoidDistance = 10f;
		public float secondaryWhiskerAngle = 30f;
		public float secondaryWhiskerRatio = 0.7f;


		public override SteeringOutput GetSteering () {

			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = FlockingAroundPlusAvoid.GetSteering (ownKS, attractor, seekWeight, idTag, cohesionThreshold, repulsionThreshold, wanderRate,
				vmWeight, rpWeight, coWeight, wdWeight,
				showWhisker, lookAheadLength, avoidDistance, secondaryWhiskerAngle, secondaryWhiskerRatio);
			base.applyRotationalPolicy (rotationalPolicy, result, attractor);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS,
			GameObject attractor, float seekWeight,
			string idTag="BOID", float cohesionsThreshold=40f, float repulsionThreshold=10f, float wanderRate=10f,
			float vmWeight = 0.08f, float rpWeight = 0.46f,  float coWeight = 0.23f, float wdWeight = 023f,
			bool showWhishker=true, float lookAheadLength=10f, float avoidDistance=10f, float secondaryWhiskerAngle=30f, float secondaryWhiskerRatio=0.7f) {

			// give priority to obstacle avoidance
			SteeringOutput so = ObstacleAvoidance.GetSteering(ownKS, showWhishker, lookAheadLength, 
				avoidDistance, secondaryWhiskerAngle, secondaryWhiskerRatio);

			if (so == NULL_STEERING) {
				return FlockingAround.GetSteering (ownKS, attractor, seekWeight, idTag, cohesionsThreshold, repulsionThreshold, wanderRate);
			}

			return so;

		}


	}
}
