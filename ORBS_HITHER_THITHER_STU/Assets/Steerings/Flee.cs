/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class Flee : SteeringBehaviour
	{
		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;
		public GameObject target;

		public override SteeringOutput GetSteering ()
		{
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = Flee.GetSteering (this.ownKS, this.target);
			base.applyRotationalPolicy(rotationalPolicy, result, this.target);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject target) {
			// invoke SEEK...
			SteeringOutput result = Seek.GetSteering (ownKS, target);
			// ... reverse direction and that's all
			result.linearAcceleration = -result.linearAcceleration;
			return result;
		}
	}
}

