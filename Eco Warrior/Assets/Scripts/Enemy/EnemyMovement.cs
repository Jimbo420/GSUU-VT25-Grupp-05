using System.Collections.Generic;
//using System.Linq;
using System.Threading;
//using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.InputSystem;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] public float health = 0;
    [SerializeField] public float maxHealth = 25;
    [SerializeField] private bool isPlayerInRange = false;
    [SerializeField] public Transform[] WayPoints;

    private float waitTimeMin = 1f;
    private float waitTimeMax = 3f;
    private int newWayPoint;
    private float idleTimer = 0f;
    private bool isIdle = false;

    public NavMeshAgent agent;
    private Animator _animator;
    private ToolRotator _toolRotator;
    private Vector2 currentTarget;
    private TargetPlayer targetPlayer;
    public Transform collisionObsticle;
    public Transform enemy;


    void Start()
    {
        _animator = GetComponent<Animator>();
        targetPlayer = GetComponent<TargetPlayer>();
        _toolRotator = GetComponent<ToolRotator>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        EnemyStartup();
    }

    private void EnemyStartup()
    {
        health = maxHealth;
        NewPosition();
    }
    void Update()
    {
        targetPlayer.lastFacingDirection = agent.velocity.normalized;
        if (targetPlayer.PlayerIsInRangeOfEnemy())
        {
            isPlayerInRange = true;
            targetPlayer.EngageTarget();
        }
        else
        {
            isPlayerInRange = false;
            Guard();
        }
    }
    public void SetTarget(Vector2 newTarget)
    {
        currentTarget = newTarget;
    }

    public void Walk()
    {
        if (targetPlayer.PlayerIsInRangeOfEnemy() == false)
        {
            agent.autoBraking = true;
            agent.SetDestination(WayPoints[newWayPoint].position);
            agent.speed = 1.5f;
        }
        else
        {
            agent.autoBraking = false;
            agent.SetDestination(currentTarget);
            agent.speed = 3.5f;
        }
        EnemyWalkAnimation();
    }

    public void HearSound(Vector2 sourcePosition)
    {
        // Start moving towards the sound
        agent.SetDestination(sourcePosition);
        Walk();
    }

    private void EnemyWalkAnimation()
    {
        Vector2 direction = agent.velocity.normalized;
        _animator.SetFloat("InputX", direction.x);
        _animator.SetFloat("InputY", direction.y);
        _animator.SetBool("isWalking", direction.magnitude > 0.1f);
        _toolRotator.RotateTool(false, direction);

        if (!isPlayerInRange && agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            isIdle = true;
            idleTimer = Random.Range(waitTimeMin, waitTimeMax);
        }
    }

    private void NewPosition()
    {
        var tempWaypoints = Random.Range(0, WayPoints.Length);
        if (newWayPoint != tempWaypoints)
            newWayPoint = tempWaypoints;
    }

    private void Guard()
    {
        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isIdle = false;
                NewPosition();
            }
            _animator.SetBool("isWalking", false);
            return;
        }
        Walk();
    }

    public void Dead()
    {
        Destroy(gameObject);
    }
}
