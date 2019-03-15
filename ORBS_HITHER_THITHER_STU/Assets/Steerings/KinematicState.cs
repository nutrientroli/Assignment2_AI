using UnityEngine;

namespace Steerings
{

	public class KinematicState : MonoBehaviour
	{
		

		// basic kinematic parameters
		public float maxAcceleration = 2f;
		public float maxSpeed = 10f;
		public float maxAngularAcceleration = 45f;
		public float maxAngularSpeed = 45f; // max rotation

		// the true components of the kinematic state
		public Vector3 position;
		public float orientation;
		public Vector3 linearVelocity = Vector3.zero;
		public float angularSpeed = 0; // sometimes known as rotation


		// Use this for initialization
		void Start ()
		{
			this.position = transform.position;
			this.orientation = transform.eulerAngles.z;
		}
	
		//----------------- SETTERS----

        public void SetMaxAcceleration (float ma)
        {
            maxAcceleration = ma;
        }

        public void SetMaxSpeed (float ms)
        {
            maxSpeed = ms;
        }

        public void SetMaxAngularAcceleration (float maa)
        {
            maxAngularAcceleration = maa;
        }

        public void SetMaxAngularSpeed (float mas)
        {
            maxAngularSpeed = mas;
        }
	}
}