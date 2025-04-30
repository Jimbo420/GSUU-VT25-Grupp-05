using System.Collections.Generic;
//using System.Linq;
using System.Threading;
//using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.InputSystem;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] public float health = 0;
    [SerializeField] public float maxHealth = 25;
    [SerializeField] private bool isPlayerInRange = false;
    [SerializeField] private float waitTimeMin = 1f;
    [SerializeField] private float waitTimeMax = 3f;
    [SerializeField] public Transform[] WayPoints;

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
        collisionObsticle = GameObject.FindGameObjectWithTag("CollisionObsticle").transform;
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
        if(agent is null) Debug.Log("Target Player is null");
        if (targetPlayer.PlayerIsInRangeOfEnemy() == false)
        {
            agent.SetDestination(WayPoints[newWayPoint].position);
        }
        else
        {
            agent.SetDestination(currentTarget);
        }
        EnemyWalkAnimation();
    }

    private void EnemyWalkAnimation()
    {
        Vector2 direction = agent.velocity.normalized;
        _animator.SetFloat("InputX", direction.x);
        _animator.SetFloat("InputY", direction.y);
        _animator.SetBool("isWalking", direction.magnitude > 0.1f);
        //_toolRotator.RotateTool(false, direction);

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

    //public void HitDamage(float hitDamage)
    //{
    //    health -= hitDamage;
    //    healthbarBehavior.Health(health, maxHealth);
    //    if (health <= 0)
    //        Dead();
    //}

    public void Heal()
    {
        for(float healValue = health; healValue < maxHealth; healValue++)
        {
            health = healValue;
            //healthbarBehavior.Health(healValue, maxHealth);
        }
    }
    public void Dead()
    {
        Destroy(gameObject);
    }
}
