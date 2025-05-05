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
    private BossMovement movement; // Reference to BossMovement
    private bool isPerformingSpecialAttack = false; // Flag to track if a special attack is active

    public void Initialize(Animator bossAnimator, Transform playerTransform)
    {
        animator = bossAnimator;
        player = playerTransform;
        movement = GetComponent<BossMovement>();
    }

    public void HandleAttack()
    {
        // If a special attack is active, skip regular attacks
        if (isPerformingSpecialAttack)
            return;

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
            // Ensure Gus faces the player before attacking
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            movement.FaceDirection(directionToPlayer);

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
                Debug.Log("Player hit!");
                HealthbarBehavior playerHealth = hit.GetComponentInChildren<HealthbarBehavior>();
                if (playerHealth != null)
                {
                    playerHealth.HitDamage(attackDamage, hit.gameObject);
                }

                ApplyKnockback(hit);
            }
        }
    }

    private void ApplyKnockback(Collider2D playerCollider)
    {
        if (playerCollider.TryGetComponent<Rigidbody2D>(out var playerRb))
        {
            Vector2 knockbackDirection = (playerCollider.transform.position - transform.position).normalized;
            if (knockbackDirection == Vector2.zero)
            {
                knockbackDirection = Vector2.up; // Default knockback direction if positions overlap
            }

            StartCoroutine(InterpolateKnockback(playerRb, knockbackDirection));
        }
    }

    private IEnumerator InterpolateKnockback(Rigidbody2D playerRb, Vector2 direction)
    {
        float knockbackDuration = 0.2f;
        float elapsedTime = 0f;
        Vector2 startPosition = playerRb.position;
        Vector2 targetPosition = startPosition + direction * knockbackForce;

        playerRb.linearVelocity = Vector2.zero;

        while (elapsedTime < knockbackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / knockbackDuration;
            playerRb.MovePosition(Vector2.Lerp(startPosition, targetPosition, t));
            yield return null;
        }

        playerRb.MovePosition(targetPosition);
    }

    public void SetSpecialAttackActive(bool isActive)
    {
        isPerformingSpecialAttack = isActive;
    }
}
