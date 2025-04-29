using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float attackDamage = 3f;
    public float knockbackForce = 10f;

    private float lastAttackTime;
    private Animator animator;
    private Transform player;

    public void Initialize(Animator bossAnimator, Transform playerTransform)
    {
        animator = bossAnimator;
        player = playerTransform;
    }

    public void HandleAttack()
    {
        if (Time.time > lastAttackTime + attackCooldown && IsPlayerInRange())
        {
            StartAttack();
        }
    }

    private bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer <= attackRange;
    }

    private void StartAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack"); // Trigger the attack animation
        }
        lastAttackTime = Time.time;
    }

    // This method should be called via an animation event
    public void PerformAttack()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                // Apply damage to the player
                Debug.Log("Player hit!");
                HealthbarBehavior playerHealth = hit.GetComponentInChildren<HealthbarBehavior>();
                if (playerHealth != null)
                {
                    playerHealth.HitDamage(attackDamage, hit.gameObject);
                }

                // Apply knockback to the player
                ApplyKnockback(hit);
            }
        }
    }

    private void ApplyKnockback(Collider2D playerCollider)
    {
        Rigidbody2D playerRb = playerCollider.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 knockbackDirection = (playerCollider.transform.position - transform.position).normalized;
            if (knockbackDirection == Vector2.zero)
            {
                knockbackDirection = Vector2.up; // Default knockback direction if positions overlap
            }

            Debug.Log($"Applying knockback to player. Direction: {knockbackDirection}, Force: {knockbackForce}");
            StartCoroutine(InterpolateKnockback(playerRb, knockbackDirection));
        }
        else
        {
            Debug.LogWarning("Player does not have a Rigidbody2D. Knockback not applied.");
        }
    }

    private IEnumerator InterpolateKnockback(Rigidbody2D playerRb, Vector2 direction)
    {
        float knockbackDuration = 0.2f; // Duration of the knockback effect
        float elapsedTime = 0f;
        Vector2 startPosition = playerRb.position;
        Vector2 targetPosition = startPosition + direction * knockbackForce;

        // Stop the player's velocity during knockback
        playerRb.linearVelocity = Vector2.zero;

        while (elapsedTime < knockbackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / knockbackDuration;
            playerRb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));
            yield return null;
        }

        // Ensure the player ends up at the target position
        playerRb.MovePosition(targetPosition);
    }
}
