using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [SerializeField] private Vector2 patrolCordinates = new Vector2(5f, 7f);
    [SerializeField] private bool isPlayerInRange = false;
    [SerializeField] private float waitTimeMin = 1f;
    [SerializeField] private float waitTimeMax = 3f;

    private Vector2 startPosition;
    private Vector2 currentTarget;

    private TargetPlayer targetPlayer;

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
        EnemyStartup();
    }

    private void EnemyStartup()
    {
        health = maxHealth;
        startPosition = transform.position;
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
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);

        Vector2 direction = (currentTarget - (Vector2)transform.position).normalized;
        _animator.SetFloat("InputX", direction.x);
        _animator.SetFloat("InputY", direction.y);
        _animator.SetBool("isWalking", true);
        _toolRotator.RotateTool( false, direction);
        

        if (!isPlayerInRange && Vector2.Distance(transform.position, currentTarget) < 0.1f)
        {
            isIdle = true;
            idleTimer = Random.Range(waitTimeMin, waitTimeMax);
        }
    }

    private void NewPosition()
    {
        float x = Random.Range(-patrolCordinates.x / 2f, patrolCordinates.x / 2f);
        float y = Random.Range(-patrolCordinates.y / 2f, patrolCordinates.y / 2f);
        currentTarget = startPosition + new Vector2(x, y);
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
