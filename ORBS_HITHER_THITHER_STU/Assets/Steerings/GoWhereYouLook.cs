/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class GoWhereYouLook : SteeringBehaviour
	{

		public override SteeringOutput GetSteering ()
		{
			// no rotational policy applied here. It makes no sense.
			return GoWhereYouLook.GetSteering (this.ownKS);
		}

		public static SteeringOutput GetSteering (KinematicState ownKS) {
			// just "seek" your own direction

			Vector3 myDirection = Utils.OrientationToVector (ownKS.orientation);
			SURROGATE_TARGET.transform.position = ownKS.position + myDirection;

			return Seek.GetSteering (ownKS, SURROGATE_TARGET);
		}
			
	}
}