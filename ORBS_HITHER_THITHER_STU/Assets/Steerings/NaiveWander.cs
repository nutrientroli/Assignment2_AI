/* **************** VERSION 2 ****************** */

using UnityEngine;

namespace Steerings
{
	public class NaiveWander : SteeringBehaviour
	{

		public float wanderRate = 30f;
		public float targetAngularRadius = 2f;
		public float slowDownAngularRadius = 10f;
		public float timeToDesiredAngularSpeed = 0.1f;

		// having its own rotational component, this steering does not apply any rotational policy

		public override SteeringOutput GetSteering ()
		{ 
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = NaiveWander.GetSteering(this.ownKS, this.wanderRate, this.targetAngularRadius, this.slowDownAngularRadius, this.timeToDesiredAngularSpeed);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, float wanderRate=30f, float targetAngularRadius=2f,
			                                       float slowDownAngularRadius = 10f, float timeToDesiredAngularSpeed = 0.1f ) {
			// align with a surrogate target that has your new orientation and go there

			// slightly change the orientation
			float desiredOrientation = ownKS.orientation + wanderRate * Utils.binomial ();

			// give that orientation to the surrogate target
			SURROGATE_TARGET.transform.rotation = Quaternion.Euler(0, 0, desiredOrientation);

			// align with the surrogate target
			SteeringOutput al = Align.GetSteering(ownKS, SURROGATE_TARGET, targetAngularRadius, slowDownAngularRadius, timeToDesiredAngularSpeed);

			// go where you look (looked, actually)
			SteeringOutput gwyl = GoWhereYouLook.GetSteering(ownKS); // should never return null

			// combine, if possible
			if (al != null) {
				gwyl.angularActive = true;
				gwyl.angularAcceleration = al.angularAcceleration;
			}
				
			return gwyl;
		}
	
	}
}