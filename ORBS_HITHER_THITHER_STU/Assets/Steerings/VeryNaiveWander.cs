/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class VeryNaiveWander : SteeringBehaviour
	{

		public float wanderRate=10f;
		// this method does not honour any rotational policy. It applies LWYGI always

		public override SteeringOutput GetSteering ()
		{
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = VeryNaiveWander.GetSteering (this.ownKS, this.wanderRate);
			base.applyLWYGI (result); // look where you go immediately
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, float wanderRate = 10f) {

			// "slightly" change your own orientation
			ownKS.orientation += Utils.binomial()*wanderRate;

			// and go where you look 
			return GoWhereYouLook.GetSteering (ownKS);
		}

	}
}
