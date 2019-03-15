/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class Align : SteeringBehaviour
	{

		public float closeEnoughAngle = 2f;
		public float slowDownAngle = 10f;
		public float timeToDesiredAngularSpeed = 0.1f;

		public GameObject target;

		public override SteeringOutput GetSteering () {

			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			// being a rotational behaviour, align applies no rotational policy
			return Align.GetSteering (this.ownKS, this.target, this.closeEnoughAngle, this.slowDownAngle, this.timeToDesiredAngularSpeed);
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject target,
		                                          float targetAngularRadius = 2f,
			                                      float slowDownAngularRadius = 10f,
		                                          float timeToDesiredAngularSpeed = 0.1f) {

			SteeringOutput result = new SteeringOutput();
			result.linearActive = false; // this is not a linear steering
			result.angularActive = true; // this is a rotational steering


			float requiredAngularSpeed;
			float targetOrientation = target.transform.eulerAngles.z; // BEWARE...

			float requiredRotation = targetOrientation - ownKS.orientation; // how many degs do we have to rotate?

			if (requiredRotation < 0)
				requiredRotation = 360 + requiredRotation; // map to positive angles

			if (requiredRotation > 180)
				requiredRotation = -(360 - requiredRotation); // don't rotate more than 180 degs. just reverse rotation sense

			// when here, required rotation is in [-180, +180]

			float rotationSize = Mathf.Abs (requiredRotation); 

			if (rotationSize <= targetAngularRadius) // if we're "there", no steering needed
				return NULL_STEERING;
			

			if (rotationSize > slowDownAngularRadius)
				requiredAngularSpeed = ownKS.maxAngularSpeed;
			else
				requiredAngularSpeed = ownKS.maxAngularSpeed * (rotationSize/slowDownAngularRadius);

			// restablish sign
			requiredAngularSpeed = requiredAngularSpeed * Mathf.Sign (requiredRotation);

			// compute acceleration
			result.angularAcceleration = (requiredAngularSpeed - ownKS.angularSpeed)/timeToDesiredAngularSpeed;
			// clip if necessary
			if (Mathf.Abs (result.angularAcceleration) > ownKS.maxAngularAcceleration)
				result.angularAcceleration = ownKS.maxAngularAcceleration * Mathf.Sign (result.angularAcceleration);

			return result;
		}
	
	}
}