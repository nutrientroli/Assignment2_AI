/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{

	public class VelocityMatching : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		public GameObject target;
		public float timeToDesiredVelocity=0.1f;
			

		public override SteeringOutput GetSteering ()
		{
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = VelocityMatching.GetSteering (this.ownKS, this.target, 
				                                                  this.timeToDesiredVelocity);
			base.applyRotationalPolicy (rotationalPolicy, result, target);
			return result;

		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject target, float 
			                                                            timeToDesiredVelocity=0.1f) {

			KinematicState targetKS = target.GetComponent<KinematicState> ();
			if (targetKS == null) {
				Debug.LogError ("Velocity matching requires a target with an accessible kinematic state ");
				return NULL_STEERING; // or produce an exception
			}

			SteeringOutput result = new SteeringOutput ();
			// compute required acceleration to have the same velocity
			result.linearAcceleration = (targetKS.linearVelocity - ownKS.linearVelocity) / timeToDesiredVelocity;

			// clip if necessary
			if (result.linearAcceleration.magnitude > ownKS.maxAcceleration)
				result.linearAcceleration = result.linearAcceleration.normalized * ownKS.maxAcceleration;

			return result;
		}
	}
}