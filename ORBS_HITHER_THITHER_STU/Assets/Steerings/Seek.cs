/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{

	public class Seek : SteeringBehaviour
	{

		public  GameObject target;
		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		public override SteeringOutput GetSteering () {
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			if (this.target == null)
				Debug.Log ("Null target in Seek of "+this.gameObject );


			SteeringOutput result = Seek.GetSteering (this.ownKS, this.target);
			base.applyRotationalPolicy(rotationalPolicy, result, this.target);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject target) {
			SteeringOutput steering = new SteeringOutput ();
			Vector3 directionToTarget;

			// Compute direction to target
			directionToTarget = target.transform.position - ownKS.position;
			directionToTarget.Normalize ();

			// give maxAcceleration towards the target
			steering.linearAcceleration = directionToTarget * ownKS.maxAcceleration;

			return steering;
		}
	}
}
