/* **************** VERSION 2 ****************** */

using UnityEngine;
using System;

namespace Steerings
{
	public class Flocking : SteeringBehaviour
	{
		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;
		// FT and FTI make no sense for this steering.

		public string idTag = "BOID";
		public float cohesionThreshold = 40f;
		public float repulsionThreshold = 10f;
		public float wanderRate = 10f;

		public float vmWeight = 0.08f;
		public float rpWeight = 0.46f;
		public float coWeight = 0.23f;
		public float wdWeight = 0.23f;

		/* other weight configurations:
		 - 0.1 0.3 0.2 0.4
		 - 0.1 0.25 0.25 0.4
		 - 0.2 0.4 0.3 0.1

		Generally a low vm is enough. 
		Generally rp must be higher than co and the rest, otherwise boids touch each other

		*/

		public override SteeringOutput GetSteering ()
		{
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = Flocking.GetSteering (this.ownKS, idTag, cohesionThreshold, repulsionThreshold, wanderRate, vmWeight, rpWeight, coWeight, wdWeight);
			base.applyRotationalPolicy (rotationalPolicy, result, null);
			return result;
		}


		public static SteeringOutput GetSteering (KinematicState ownKS, string idTag="BOID", 
			                                      float cohesionThreshold = 40f, float repulsionThreshold = 10f,
												  float wanderRate = 10f, 
												  float vmWeight = 0.08f, float rpWeight = 0.46f,  float coWeight = 0.23f, float wdWeight = 0.23f) {
		
			float distanceToBoid;
			KinematicState boidKS;
			Vector3 averageVelocity = Vector3.zero;
			int count = 0;
			SteeringOutput result = new SteeringOutput ();

			// get all the other boids
			GameObject [] boids = GameObject.FindGameObjectsWithTag(idTag);


			// ... and iterate to find average velocity
			foreach (GameObject boid in boids) {
				// skip yourself
				if (boid==ownKS.gameObject) continue;

				boidKS = boid.GetComponent<KinematicState> ();
				if (boidKS == null) {
					// this should never happen but you never know
					Debug.Log("Incompatible mate in flocking. Flocking mates must have a kinematic state attached: "+boid);
					continue;
				}

				// disregard distant boids
				distanceToBoid = (boidKS.position - ownKS.position).magnitude;
				if (distanceToBoid > Math.Max(cohesionThreshold, repulsionThreshold))
					continue;

				averageVelocity = averageVelocity + boidKS.linearVelocity;
				count++;

			} // end of iteration to find average velocity

			if (count > 0)
				averageVelocity = averageVelocity / count;
			else {
				// if no boid is close enough (count==0) there's no flocking to be performed so return NULL_STEERING
				// or just apply some wandering... 
				return NaiveWander.GetSteering(ownKS, wanderRate);
			}
				

		
			SURROGATE_TARGET.GetComponent<KinematicState> ().linearVelocity = averageVelocity;

			SteeringOutput vm = VelocityMatching.GetSteering (ownKS, SURROGATE_TARGET); // (in normal conditions) this does NOT return NULL_STEERING 
			SteeringOutput rp = LinearRepulsion.GetSteering(ownKS, idTag, repulsionThreshold); // this MAY return NULL_STEERING
			SteeringOutput co = Cohesion.GetSteering(ownKS, idTag, cohesionThreshold); // this MAY return NULL_STEERING

				
			result.linearAcceleration = vm.linearAcceleration * vmWeight +
				rp.linearAcceleration * rpWeight + // if rp==NULL_STEERING linearAcceleration is Vector3.zero 
				co.linearAcceleration * coWeight;  // id for co. 

			// and now let's add some wandering to make things less predictable (yet more stable)

			SteeringOutput wd = NaiveWander.GetSteering(ownKS, wanderRate);
			result.linearAcceleration += wd.linearAcceleration * wdWeight;

			// adjust to maxAcceleration
			result.linearAcceleration = result.linearAcceleration.normalized * ownKS.maxAcceleration;

			return result;

		}


	}
}

