/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class FlockingPlusAvoid : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		// parameters required by flocking
		public string idTag = "BOID";
		public float cohesionThreshold = 40f;
		public float repulsionThreshold = 10f;
		public float wanderRate = 10f;

		// parameters required by obstacle avoidance...
		public bool showWhisker = true;
		public float lookAheadLength = 10f;
		public float avoidDistance = 10f;
		public float secondaryWhiskerAngle = 30f;
		public float secondaryWhiskerRatio = 0.7f;


		public override SteeringOutput GetSteering () {

			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = FlockingPlusAvoid.GetSteering (ownKS, idTag, cohesionThreshold, repulsionThreshold, wanderRate, 
				showWhisker, lookAheadLength, avoidDistance, secondaryWhiskerAngle, secondaryWhiskerRatio);

			base.applyRotationalPolicy (rotationalPolicy, result, null);

			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS,
			string idTag, float cohesionsThreshold, float repulsionThreshold, float wanderRate,
			bool showWhishker, float lookAheadLength, float avoidDistance, float secondaryWhiskerAngle, float secondaryWhiskerRatio) {

			// give priority to obstacle avoidance
			SteeringOutput so = ObstacleAvoidance.GetSteering(ownKS, showWhishker, lookAheadLength, 
				avoidDistance, secondaryWhiskerAngle, secondaryWhiskerRatio);

			if (so == NULL_STEERING ) {
				return Flocking.GetSteering (ownKS, idTag, cohesionsThreshold, repulsionThreshold, wanderRate);
			}

			return so;

		}
	}
}
