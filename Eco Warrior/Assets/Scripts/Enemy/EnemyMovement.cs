using System.Collections.Generic;
//using System.Linq;
using System.Threading;
//using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.InputSystem;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    //private Rigidbody2D rb;
    private Animator _animator;

    private HealthbarBehavior healthbarBehavior;
    private ToolRotator _toolRotator;
    //[SerializeField] PolygonCollider2D polygonCollider;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] public float health = 0;
    [SerializeField] public float maxHealth = 25;

    //[SerializeField] private Vector2 patrolCordinates = new Vector2(5f, 7f);
    [SerializeField] private bool isPlayerInRange = false;
    [SerializeField] private float waitTimeMin = 1f;
    [SerializeField] private float waitTimeMax = 3f;

    [SerializeField] public Transform[] WayPoints;
    public NavMeshAgent agent;

    private int wayPointIndex = -1;

    private Vector2 startPosition;
    private Vector2 currentTarget;
    private int newWayPoint;

    private TargetPlayer targetPlayer;
    public Transform collisionObsticle;

    private float idleTimer = 0f;
    private bool isIdle = false;

    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        //healthbarBehavior = GetComponentInChildren<HealthbarBehavior>();
        targetPlayer = GetComponent<TargetPlayer>();
        //polygonCollider = GetComponentInChildren<PolygonCollider2D>();
        _toolRotator = GetComponentInChildren<ToolRotator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        collisionObsticle = GameObject.FindGameObjectWithTag("CollisionObsticle").transform;
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
        if (targetPlayer.PlayerIsInRangeOfEnemy() == false)
            agent.SetDestination(WayPoints[newWayPoint].position);
        else
            agent.SetDestination(currentTarget);
        EnemyWalkAnimation();
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
