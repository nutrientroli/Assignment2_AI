using UnityEngine;

namespace Steerings
{
	public class WanderPlusAvoid : SteeringBehaviour
	{

		// parameters required by Wander
		public float wanderRate = 30f;
		public float wanderRadius = 10f;
		public float wanderOffset = 20f;
		private float targetOrientation = 0f;

		// parameters required by obstacle avoidance...
		public bool showWhisker = true;
		public float lookAheadLength = 10f;
		public float avoidDistance = 10f;
		public float secondaryWhiskerAngle = 30f;
		public float secondaryWhiskerRatio = 0.7f;

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		private bool avoidActive = false;  // true if avoidance has been active last frame


		public override SteeringOutput GetSteering () {

			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = WanderPlusAvoid.GetSteering (this.ownKS, wanderRate, wanderRate, wanderOffset, ref targetOrientation, 
				showWhisker, lookAheadLength, avoidDistance, secondaryWhiskerAngle, secondaryWhiskerRatio, ref avoidActive);
			base.applyRotationalPolicy (rotationalPolicy, result, null);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS,
			float WanderRate, float wanderRadius, float wanderOffset, ref float targetOrientation,
			bool showWhishker, float lookAheadLength, float avoidDistance, float secondaryWhiskerAngle, float secondaryWhiskerRatio,
			ref bool avoidActive) {

			// give priority to obstacle avoidance
			SteeringOutput so = ObstacleAvoidance.GetSteering(ownKS, showWhishker, lookAheadLength, 
				                                               avoidDistance, secondaryWhiskerAngle, secondaryWhiskerRatio);

			if (so == NULL_STEERING) {
				if (avoidActive) {
					// if avoidance was active last frame, update target orientation (otherwise the object would tend to regain
					// the orientation it had before avoiding a collision which would make it face the obstacle again)
					targetOrientation = ownKS.orientation;
				}
				avoidActive = false;
				return Wander.GetSteering (ownKS, ref targetOrientation, WanderRate, wanderRadius, wanderOffset);
			} else {
				avoidActive = true;
				return so;
			}

		}
	}
}
