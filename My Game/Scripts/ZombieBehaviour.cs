using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent),typeof(Animator))]
public class ZombieBehaviour : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    public enum State
    {
        Idle,
        Follow,
        Attack,
    }

    public State state;

    // The object the enemy wants to follow
    public Transform target;

    // How close should the enemy be before they follow?
    public float followRange = 10.0f;

    // How close should the enemy be before attacking?
    public float attackRange = 2.0f;

    // How far should the target be before the enemy gives up following it? 
    // Needs to be >= followRange
    public float idleRange = 10.0f;

    // Use this for initialization
    void Start ()
	{
	    agent = GetComponent<NavMeshAgent>();
	    animator = GetComponent<Animator>();

        GoToNextState();
    }

    IEnumerator IdleState()
    {
        //OnEnter
        Debug.Log("Idle: Enter");
        while (state == State.Idle)
        {
            agent.SetDestination(transform.position);
            animator.speed = 0;

            //OnUpdate
            if (GetDistance() < followRange)
            {
                animator.speed = 1;
                state = State.Follow;
            }

            yield return 0;
        }
        //OnEnd
        Debug.Log("Idle: Exit");
        GoToNextState();
    }

    IEnumerator FollowState()
    {
        Debug.Log("Follow: Enter");
        while (state == State.Follow)
        {
            var dist = GetDistance();
            agent.SetDestination(target.position);

            if (dist > idleRange)
            {
                state = State.Idle;
            }
            else if (dist < attackRange)
            {
                state = State.Attack;
            }

            yield return 0;
        }
        Debug.Log("Follow: Exit");
        GoToNextState();
    }

    IEnumerator AttackState()
    {
        Debug.Log("Attack: Enter");
        while (state == State.Attack)
        {
            var dist = GetDistance();
            agent.SetDestination(transform.position);

            animator.Play("attack");

            if (dist > attackRange)
            {
                animator.Play("walk");
                state = State.Follow;
            }

            yield return 0;
        }
        Debug.Log("Attack: Exit");
        GoToNextState();
    }

    void GoToNextState()
    {
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info = GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }

    public float GetDistance()
    {
        return (transform.position - target.transform.position).magnitude;
    }

    public float damageAmount = 1.0f;

    public void PhysicalAttack()
    {
        if (GetDistance() < attackRange)
        {
            target.SendMessage("Damage", damageAmount, SendMessageOptions.DontRequireReceiver);
        }
    }

}
