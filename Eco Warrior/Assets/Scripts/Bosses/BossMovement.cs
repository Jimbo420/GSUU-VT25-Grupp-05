using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1f;
    public float gasTankMoveSpeed = 6f;

    public Transform target;
    private Animator animator;

    private float originalMoveSpeed; // To store the default move speed

    public void Initialize(Transform player, Animator bossAnimator)
    {
        target = player;
        animator = bossAnimator;
        originalMoveSpeed = moveSpeed; // Store the default move speed
    }

    public void HandleMovement()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Update animator
        if (animator != null)
        {
            animator.SetFloat("IdleX", Mathf.Round(direction.x));
            animator.SetFloat("IdleY", Mathf.Round(direction.y));
            animator.SetFloat("Speed", moveSpeed);
        }
    }

    // Temporarily override the movement speed (e.g., for gasoline tank movement)
    public void SetTemporaryMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    // Reset the movement speed to its original value
    public void ResetMoveSpeed()
    {
        moveSpeed = originalMoveSpeed;
    }
}

