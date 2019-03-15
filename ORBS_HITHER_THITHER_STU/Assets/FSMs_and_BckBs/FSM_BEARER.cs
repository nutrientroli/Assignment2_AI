using FSM;
using UnityEngine;

[RequireComponent(typeof(FSM_RouteExecutor))]
[RequireComponent(typeof(BEARER_BLACKBOARD))]
public class FSM_BEARER : FiniteStateMachine
{
    public enum State {INITIAL, GOING, RETURNING, DYING};
    public State currentState = State.INITIAL;

    private FSM_RouteExecutor gotoTarget;
    private BEARER_BLACKBOARD blackboard;
    private GameObject deliveryPoint;
    private GameObject exitPoint;

    private float elapsedTime;
    private bool dying; 

    void Start()
    {
        gotoTarget = GetComponent<FSM_RouteExecutor>();
        blackboard = GetComponent<BEARER_BLACKBOARD>();

        gotoTarget.enabled = false;
        gotoTarget.wayPointReachedRadius = 2;
      
        dying = false;
    }

    public override void Exit()
    {
        gotoTarget.Exit();
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
                ChangeState(State.GOING);
                break;
            case State.GOING:
                if (dying)
                {
                    ChangeState(State.DYING);
                    break;
                }
                if (SensingUtils.DistanceToTarget(gameObject, deliveryPoint)<=blackboard.pointReachedRadius)
                {
                    ChangeState(State.RETURNING);
                    break;
                }
                break;
            case State.RETURNING:
                if (dying)
                {
                    ChangeState(State.DYING);
                    break;
                }
                if (SensingUtils.DistanceToTarget(gameObject, exitPoint) <= blackboard.pointReachedRadius)
                {
                    ChangeState(State.DYING);
                    break;
                }
                break;
            case State.DYING:
                if (elapsedTime >= blackboard.vanishTime)
                {
                    if (blackboard.theOrb != null) blackboard.DropOrb(gameObject);
                    Destroy(gameObject);
                    break;
                }
                gameObject.transform.localScale /= 1.005f;
                elapsedTime = elapsedTime + Time.deltaTime;
                break;
        }
    }

    void ChangeState(State newState)
    {
        switch (currentState)
        {
            case State.GOING:
                // deliver "drop" the orb (if still carrying an orb, that is...
                if (blackboard.theOrb!=null)
                    blackboard.DropOrb(deliveryPoint);

                gameObject.tag = "BEARER_WITHOUT_ORB";
                gotoTarget.Exit();
                break;
            case State.RETURNING:
                gotoTarget.Exit();
                break;
            case State.DYING:
                break;
        }

        switch (newState)
        {
            case State.GOING:
                // when entering going a random delivery point must be selected
                deliveryPoint = blackboard.GetRandomDropPoint();
                gotoTarget.target = deliveryPoint;
                gotoTarget.targetReachedRadius = blackboard.pointReachedRadius;
                gotoTarget.repathTime = 0;
                gotoTarget.ReEnter();
                break;
            case State.RETURNING:
                // when entering returning a random exit point must be selected
                exitPoint = blackboard.GetRandomExitPoint();
                gotoTarget.target = exitPoint;
                gotoTarget.targetReachedRadius = blackboard.pointReachedRadius;
                gotoTarget.repathTime = 0;
                gotoTarget.ReEnter();
                break;
            case State.DYING:
                elapsedTime = 0f;
                break;
        }
    
        currentState = newState;
    }

    public void BeKilled()
    {
        if (blackboard.theOrb != null) blackboard.DropOrb(gameObject);
        this.dying = true;
    }
}
