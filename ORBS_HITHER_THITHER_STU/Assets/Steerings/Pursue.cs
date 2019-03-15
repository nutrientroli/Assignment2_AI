/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class Pursue : SteeringBehaviour
	{
		// INTERCEPT

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		public float maxPredictionTime = 3f;
		public GameObject target;

		public override SteeringOutput GetSteering ()
		{
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = Pursue.GetSteering (this.ownKS, this.target, this.maxPredictionTime);
			base.applyRotationalPolicy (rotationalPolicy, result, target);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject target, float maxPredictionTime=3f) {
			// we need to know the kinematic state of the target since we need to know its linear velocity

			// if target has no kinematic state "give up" and just seek
			KinematicState targetKS = target.GetComponent<KinematicState> ();
			if (targetKS == null) {
				Debug.Log("Pursue invoked with a target that has no kinematic state attached. Resorting to Seek");
				return Seek.GetSteering (ownKS, target);
			}

			Vector3 directionToTarget = targetKS.position - ownKS.position;
			float distanceToTarget = directionToTarget.magnitude;
			float currentSpeed = ownKS.linearVelocity.magnitude;

			// determine the time it will take to reach the target
			float predictedTimeToTarget = distanceToTarget / currentSpeed;
			if (predictedTimeToTarget > maxPredictionTime) {
				predictedTimeToTarget = maxPredictionTime;
			}

			// now determine future (at predicted time) location of target
			Vector3 futurePositionOfTarget = targetKS.position + targetKS.linearVelocity*predictedTimeToTarget;

            DebugExtension.DebugPoint(futurePositionOfTarget, Color.red, 2f);

			// create surrogate target and place it at future location
			SURROGATE_TARGET.transform.position = futurePositionOfTarget;

			// delegate to seek
			return Seek.GetSteering(ownKS, SURROGATE_TARGET);
			// could also delegate to Arrive if overshooting is an issue...
		}

	}
}