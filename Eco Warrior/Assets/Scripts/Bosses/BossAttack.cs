using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float attackDamage = 3f;
    public float knockbackForce = 10f;

    [Header("Sound Settings")]
    [Tooltip("Sound effect for the attack swing.")]
    public AudioClip swingSound;
    [Tooltip("Sound effect for hitting the player.")]
    public AudioClip hitSound;

    private float lastAttackTime;
    private Animator animator;
    private Transform player;
    private BossMovement movement; // Reference to BossMovement
    private bool isPerformingSpecialAttack = false; // Flag to track if a special attack is active
    private AudioSource audioSource; // Dynamically added AudioSource

    public void Initialize(Animator bossAnimator, Transform playerTransform)
    {
        animator = bossAnimator;
        player = playerTransform;
        movement = GetComponent<BossMovement>();

        // Dynamically add an AudioSource if it doesn't exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // Ensure it doesn't play on awake
        }
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

            // Play the swing sound effect
            PlaySound(swingSound);
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
                //Debug.Log("Player hit!");
                PlayerHealthBarParent playerHealth = hit.GetComponentInChildren<PlayerHealthBarParent>();
                if (playerHealth != null)
                {
                    playerHealth.HitDamage(attackDamage, hit.gameObject);

                    // Play the hit sound effect
                    PlaySound(hitSound);
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

            // Adjust knockback distance to avoid walls
            float adjustedKnockbackForce = GetAdjustedKnockbackDistance(playerRb.position, knockbackDirection);

            StartCoroutine(InterpolateKnockback(playerRb, knockbackDirection, adjustedKnockbackForce));
        }
    }

    private float GetAdjustedKnockbackDistance(Vector2 startPosition, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, knockbackForce, LayerMask.GetMask("Walls"));
        if (hit.collider != null)
        {
            // Reduce knockback distance to stop before the obstacle
            return hit.distance;
        }
        return knockbackForce; // No obstacle, use full knockback force
    }

    private IEnumerator InterpolateKnockback(Rigidbody2D playerRb, Vector2 direction, float distance)
    {
        float knockbackDuration = 0.2f;
        float elapsedTime = 0f;
        Vector2 startPosition = playerRb.position;
        Vector2 targetPosition = startPosition + direction * distance;

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

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}


