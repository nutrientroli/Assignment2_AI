/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{

	public class KBDSteer02 : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		public override SteeringOutput GetSteering () {

			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = KBDSteer02.GetSteering (this.ownKS);
			base.applyRotationalPolicy (rotationalPolicy, result, null);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS) {

			SteeringOutput steering = new SteeringOutput ();

			Vector3 desiredDirection = Vector3.zero;

			if (Input.GetKey(KeyCode.LeftArrow))
			{
				desiredDirection += Vector3.left;
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				desiredDirection += Vector3.right;
			}
			if (Input.GetKey(KeyCode.UpArrow))
			{
				desiredDirection += Vector3.up;
			}
			if (Input.GetKey(KeyCode.DownArrow))
			{
				desiredDirection += Vector3.down;
			}


			if (desiredDirection.magnitude < 0.01f) {
				return NULL_STEERING;
			} else {
				steering.linearAcceleration = desiredDirection * ownKS.maxAcceleration;
			}

			return steering;

		}
	}

}