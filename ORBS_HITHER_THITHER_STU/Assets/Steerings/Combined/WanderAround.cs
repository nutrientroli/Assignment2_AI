/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class WanderAround : SteeringBehaviour
	{
	
		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		// parameters for wander
		public float wanderRate = 30f;
		public float wanderRadius = 10f;
		public float wanderOffset = 20f;
		protected float targetOrientation = 0f; // value has to be kept between calls...

		// parameter for seek
		public GameObject attractor; // target for seek

		// the weight of the seek behaviour
		public float seekWeight = 0.2f; 


		public override SteeringOutput GetSteering ()
		{
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = WanderAround.GetSteering (ownKS, attractor, seekWeight, ref targetOrientation, wanderRate, wanderRadius, wanderOffset);
			base.applyRotationalPolicy (rotationalPolicy, result, attractor);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject attractor, float seekWeight,  
			ref float targetOrientation, float wanderRate = 30f, 
			float wanderRadius = 10f, float wanderOffset = 20f) {

			SteeringOutput seekOutput = Seek.GetSteering (ownKS, attractor);
			SteeringOutput result = Wander.GetSteering (ownKS, ref targetOrientation, wanderRate, wanderRadius, wanderOffset);

			result.linearAcceleration = result.linearAcceleration * (1 - seekWeight) + seekOutput.linearAcceleration * seekWeight;
			// result.angularAcceleration = result.angularAcceleration * (1 - seekWeight) + seekOutput.angularAcceleration * seekWeight;

			return result;
		}
			

        // ------------------------------

        public void SetSeekWeight (float sw)
        {
            seekWeight = sw;
        }
   
	}
}
