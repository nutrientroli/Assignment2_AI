
using FSM;
using Steerings;
using UnityEngine;
using Pathfinding;

// By Daniel Moreno
public class FSM_MARAUDER : FiniteStateMachine
{
    // References:
    MARAUDER_BLACKBOARD blackboard;
    FSM_RouteExecutor routeExecutor;
    FSM_WANDER wander;

    // Variables
    enum State
    {
        INITIAL,
        WANDERING,
        SEEKING_ORBE,
        SEEKING_BEARER,
        TRANSPORTING_ORBE
    }
    [SerializeField] private State currentState;

    [SerializeField] private GameObject currentTarget;
    [SerializeField] private GameObject surrogateTarget;       // not in use at this stage
    private GameObject orbe_obj;
    private GameObject bearer_obj;
    private KinematicState bearer_ks;

    [Header("Pursue Behaviour")]
    [SerializeField] private float distanceAhead = 20f;      // distance in front of the target we will try to reach
    [SerializeField] private float repathTime = 0.2f;       // time in between path computing calls

    private void Awake() {
        blackboard = GetComponent<MARAUDER_BLACKBOARD>();
        routeExecutor = GetComponent<FSM_RouteExecutor>();
        wander = GetComponent<FSM_WANDER>();

        wander.wanderPoints_Ar = GameObject.FindGameObjectsWithTag("NORMAL_WP");
        blackboard.baseWayPoints_Arr = GameObject.FindGameObjectsWithTag("NORTH_DELIVERY");
        surrogateTarget = GameObject.Find("surrogateTarget");

        routeExecutor.enabled = false;
        wander.enabled = false;

        currentState = State.INITIAL;
    }

    public override void ReEnter() {
        currentState = State.INITIAL;
        base.ReEnter();
    }

    public override void Exit() {
        routeExecutor.Exit();
        wander.Exit();
        currentTarget = null;
        base.Exit();
    }

    private void Update() {
        switch (currentState) {
            case (State.INITIAL):
                ChangeState(State.WANDERING);
                break;
            case (State.WANDERING):
                // IS A BEARER ON SIGHT (HIGHER PRIORITY THAN ORBS)
                bearer_obj = SensingUtils.FindInstanceWithinRadius(this.gameObject, blackboard.bearerWithoutOrbe, blackboard.bearerDetectionRadius);

                //Debug.LogError(gameObject.name + " ::: ");
                if (!ReferenceEquals(bearer_obj, null)) {
                    ChangeState(State.SEEKING_BEARER);
                    break;
                }
                // IS ORB ON SIGHT?
                orbe_obj = SensingUtils.FindInstanceWithinRadius(this.gameObject, blackboard.freeOrb_Tag, blackboard.orbDetectionRadius);
                if (!ReferenceEquals(orbe_obj, null)) {
                    ChangeState(State.SEEKING_ORBE);
                    break;
                }
                break;
            case (State.SEEKING_ORBE):
                // Is there any bearer on sight? If so, go and get him!
                bearer_obj = SensingUtils.FindInstanceWithinRadius(this.gameObject, blackboard.bearerWithoutOrbe, blackboard.bearerDetectionRadius);
                if (!ReferenceEquals(bearer_obj, null)) {
                    ChangeState(State.SEEKING_BEARER);
                    break;
                }
                // No bearer on sight? Get to the current target if it is STILL being a free orbe
                // on the premnises of the orb
                if (orbe_obj.tag == blackboard.freeOrb_Tag && SensingUtils.DistanceToTarget(this.gameObject, currentTarget) <= blackboard.orbReachedRadius) {
                    blackboard.GrabOrb(orbe_obj, transform);
                    ChangeState(State.TRANSPORTING_ORBE);
                    break; 
                }

                if(orbe_obj.tag == blackboard.claimedOrb_Tag) {
                    ChangeState(State.WANDERING);
                    break;
                }
                break;
            case (State.SEEKING_BEARER):
                // EXPERIMENTAL --------------------------------------------------------------------------- //
                // update the position of the surrogated target in order to "pursue" the enemy on a more clever way (still not clever at all)
                Vector3 displacement_delta;
                displacement_delta = bearer_ks.linearVelocity * distanceAhead * Time.deltaTime;
                surrogateTarget.transform.position = bearer_obj.transform.position + displacement_delta;
                Debug.DrawLine(this.transform.position, surrogateTarget.transform.position, Color.red);
                // ----------------------------------------------------------------------------------------- //
                
                // Does the bearer still have the orbe with him? if not, ignore him
                if (bearer_obj == null || bearer_obj.tag == blackboard.bearerWithoutOrbe_Tag) {
                    ChangeState(State.WANDERING);
                    break;
                }
                // if we are close enought to the bearer we ... KILL HIM! 
                if (SensingUtils.DistanceToTarget(this.gameObject,bearer_obj) <= blackboard.bearerReachedRadius) {
                    bearer_obj.GetComponent<FSM_BEARER>().BeKilled();
                    ChangeState(State.WANDERING);
                    break;
                }
                break;
            case (State.TRANSPORTING_ORBE):
                // Is there a bearer on sight? If so we relase the orb and we try to kill him
                bearer_obj = SensingUtils.FindInstanceWithinRadius(this.gameObject, blackboard.bearerWithoutOrbe, blackboard.bearerDetectionRadius);
                if (!ReferenceEquals(bearer_obj, null)) {
                    blackboard.DropOrb(orbe_obj, false);        // normal drop
                    ChangeState(State.SEEKING_BEARER);
                    break;
                }
                // Did we arrive to the relase point?
                if (SensingUtils.DistanceToTarget(this.gameObject,currentTarget) <= blackboard.deliveryPointReachedRadius) {
                    blackboard.DropOrb(orbe_obj, true);          // base drop
                    ChangeState(State.WANDERING);
                    break;
                }
                break;
        }
    }

    private void ChangeState( State _nextState ) {
        switch (currentState) {
            case (State.INITIAL):
                break;
            case (State.WANDERING):
                wander.Exit();
                break;
            case (State.SEEKING_ORBE):
                currentTarget = null;
                routeExecutor.Exit();
                break;
            case (State.SEEKING_BEARER):
                bearer_ks = null;

                currentTarget = null;
                routeExecutor.repathTime = 0f;
                routeExecutor.Exit();
                break;
            case (State.TRANSPORTING_ORBE):
                currentTarget = null;
                routeExecutor.Exit();
                break;
        }

        switch (_nextState) {
            case (State.INITIAL):
                break;
            case (State.WANDERING):
                wander.ReEnter();
                break;
            case (State.SEEKING_ORBE):
                currentTarget = orbe_obj;
                routeExecutor.target = currentTarget;
                routeExecutor.ReEnter();
                break;
            case (State.SEEKING_BEARER):

                bearer_ks = bearer_obj.GetComponent<KinematicState>();

                currentTarget = surrogateTarget;
                routeExecutor.target = currentTarget;
                routeExecutor.repathTime = repathTime;
                routeExecutor.ReEnter();
                break;
            case State.TRANSPORTING_ORBE:
                currentTarget = blackboard.GetRandomBasePoint();
                routeExecutor.target = currentTarget;
                routeExecutor.ReEnter();
                break;
        }
        currentState = _nextState;
    }
}
