using System.Collections.Generic;
//using System.Linq;
using System.Threading;
//using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.Rendering.UI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] public float health = 0;
    [SerializeField] public float maxHealth = 10;
    [SerializeField] private bool isPlayerInRange = false;
    [SerializeField] public Transform[] WayPoints;

    private float waitTimeMin = 1f;
    private float waitTimeMax = 3f;
    private int newWayPoint;
    private float idleTimer = 0f;
    private bool isIdle = false;
    private bool isAggressive = false;

    private float calmDelay = 2f;     // Tid att vänta innan calm
    private float calmTimer = 0f;     // Timer för att räkna ned

    public NavMeshAgent agent;
    private Animator _animator;
    private ToolRotator _toolRotator;
    private Vector2 currentTarget;
    private TargetPlayer targetPlayer;
    public Transform enemy;


    public bool isMakingSound;

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
            //GetComponentInParent<SoundEmitter>().Play(_musicSource, true);
            if (!isAggressive)
            {
                isAggressive = true;  // Nu vet vi att vi är i jaktläge
                MusicManager.Instance.PlayTensionMusic();
            }
        }
        else
        {
            isPlayerInRange = false;

            // Starta nertrappningstimer om vi är aggressiva
            if (isAggressive)
            {
                calmTimer += Time.deltaTime;
                if (calmTimer >= calmDelay)
                {
                    isAggressive = false;
                    calmTimer = 0f; // Nollställ timer
                    MusicManager.Instance.PlayCalmMusic();
                }
            }

            Guard();
        }
    }

    public void SetTarget(Vector2 newTarget)
    {
        currentTarget = newTarget;
    }

    public void Walk()
    {
        if (targetPlayer.PlayerIsInRangeOfEnemy() || isMakingSound)
        {
            if (isMakingSound && targetPlayer.player != null)
            {
                currentTarget = targetPlayer.player.position;
            }

            agent.autoBraking = false;

            if (agent.isActiveAndEnabled)
            {
                agent.SetDestination(currentTarget);
                agent.speed = 3.5f;
            }
        }
        else
        {
            agent.autoBraking = true;

            // Check if the waypoint exists before accessing it
            if (WayPoints != null && WayPoints.Length > newWayPoint && WayPoints[newWayPoint] != null)
            {
                if (agent.isActiveAndEnabled)
                {
                    agent.SetDestination(WayPoints[newWayPoint].position);
                    agent.speed = 1.5f;
                }
            }
        }

        EnemyWalkAnimation();
    }

    public void HearSound(Vector2 sourcePosition)
    {
        if (!targetPlayer.hasLineOfSight)
        {
            return;
        }

        isMakingSound = true;
        targetPlayer.EngageTarget();
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
}
