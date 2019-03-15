/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class WanderAroundPlusAvoid : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		// parameters required by WanderAround
		public float wanderRate = 30f;
		public float wanderRadius = 10f;
		public float wanderOffset = 20f;
		protected float targetOrientation = 0f; // must be kept between calls.
		public GameObject attractor;
		public float seekWeight = 0.2f; 

		// parameters required by obstacle avoidance...
		public bool showWhisker = true;
		public float lookAheadLength = 10f;
		public float avoidDistance = 10f;
		public float secondaryWhiskerAngle = 30f;
		public float secondaryWhiskerRatio = 0.7f;

		private bool avoidActive = false;

		public override SteeringOutput GetSteering ()
		{
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = WanderAroundPlusAvoid.GetSteering (ownKS, attractor, seekWeight, wanderRate, wanderRadius, wanderOffset, 
				                                                       ref targetOrientation, showWhisker, lookAheadLength, avoidDistance, 
																		secondaryWhiskerAngle, secondaryWhiskerRatio, ref avoidActive);
			base.applyRotationalPolicy (rotationalPolicy, result, attractor);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject attractor, float seekWeight,
			float wanderRate, float wanderRadius, float wanderOffset, ref float targetOrientation,
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
				return WanderAround.GetSteering (ownKS, attractor, seekWeight, ref targetOrientation, wanderRate, wanderRadius, wanderOffset);
			} else {
				avoidActive = true;
				return so;
			}

		}

        //-------------------------

        public void SetSeekWeight (float sk)
        {
            seekWeight = sk;
        }

	}
}
