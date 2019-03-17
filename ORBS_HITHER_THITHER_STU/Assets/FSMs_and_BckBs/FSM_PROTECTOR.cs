using FSM;
using Steerings;
using UnityEngine;
using Pathfinding;


public class FSM_PROTECTOR : FiniteStateMachine 
{
    // References:
    PROTECTOR_BLACKBOARD blackboard;
    FSM_RouteExecutor routeExecutor;
    FSM_WANDER wander;

    // Variables
    enum State
    {
        INITIAL,
        WANDERING,
        SEEKING_ORBE,
        SEEKING_MARAUDER,
        TRANSPORTING_ORBE,
        KILLING_MARAUDER
    }
    [SerializeField] private State currentState;

    [SerializeField] private GameObject currentTarget;
    [SerializeField] private GameObject surrogateTarget;
    private GameObject orbe_obj;
    private GameObject marauder_obj;
    private KinematicState maraurer_ks;

    [Header("Pursue Behaviour")]
    [SerializeField] private float distanceAhead = 20f;      // distance in front of the target we will try to reach
    [SerializeField] private float repathTime = 0.2f;       // time in between path computing calls

    private void Awake()
    {
        blackboard = GetComponent<PROTECTOR_BLACKBOARD>();
        routeExecutor = GetComponent<FSM_RouteExecutor>();
        wander = GetComponent<FSM_WANDER>();

        wander.wanderPoints_Ar = GameObject.FindGameObjectsWithTag("NORMAL_WP");
        blackboard.baseWayPoints_Arr = GameObject.FindGameObjectsWithTag("SOUTH_DELIVERY");
        blackboard.killingWayPoints_Arr = GameObject.FindGameObjectsWithTag("MARAUDER_KILLING");
        surrogateTarget = GameObject.Find("surrogateTarget");

        routeExecutor.enabled = false;
        wander.enabled = false;

        currentState = State.INITIAL;
    }

    public override void ReEnter()
    {
        currentState = State.INITIAL;
        base.ReEnter();
    }

    public override void Exit()
    {
        routeExecutor.Exit();
        wander.Exit();
        currentTarget = null;
        base.Exit();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.INITIAL:
                ChangeState(State.WANDERING);
                break;
            case State.WANDERING:
                // IS A MARAUDER ON SIGHT (HIGHER PRIORITY THAN ORBS)
                marauder_obj = SensingUtils.FindInstanceWithinRadius(this.gameObject, blackboard.marauder_Tag, blackboard.marauderDetectionRadius);
                if (!ReferenceEquals(marauder_obj, null)) {
                    ChangeState(State.SEEKING_MARAUDER);
                    break;
                }
                // IS ORB ON SIGHT?
                orbe_obj = SensingUtils.FindInstanceWithinRadius(this.gameObject, blackboard.freeOrb_Tag, blackboard.orbDetectionRadius);
                if (!ReferenceEquals(orbe_obj, null)) {
                    ChangeState(State.SEEKING_ORBE);
                    break;
                }
                break;
            case State.SEEKING_ORBE:
                // Is there any marauder on sight? If so, go and get him!
                marauder_obj = SensingUtils.FindInstanceWithinRadius(this.gameObject, blackboard.marauder_Tag, blackboard.marauderDetectionRadius);
                if (!ReferenceEquals(marauder_obj, null)) {
                    ChangeState(State.SEEKING_MARAUDER);
                    break;
                }
                // No marauder on sight? Get to the current target if it is STILL being a free orbe
                // on the premnises of the orb
                if (orbe_obj.tag == blackboard.freeOrb_Tag && SensingUtils.DistanceToTarget(this.gameObject, currentTarget) <= blackboard.orbReachedRadius)
                {
                    blackboard.GrabOrb(orbe_obj, transform);
                    ChangeState(State.TRANSPORTING_ORBE);
                    break;
                }

                if (orbe_obj.tag == blackboard.claimedOrb_Tag)
                {
                    ChangeState(State.WANDERING);
                    break;
                }
                break;
            case State.SEEKING_MARAUDER:
                
                // Does the marauder still have the orbe with him? if not, ignore him
                if (marauder_obj == null) {
                    ChangeState(State.WANDERING);
                    break;
                }
                // update the position of the surrogated target in order to "pursue" the enemy on a more clever way (still not clever at all)
                Vector3 displacement_delta = maraurer_ks.linearVelocity * distanceAhead * Time.deltaTime;
                surrogateTarget.transform.position = maraurer_ks.transform.position + displacement_delta;
                Debug.DrawLine(this.transform.position, surrogateTarget.transform.position, Color.red);

                // if we are close enought to the marauder we take him 
                if (SensingUtils.DistanceToTarget(this.gameObject, marauder_obj) <= blackboard.marauderReachedRadius)
                {
                    marauder_obj.GetComponent<FSM_MARAUDER>().Exit();
                    blackboard.GrabMarauder(marauder_obj, transform);
                    ChangeState(State.KILLING_MARAUDER);
                    break;
                }

                if (marauder_obj.tag == blackboard.marauderCaught_Tag)
                {
                    ChangeState(State.WANDERING);
                    break;
                }
                break;
            case State.TRANSPORTING_ORBE:
                // Is there a marauder on sight? If so we relase the orb and we try to kill him
                marauder_obj = SensingUtils.FindInstanceWithinRadius(this.gameObject, blackboard.marauder_Tag, blackboard.marauderDetectionRadius);
                if (!ReferenceEquals(marauder_obj, null))
                {
                    blackboard.DropOrb(orbe_obj, false);        // normal drop
                    ChangeState(State.SEEKING_MARAUDER);
                    break;
                }
                // Did we arrive to the relase point?
                if (SensingUtils.DistanceToTarget(this.gameObject, currentTarget) <= blackboard.deliveryPointReachedRadius)
                {
                    blackboard.DropOrb(orbe_obj, true);          // base drop
                    ChangeState(State.WANDERING);
                    break;
                }
                break;
            case State.KILLING_MARAUDER:
                // Has de marauder reached the killing point?
                if (SensingUtils.DistanceToTarget(this.gameObject, currentTarget) <= blackboard.killingPointReachedRadius)
                {
                    if (marauder_obj != null)
                    {
                        marauder_obj.GetComponent<FSM_MARAUDER>().ReEnter();
                        marauder_obj.GetComponent<FSM_MARAUDER>().BeKilled();
                        blackboard.DropMarauder(marauder_obj);
                    }
                    ChangeState(State.WANDERING);
                }
                break;
        }
    }

    private void ChangeState(State _nextState)
    {
        switch (currentState)
        {
            case (State.INITIAL):
                break;
            case (State.WANDERING):
                wander.Exit();
                break;
            case (State.SEEKING_ORBE):
                currentTarget = null;
                routeExecutor.Exit();
                break;
            case (State.SEEKING_MARAUDER):
                maraurer_ks = null;
                currentTarget = null;
                routeExecutor.repathTime = 0f;
                routeExecutor.Exit();
                break;
            case (State.TRANSPORTING_ORBE):
                currentTarget = null;
                routeExecutor.Exit();
                break;
            case (State.KILLING_MARAUDER):
                currentTarget = null;
                routeExecutor.Exit();
                break;
        }

        switch (_nextState)
        {
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
            case (State.SEEKING_MARAUDER):
                maraurer_ks = marauder_obj.GetComponent<KinematicState>();
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
            case State.KILLING_MARAUDER:
                currentTarget = blackboard.GetRandomKillingPoint();
                routeExecutor.target = currentTarget;
                routeExecutor.ReEnter();
                break;
        }
        currentState = _nextState;
    }
}
