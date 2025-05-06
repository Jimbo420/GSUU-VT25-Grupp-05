using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1f; // Base movement speed
    public Transform target;    // Current movement target

    private Animator animator;
    private float originalMoveSpeed;
    private float temporarySpeedModifier = 1f; // Temporary speed multiplier

    [Header("Footstep Settings")]
    public AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepInterval = 0.5f;

    private float footstepTimer = 0f;

    public void Initialize(Transform player, Animator bossAnimator)
    {
        target = player;
        animator = bossAnimator;
        originalMoveSpeed = moveSpeed;
    }

    public void HandleMovement()
    {
        if (target == null)
            return;

        // Calculate the direction to the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Gradually move toward the target using the temporary speed modifier
        float adjustedSpeed = originalMoveSpeed * temporarySpeedModifier;
        transform.position = Vector3.MoveTowards(transform.position, target.position, adjustedSpeed * Time.deltaTime);

        // Update facing direction and animation
        UpdateFacingDirection(direction);
        UpdateAnimation(direction);

        // Handle footsteps
        PlayFootstepSound();
    }

    public void SetTemporaryMoveSpeed(float multiplier)
    {
        // Cap the multiplier to prevent extreme values
        temporarySpeedModifier = Mathf.Clamp(multiplier, 0.5f, 3f);
        //Debug.Log($"[BossMovement] Setting temporary speed modifier. Multiplier: {temporarySpeedModifier}, Adjusted Speed: {originalMoveSpeed * temporarySpeedModifier}");
    }

    public void ResetMoveSpeed()
    {
        //Debug.Log("[BossMovement] Resetting temporary speed modifier to 1.");
        temporarySpeedModifier = 1f;
    }

    public void SetTarget(Transform newTarget)
    {
        //Debug.Log($"[BossMovement] Setting target to: {(newTarget != null ? newTarget.position : null)}");
        target = newTarget;
    }

    private void UpdateFacingDirection(Vector3 direction)
    {
        if (animator == null)
            return;

        // Update facing direction (IdleX and IdleY)
        animator.SetFloat("IdleX", Mathf.Round(direction.x));
        animator.SetFloat("IdleY", Mathf.Round(direction.y));
    }

    private void UpdateAnimation(Vector3 direction)
    {
        if (animator == null)
            return;

        // Update animator parameters for movement
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
        animator.SetFloat("Speed", direction.magnitude);
    }

    private void PlayFootstepSound()
    {
        if (footstepAudioSource == null || footstepClips.Length == 0)
            return;

        footstepTimer += Time.deltaTime;
        if (footstepTimer >= footstepInterval)
        {
            footstepTimer = 0f;
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepAudioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Forces Gus to face a specific direction (e.g., toward the player).
    /// </summary>
    public void FaceDirection(Vector3 direction)
    {
        UpdateFacingDirection(direction);
    }
}

