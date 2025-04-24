using System.Collections;
using UnityEngine;

public class GasGuzzlerGus : MonoBehaviour
{
    // Boss Movement and Attack Variables
    public Transform player; // Reference to the player
    public float moveSpeed = 3f; // Gus's movement speed
    public float attackRange = 2f; // Radius of Gus's attack area
    public float attackCooldown = 2f; // Cooldown time between attacks
    [SerializeField] private float attackDamage = 3f; // Damage dealt to the player during attack
    [SerializeField] private float knockbackForce = 5f; // Knockback force applied to the player

    // Health System Variables
    public HealthbarBehavior healthBar; // Reference to the health bar
    private Animator animator; // Reference to Animator
    private float lastAttackTime; // Tracks the time of the last attack

    [SerializeField] private float maxHealth = 100f; // Gus's maximum health
    private float currentHealth; // Gus's current health

    void Start()
    {
        // Initialize components and health
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.Health(currentHealth, maxHealth); // Initialize the health bar
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            // Chase the player if outside attack range
            ChasePlayer();
        }
        else
        {
            // Attack the player if cooldown allows
            if (Time.time > lastAttackTime + attackCooldown)
            {
                StartAttack();
            }
        }

        // Update IdleX and IdleY regularly to ensure correct idle animation
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

    void StartAttack()
    {
        animator.SetFloat("Speed", 0); // Stop movement
        animator.SetTrigger("Attack"); // Trigger attack animation
        lastAttackTime = Time.time;
    }

    // This method is triggered by an animation event at the correct moment in the attack animation
    public void PerformAttack()
    {
        // Use Physics2D.OverlapCircleAll to detect objects in the attack range
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player")) // Ensure it's the player
            {
                // Apply damage
                HealthbarBehavior playerHealth = hit.GetComponentInChildren<HealthbarBehavior>();
                if (playerHealth != null)
                {
                    playerHealth.HitDamage(attackDamage, hit.gameObject);
                    Debug.Log($"Gus attacked the player and dealt {attackDamage} damage!");
                }

                // Apply knockback
                ApplyKnockback(hit);
            }
        }
    }

    void ApplyKnockback(Collider2D playerCollider)
    {
        Rigidbody2D playerRb = playerCollider.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            // Calculate knockback direction
            Vector2 knockbackDirection = (playerCollider.transform.position - transform.position).normalized;

            // Ensure the direction is not zero (e.g., if the player and Gus are at the same position)
            if (knockbackDirection == Vector2.zero)
            {
                knockbackDirection = Vector2.up; // Default to upward knockback if direction is zero
            }

            // Start the interpolation coroutine for smooth knockback
            StartCoroutine(InterpolateKnockback(playerRb, knockbackDirection));
        }
        else
        {
            Debug.LogWarning("Player does not have a Rigidbody2D component. Knockback cannot be applied.");
        }
    }

    IEnumerator InterpolateKnockback(Rigidbody2D playerRb, Vector2 direction)
    {
        float knockbackDuration = 0.2f; // Duration of the knockback effect
        float elapsedTime = 0f;

        // Calculate the target position based on the knockback force
        Vector2 startPosition = playerRb.position;
        Vector2 targetPosition = startPosition + direction * knockbackForce;

        // Disable the player's velocity to prevent interference
        playerRb.linearVelocity = Vector2.zero;

        // Smoothly interpolate the player's position
        while (elapsedTime < knockbackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / knockbackDuration;

            // Interpolate position using Lerp
            playerRb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));

            yield return null; // Wait for the next frame
        }

        // Ensure the player ends up at the exact target position
        playerRb.MovePosition(targetPosition);

        Debug.Log("Knockback interpolation complete!");
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

    // Visualize the attack range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
