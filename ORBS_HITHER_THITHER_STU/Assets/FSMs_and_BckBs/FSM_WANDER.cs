using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

// By Daniel Moreno
public class FSM_WANDER : FiniteStateMachine
{
    // references
    FSM_RouteExecutor nodeExecutor;


    public GameObject[] wanderPoints_Ar;


    [Header("Behaviour Controll")]
    private float targetReachedDistance = 2f;
    private enum State
    {
        INITIAL, WANDERING
    }
    [SerializeField] private State currentState;
    public GameObject currentTarget;

    private void Awake()
    {
        nodeExecutor = GetComponent<FSM_RouteExecutor>();
        nodeExecutor.enabled = false;
        nodeExecutor.targetReachedRadius = targetReachedDistance;

    }

    private GameObject GetRandomListReference(GameObject[] _inputArray)
    {
        GameObject _object;
        int _randomIndex;

        _randomIndex = Random.Range(0, _inputArray.Length);
        Debug.LogWarning(_randomIndex + "[" + 0 + " , " + _inputArray.Length + "]");

        _object = _inputArray[_randomIndex];

        return _object;
    }


    public override void ReEnter()
    {
        currentState = State.INITIAL;
        base.ReEnter();
    }

    public override void Exit()
    {
        nodeExecutor.Exit();
        currentTarget = null;
        base.Exit();
    }

    private void Update()
    {
        switch (currentState)
        {
            case (State.INITIAL):
                ChangeState(State.WANDERING);
                break;

            case (State.WANDERING):

                // waypoint reached=?
                if (SensingUtils.DistanceToTarget(this.gameObject,currentTarget) <= targetReachedDistance)
                {
                    ChangeState(State.WANDERING);
                    break;
                }


                break;
        }
    }

    private void ChangeState( State _newState )
    {
        switch(currentState)
        {
            case (State.INITIAL):
                //
                break;

            case (State.WANDERING):

                currentTarget = null;
                nodeExecutor.Exit();
                nodeExecutor.target = null;

                break;
        }


        switch (_newState)
        {
            case (State.INITIAL):
                //
                break;

            case (State.WANDERING):

                currentTarget = GetRandomListReference(wanderPoints_Ar);

                nodeExecutor.target = currentTarget;
                nodeExecutor.ReEnter();

                break;
        }

        currentState = _newState;
    }
}
