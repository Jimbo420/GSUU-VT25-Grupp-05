using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1f;
    public float gasTankMoveSpeed = 6f;

    public Transform target;
    private Animator animator;

    private float originalMoveSpeed; // To store the default move speed

    [Header("Footstep Settings")]
    [SerializeField] public AudioSource footstepAudioSource; // AudioSource for footstep sounds
    [SerializeField] private AudioClip[] footstepClips; // Array of footstep sound effects
    [SerializeField] private float footstepInterval = 0.5f; // Time between footsteps

    private float footstepTimer = 0f; // Timer to track footstep intervals

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

        // Dynamically adjust the footstep interval based on movement speed
        float dynamicFootstepInterval = Mathf.Clamp(1f / moveSpeed, 0.2f, 1f); // Adjust range as needed

        // Play footstep sounds at regular intervals
        if (moveSpeed > 0) // Only play footsteps if Gus is moving
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstepSound();
                footstepTimer = dynamicFootstepInterval; // Reset the timer with the dynamic interval
            }
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

    private void PlayFootstepSound()
    {
        if (footstepAudioSource != null && footstepClips.Length > 0)
        {
            // Select a random footstep sound
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepAudioSource.PlayOneShot(clip);
        }
    }
}

