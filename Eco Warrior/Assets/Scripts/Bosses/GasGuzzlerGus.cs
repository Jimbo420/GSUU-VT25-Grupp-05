using UnityEngine;

public class GasGuzzlerGus : MonoBehaviour
{
    // Boss Movement and Attack Variables
    public Transform player;          // Reference to the player
    public float moveSpeed = 3f;      // Gus's movement speed
    public float attackRange = 1.5f;  // Range within which Gus attacks
    public float attackCooldown = 2f; // Cooldown time between attacks

    // Health System Variables
    private Animator animator;        // Reference to Animator
    private float lastAttackTime;     // Tracks the time of the last attack
    public HealthbarBehavior healthBar; // Reference to the health bar

    [SerializeField] private float maxHealth = 100f; // Gus's maximum health
    private float currentHealth;      // Gus's current health

    void Start()
    {
        // Initialize components and health
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.Health(currentHealth, maxHealth); // Set up the health bar
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            ChasePlayer();
        }
        else if (Time.time > lastAttackTime + attackCooldown)
        {
            AttackPlayer();
        }

        // Update animator parameters for direction
        Vector2 direction = (player.position - transform.position).normalized;
        animator.SetFloat("IdleX", Mathf.Round(direction.x));
        animator.SetFloat("IdleY", Mathf.Round(direction.y));
    }

    void ChasePlayer()
    {
        // Calculate direction toward the player
        Vector2 direction = (player.position - transform.position).normalized;

        // Move Gus toward the player
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Update animator parameters for movement animations
        animator.SetFloat("IdleX", Mathf.Round(direction.x));
        animator.SetFloat("IdleY", Mathf.Round(direction.y));
        animator.SetFloat("Speed", moveSpeed); // Indicate Gus is walking
    }

    void AttackPlayer()
    {
        // Stop movement and trigger attack animation
        animator.SetFloat("Speed", 0); // Set speed to 0 while attacking
        animator.SetTrigger("Attack"); // Trigger attacking state
        lastAttackTime = Time.time; // Record the time of the attack

        // Update IdleX and IdleY immediately after starting the attack
        Vector2 direction = (player.position - transform.position).normalized;
        animator.SetFloat("IdleX", Mathf.Round(direction.x));
        animator.SetFloat("IdleY", Mathf.Round(direction.y));

        Debug.Log("GasGuzzlerGus is attacking the player!");
    }

    public void HitDamage(float damage)
    {
        // Apply damage to Gus
        currentHealth -= damage;
        healthBar.Health(currentHealth, maxHealth); // Update health bar UI

        Debug.Log($"GasGuzzlerGus took {damage} damage! Current Health: {currentHealth}");

        // Check if Gus should die
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("GasGuzzlerGus has been defeated!");
        Destroy(gameObject); // Remove Gus from the scene
    }
}
