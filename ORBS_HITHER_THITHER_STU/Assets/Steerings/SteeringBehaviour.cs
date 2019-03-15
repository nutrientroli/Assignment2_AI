/* **************** VERSION 2 ****************** */

using UnityEngine;


namespace Steerings
{
	[RequireComponent(typeof(KinematicState))]

	public class SteeringBehaviour : MonoBehaviour 
	{

		public enum RotationalPolicy {LWYG, LWYGI, FT, FTI, NONE};
		// LWYG: look where you go
		// LWYGI: look where you go immediately
		// FT: face your target
		// FTI: face your target immediately
		// NONE: no rotational policy

		protected KinematicState ownKS;

		protected static GameObject SURROGATE_TARGET = null; // all behaviours requiring a surrogate target will use this one
		protected static SteeringOutput NULL_STEERING;

		// BEWARE: made vitual in order to allow redefinitions
		protected virtual void Start ()
		{
			// get a reference to the kinematic state and hold it
			ownKS = GetComponent<KinematicState>();

			if (SURROGATE_TARGET == null) {
				SURROGATE_TARGET = new GameObject ("surrogate target");
				SURROGATE_TARGET.AddComponent<KinematicState> ();
			}

			if (NULL_STEERING == null) {
				NULL_STEERING = new SteeringOutput ();
				NULL_STEERING.linearActive = false;
			}
				
		}
	

	
		void Update () {
			SteeringOutput steering = GetSteering ();

			// steering should not be null. But if it were, we'd better stop everything
			if (steering == null) {
				Debug.Log ("null steering returned");
				ownKS.linearVelocity = Vector3.zero; // stop!!!
				ownKS.angularSpeed = 0f; // stop!!!
				return;
			}

			float dt = Time.deltaTime;

			// apply linear steering, if there's a linear steering to apply...
			if (steering.linearActive) {
				// change linear velocity and position (in Kinematic state)
				ownKS.linearVelocity = ownKS.linearVelocity + steering.linearAcceleration * dt; // v=v+a·t
				if (ownKS.linearVelocity.magnitude > ownKS.maxSpeed)
					ownKS.linearVelocity = ownKS.linearVelocity.normalized * ownKS.maxSpeed; // clipping of velocity
				ownKS.position = ownKS.position + ownKS.linearVelocity * dt + 0.5f * steering.linearAcceleration * dt * dt; // s = v·t + 1/2(a·t^2)
				// apply to game object
				transform.position = ownKS.position;
			} else {
				ownKS.linearVelocity = Vector3.zero; // stop!!!
			}

			// apply angular steering, if there's an angular steering to apply...
			if (steering.angularActive) {
				//change angular velocity and orientation
				ownKS.angularSpeed = ownKS.angularSpeed + steering.angularAcceleration * dt;
				if (Mathf.Abs (ownKS.angularSpeed) > ownKS.maxAngularSpeed)
					ownKS.angularSpeed = ownKS.maxAngularSpeed * Mathf.Sign (ownKS.angularSpeed); // clip if necessary
				ownKS.orientation = ownKS.orientation + ownKS.angularSpeed * dt + 0.5f * steering.angularAcceleration * dt * dt;
				// apply to gameObject
				transform.rotation = Quaternion.Euler(0, 0, ownKS.orientation);

			} else {
				ownKS.angularSpeed = 0f; // stop!!!
			}

		}

		public virtual SteeringOutput GetSteering () {
			Debug.Log ("Invoking a non-redefined virtual method");
			return null;
		}


		/* Post processing of a SteeringOutput in order to apply the required rotational policy */

		public virtual void applyLWYGI (SteeringOutput steering) {
			if (ownKS.linearVelocity.magnitude > 0.001f) {
				// if linear velocity is very small, object is not moving (isn't going anywhere
				// hence it makes no sense to apply "look where you go")
				transform.rotation = Quaternion.Euler (0, 0,  Utils.VectorToOrientation (ownKS.linearVelocity));
				ownKS.orientation = transform.rotation.eulerAngles.z;
			}
			steering.angularActive = false;
		}

		public virtual void applyLWYG (SteeringOutput steering) {
			if (ownKS.linearVelocity.magnitude > 0.001f) {
				// if linear velocity is very small, object is not moving (isn't going anywhere
				// hence it makes no sense to apply "look where you go"

				// Align with velocity.
				// create a surrogate target the orientation of which is that of my linear velocity
				SURROGATE_TARGET.transform.rotation = Quaternion.Euler (0, 0, Utils.VectorToOrientation (ownKS.linearVelocity));
				SteeringOutput st = Align.GetSteering (ownKS, SURROGATE_TARGET);
				steering.angularAcceleration = st.angularAcceleration;
				steering.angularActive = st.angularActive;
			} else
				steering.angularActive = false;

		}

		public virtual void applyFTI (SteeringOutput steering, GameObject target) {
			if (ownKS.linearVelocity.magnitude > 0.001f) {
				transform.rotation = Quaternion.Euler (0, 0,  Utils.VectorToOrientation (target.transform.position-ownKS.position));
				ownKS.orientation = transform.rotation.eulerAngles.z;
			}
			steering.angularActive = false;
		}
			
		public virtual void applyFT (SteeringOutput steering, GameObject target) {
				SteeringOutput st = Face.GetSteering (ownKS, target);
				steering.angularAcceleration = st.angularAcceleration;
				steering.angularActive = st.angularActive;
		}

		public virtual void applyRotationalPolicy (RotationalPolicy rotationalPolicy, SteeringOutput steering, GameObject target = null) {
			switch (rotationalPolicy) {
			case RotationalPolicy.LWYGI:
				applyLWYGI (steering);
				break;
			case RotationalPolicy.LWYG:
				applyLWYG (steering);
				break;
			case RotationalPolicy.FTI:
				if (target == null)
					break;
				applyFTI (steering, target);
				break;
			case RotationalPolicy.FT:
				if (target == null)
					break;
				applyFT (steering, target);
				break;
			case RotationalPolicy.NONE:
				break;
			}
		}


	}
}
