/* **************** VERSION 2 ****************** */

using UnityEngine;
using System.Collections;

namespace Steerings
{

	public class Arrive : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		public float closeEnoughRadius = 5f;  // also know as targetRadius
		public float slowDownRadius = 20f;    // if inside this radius, slow down
		public float timeToDesiredSpeed = 0.1f; 

		public GameObject target;

		public override  SteeringOutput GetSteering () {

			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = Arrive.GetSteering (this.ownKS, this.target, this.closeEnoughRadius, 
				                                        this.slowDownRadius, this.timeToDesiredSpeed);
			base.applyRotationalPolicy (rotationalPolicy, result, target);
			return result;
		} 

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject target, 
			                                      float targetRadius = 5f, float slowDownRadius = 20f, 
			                                      float timeToDesiredSpeed = 0.1f ) {


			SteeringOutput steering = new SteeringOutput ();
			Vector3 directionToTarget;
			float distanceToTarget;
			float desiredSpeed;
			Vector3 desiredVelocity;
			Vector3 requiredAcceleration;

			// compute direction and distance to target
			directionToTarget = target.transform.position - ownKS.position;
			distanceToTarget = directionToTarget.magnitude;

			// if we're already there, no steering required
			if (distanceToTarget < targetRadius) {
				return NULL_STEERING;
			}

			// if we're are far away from target, let's go with full acceleration (SEEK)
			if (distanceToTarget > slowDownRadius)
				return Seek.GetSteering(ownKS, target);


			// if we're getting closer speed has to be inversely proportional to distance
			desiredSpeed = ownKS.maxSpeed * (distanceToTarget / slowDownRadius);

			// desired velocity is towards the target
			desiredVelocity = directionToTarget.normalized * desiredSpeed;

			// compute the acceleration required to get desiredVelocity in timeToDesiredSpeed
			// take into account that we already have a velocity
			requiredAcceleration = (desiredVelocity - ownKS.linearVelocity) / timeToDesiredSpeed;

			// if required acceleration is too high, clip it
			if (requiredAcceleration.magnitude > ownKS.maxAcceleration) {
				requiredAcceleration = requiredAcceleration.normalized * ownKS.maxAcceleration;
			}

			steering.linearAcceleration = requiredAcceleration;

			return steering;
		}
	
	
	}
}