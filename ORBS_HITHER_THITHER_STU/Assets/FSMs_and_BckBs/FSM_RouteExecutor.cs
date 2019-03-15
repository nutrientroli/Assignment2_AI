using UnityEngine;
using Steerings;
using Pathfinding;

namespace FSM
{
    [RequireComponent(typeof(PathFollowing))]
    [RequireComponent(typeof(Seeker))]
    // route executor is a variation of pathfeeder 

    public class FSM_RouteExecutor : FiniteStateMachine
    {
        public enum State {INITIAL, GENERATING, FOLLOWING, TERMINATED};
        public State currentState = State.INITIAL;

      
        public GameObject target; // the place to go
        public float targetReachedRadius = 4; // default value. Should be set from the outside every time the behaviour is (Re)Entered
        public float wayPointReachedRadius = 2; // default value
        public float repathTime = 0; // if 0, no repath...

        private Seeker seeker; // the path generator
        private Path path; // the path calculated by the seeker
        private PathFollowing pathFollowing; // the steering behaviour

        private float elapsedTime;
                
        void Start()
        {
            seeker = GetComponent<Seeker>();
            pathFollowing = GetComponent<PathFollowing>();
            pathFollowing.enabled = false;
        }

        public override void Exit()
        {
            //Modificación de SESA... QUE FEM?
            if(pathFollowing != null) pathFollowing.enabled = false;
            target = null;
            path = null;
            base.Exit();
        }

        public override void ReEnter()
        {
            currentState = State.INITIAL;
            base.ReEnter();
        }

        void Update()
        {
            switch (currentState)
            {
                case State.INITIAL:
                    ChangeState(State.GENERATING);
                    break;

                case State.GENERATING:
                    if (path != null)
                    { // just wait until the path has been generated
                        ChangeState(State.FOLLOWING);
                        break;
                    }
                    // in this state just wait until the path has been calculated
                    break;

                case State.FOLLOWING:
                    if (repathTime!=0 && elapsedTime>=repathTime)
                    {
                        Debug.Log("recalculating path after: " + elapsedTime);
                        ChangeState(State.GENERATING);
                        break;
                    }
                    if (SensingUtils.DistanceToTarget(gameObject, target)<=targetReachedRadius) {
                        ChangeState(State.TERMINATED);
                        break;
                    }
                    /*
                    if (repathTime == 0 &&  pathFollowing.currentWaypointIndex == path.vectorPath.Count)
                    { // end of path reached
                        ChangeState(State.TERMINATED);
                        break;
                    }
                    */
                    elapsedTime += Time.deltaTime;
                    break;

                case State.TERMINATED:
                    break;

            } // end of switch
        }



        private void ChangeState(State newState)
        {

            // EXIT STATE LOGIC. Depends on current state
            switch (currentState)
            {
                case State.GENERATING:
                    // do nothing in particular when leaving this state
                    elapsedTime = 0;
                    break;

                case State.FOLLOWING:
                    // the path has been completed. stop moving
                    pathFollowing.enabled = false;
                    break;

                case State.TERMINATED: break;

            } // end exit switch

            // ENTER STATE LOGIC. Depends on newState
            switch (newState)
            {

                case State.GENERATING:
                    // just ask the seeker for fresh path
                    path = null;
                    seeker.StartPath(gameObject.transform.position, target.transform.position, OnPathComplete);
                    break;

                case State.FOLLOWING:
                    // a new path has been calculated. Feed that path to the pathfollowinf steering	
                    pathFollowing.path = path;
                    pathFollowing.currentWaypointIndex = 0;
                    pathFollowing.wayPointReachedRadius = wayPointReachedRadius;
                    pathFollowing.enabled = true;

                    elapsedTime = 0;
                    break;

                case State.TERMINATED: break;

            } // end of enter switch


            currentState = newState;

        } // end of method ChangeState


        // calback method for seeker.StartPath
        private void OnPathComplete(Path p)
        {
            path = p;
        }


        
    }
}