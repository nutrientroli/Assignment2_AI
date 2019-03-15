/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class Face : SteeringBehaviour
	{

		public float closeEnoughAngle = 2f;
		public float slowDownAngle = 10f;
		public float timeToDesiredAngularSpeed = 0.1f;
		public GameObject target;

		public override SteeringOutput GetSteering () {

			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			// being a rotational behaviour, face applies no rotational policy...
			return Face.GetSteering (this.ownKS, this.target, this.closeEnoughAngle, 
				                     this.slowDownAngle, this.timeToDesiredAngularSpeed);
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject target,
			float targetAngularRadius = 2f,
			float slowDownAngularRadius = 10f,
			float timeToDesiredAngularSpeed = 0.1f) {

			Vector3 directionToTarget = target.transform.position - ownKS.position;

			SURROGATE_TARGET.transform.rotation = Quaternion.Euler (0, 0, Utils.VectorToOrientation(directionToTarget));

			// Align with surrogate target
			return Align.GetSteering (ownKS, SURROGATE_TARGET, targetAngularRadius, 
				                      slowDownAngularRadius, timeToDesiredAngularSpeed);
		}
			

	}
}