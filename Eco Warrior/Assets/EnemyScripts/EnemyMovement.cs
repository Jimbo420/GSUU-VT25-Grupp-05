using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] HealthbarBehavior healthbarBehavior;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] public float health = 0;
    [SerializeField] public float maxHealth = 25;

    [SerializeField] private Vector2 patrolCordinates = new Vector2(5f, 7f);
    [SerializeField] private bool isPlayerInRange = false;
    [SerializeField] private float waitTimeMin = 1f;
    [SerializeField] private float waitTimeMax = 3f;

    private Vector2 startPosition;
    private Vector2 currentTarget;

    [SerializeField] public TargetPlayer targetPlayer;

    private float idleTimer = 0f;
    private bool isIdle = false;
    


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthbarBehavior = GetComponentInChildren<HealthbarBehavior>();
        targetPlayer = GetComponent<TargetPlayer>();
        EnemyStartup();
    }

    private void EnemyStartup()
    {
        health = maxHealth;
        healthbarBehavior.Health(health, maxHealth);
        startPosition = transform.position;
        NewPosition();
    }
    void Update()
    {
        if (targetPlayer.PlayerIsInRangeOfEnemy())
        {
            isPlayerInRange = true;
            animator.SetBool("isWalking", false);
            targetPlayer.EngageTarget();
            //Debug.Log("InRange: True");
        }
        else
        {
            isPlayerInRange = false;
            Guard();
            //Debug.Log("InRange: False");
        }
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

            animator.SetBool("isWalking", false);
            return;
        }
        Walk();
    }
    public void Walk()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);

        Vector2 direction = (currentTarget - (Vector2)transform.position).normalized;
        animator.SetFloat("InputX", direction.x);
        animator.SetFloat("InputY", direction.y);
        animator.SetBool("isWalking", true);

        if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
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
    public void HitDamage(float hitDamage)
    {
        health -= hitDamage;
        healthbarBehavior.Health(health, maxHealth);
        if (health <= 0)
            Dead();
    }

    public void Heal()
    {
        for(float healValue = health; healValue < maxHealth; healValue++)
        {
            health = healValue;
            healthbarBehavior.Health(healValue, maxHealth);
        }
    }
    public void Dead()
    {
        Destroy(gameObject);
    }
}
