using UnityEngine;

public class GasGuzzlerGus : MonoBehaviour
{
    // Variables
    public Transform player;         // Reference to the player
    public float moveSpeed = 3f;     // Gus's movement speed
    public float attackRange = 1.5f; // Range within which Gus attacks
    public float attackCooldown = 2f; // Time between attacks

    private Animator animator;       // Animator reference
    private float lastAttackTime;    // Tracks when the last attack occurred

    void Start()
    {
        // Get Animator component
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Calculate the distance to the player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            // Chase the player
            ChasePlayer();
        }
        else
        {
            // Attack the player if cooldown allows
            if (Time.time > lastAttackTime + attackCooldown)
            {
                AttackPlayer();
            }
        }
    }

    void ChasePlayer()
    {
        // Calculate direction toward the player
        Vector2 direction = (player.position - transform.position).normalized;

        // Move Gus toward the player
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Set Animator parameters for walking animations
        animator.SetFloat("IdleX", direction.x);
        animator.SetFloat("IdleY", direction.y);
        animator.SetFloat("Speed", moveSpeed); // Indicate Gus is moving
    }

    void AttackPlayer()
    {
        // Stop movement and trigger attack animation
        animator.SetFloat("Speed", 0); // Stop moving during attack
        animator.SetTrigger("Attack"); // Activate attacking state

        lastAttackTime = Time.time; // Record the time of the attack

        // Placeholder for actual attack logic (e.g., damage player)
        Debug.Log("GasGuzzlerGus attacks!");
    }
}
