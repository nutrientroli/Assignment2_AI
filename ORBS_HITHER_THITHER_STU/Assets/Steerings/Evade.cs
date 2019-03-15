/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{

	public class Evade : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;
		public float maxPredictionTime = 3f;
		public GameObject target;

		public override SteeringOutput GetSteering ()
		{
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = Evade.GetSteering (this.ownKS, this.target, this.maxPredictionTime);
			base.applyRotationalPolicy (rotationalPolicy, result, target);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject target, float maxPredictionTime=3f) {
			// we need to know the kinematic state of the target since we need to know its linear velocity

			// if target has no kinematic state "give up" and just seek
			KinematicState targetKS = target.GetComponent<KinematicState> ();
			if (targetKS == null) {
				Debug.Log("Evade invoked with a target that has no kinematic state attached. Resorting to Flee");
				return Flee.GetSteering (ownKS, target);
			}


			Vector3 directionToMe = ownKS.position - targetKS.position;
			float distanceToMe = directionToMe.magnitude;
			float currentSpeed = targetKS.linearVelocity.magnitude;

			// determine the time it will take the target to reach me
			float predictedTimeToMe = distanceToMe / currentSpeed;
			if (predictedTimeToMe > maxPredictionTime) {
				predictedTimeToMe = maxPredictionTime;
			}

			// now determine future (at predicted time) location of target
			Vector3 futurePositionOfTarget = targetKS.position + targetKS.linearVelocity*predictedTimeToMe;


			// is the target going to get me? Does it seem to be moving towards me?
			if ((futurePositionOfTarget - ownKS.position).magnitude < 1) {
				// impossible to flee your own position. Go somewhere else
				futurePositionOfTarget = Utils.OrientationToVector (Utils.VectorToOrientation (futurePositionOfTarget) + 1);
				//return Flee.GetSteering(ownKS, target);
			}


			// create surrogate target and place it at future location
			SURROGATE_TARGET.transform.position = futurePositionOfTarget;

			// delegate to flee
			return Flee.GetSteering(ownKS, SURROGATE_TARGET);

		}
	}
}

