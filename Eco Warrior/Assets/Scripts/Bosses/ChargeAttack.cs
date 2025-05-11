using UnityEngine;

public class ChargeAttack : BossSpecialAttack
{
    private enum ChargeState { Idle, Charging, Paused }
    private ChargeState currentState = ChargeState.Idle;

    [Header("Charge Attack Settings")]
    public float chargeDistance = 10f;
    public float chargeSpeed = 20f;
    public int maxChargesAfterGasCan = 3;
    public float pauseBetweenCharges = 1f;

    [Header("Fire Trail Settings")]
    public GameObject fireTrailPrefab;
    public float fireTrailLifetime = 5f;
    public float fireTrailMinScale = 2f;
    public float fireTrailMaxScale = 3f;
    public bool fireTrailRandomRotation = true;
    public float fireTrailSpawnInterval = 0.1f;

    [Header("Wall Detection Settings")]
    public LayerMask wallLayer;

    [Header("Animation Settings")]
    public string movementAnimationParameter = "Speed";

    [Header("Audio Settings")]
    public AudioClip chargeSound;
    private AudioSource audioSource;
    private Transform player;
    private Animator animator;
    private BossAttack bossAttack;

    private Vector2 chargeDirection;
    private float distanceCharged = 0f;
    private float fireTrailTimer = 0f;
    private float pauseTimer = 0f;
    private int remainingCharges = 0;

    public bool IsCharging => currentState == ChargeState.Charging; // Expose the charging state

    private void Awake()
    {
        // Find the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("[ChargeAttack] Player object not found. Ensure the player has the 'Player' tag.");
        }

        // Get required components
        animator = GetComponent<Animator>();
        bossAttack = GetComponent<BossAttack>();

        // Ensure AudioSource exists
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Validate fireTrailPrefab
        if (fireTrailPrefab == null)
        {
            Debug.LogWarning("[ChargeAttack] FireTrailPrefab is not assigned. Fire trails will not be spawned.");
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case ChargeState.Idle:
                // Wait for the charge to be triggered
                break;

            case ChargeState.Charging:
                HandleCharging();
                break;

            case ChargeState.Paused:
                HandlePause();
                break;
        }
    }

    public void EnableCharges()
    {
        remainingCharges = maxChargesAfterGasCan;
    }

    public bool HasChargesAvailable()
    {
        return remainingCharges > 0 && currentState == ChargeState.Idle;
    }

    public void PerformChargeIfAvailable()
    {
        if (HasChargesAvailable())
        {
            PerformAttack();
        }
    }

    protected override void PerformAttack()
    {
        if (currentState != ChargeState.Idle || remainingCharges <= 0)
        {
            return;
        }

        PlayChargeSound();

        if (player != null)
        {
            chargeDirection = (player.position - transform.position).normalized;
        }
        else
        {
            Debug.LogWarning("[ChargeAttack] Player reference is null. Charge direction cannot be determined.");
            return;
        }

        if (animator != null)
        {
            animator.SetFloat("IdleX", Mathf.Round(chargeDirection.x));
            animator.SetFloat("IdleY", Mathf.Round(chargeDirection.y));
            animator.SetFloat(movementAnimationParameter, chargeSpeed);
        }

        StartCharge();
    }

    private void StartCharge()
    {
        currentState = ChargeState.Charging;
        distanceCharged = 0f;
        fireTrailTimer = 0f;

        // Disable normal attacks during the charge
        bossAttack.SetSpecialAttackActive(true);

        Debug.Log($"[ChargeAttack] Starting charge. Direction: {chargeDirection}");
    }

    private void HandleCharging()
    {
        // Check for wall collision
        if (IsWallInDirection(chargeDirection))
        {
            Debug.Log("[ChargeAttack] Charge stopped due to wall collision.");
            EndCharge();
            return;
        }

        // Move the boss
        float moveDistance = chargeSpeed * Time.deltaTime;
        Vector3 movement = moveDistance * (Vector3)chargeDirection;
        transform.position += movement;
        distanceCharged += moveDistance;

        // Spawn fire trail at intervals
        fireTrailTimer += Time.deltaTime;
        if (fireTrailPrefab != null && fireTrailTimer >= fireTrailSpawnInterval)
        {
            SpawnFireTrail();
            fireTrailTimer = 0f;
        }

        // Check if charge distance is reached
        if (distanceCharged >= chargeDistance)
        {
            EndCharge();
        }
    }

    private void HandlePause()
    {
        pauseTimer += Time.deltaTime;
        if (pauseTimer >= pauseBetweenCharges)
        {
            pauseTimer = 0f;
            currentState = ChargeState.Idle;
        }
    }

    private void EndCharge()
    {
        currentState = ChargeState.Paused;
        remainingCharges--;

        // Re-enable normal attacks after the charge
        bossAttack.SetSpecialAttackActive(false);
    }

    private void PlayChargeSound()
    {
        if (chargeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(chargeSound);
        }
    }

    private void SpawnFireTrail()
    {
        if (fireTrailPrefab == null)
        {
            Debug.LogWarning("[ChargeAttack] FireTrailPrefab is not assigned. Cannot spawn fire trail.");
            return;
        }

        // Directly instantiate the fire trail prefab
        GameObject fireTrail = Instantiate(fireTrailPrefab, transform.position, Quaternion.identity);

        // Configure the fire trail
        float randomScale = Random.Range(fireTrailMinScale, fireTrailMaxScale);
        fireTrail.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        if (fireTrailRandomRotation)
        {
            float randomRotation = Random.Range(-30f, 30f);
            fireTrail.transform.Rotate(0f, 0f, randomRotation);
        }

        // Destroy the fire trail after its lifetime
        Destroy(fireTrail, fireTrailLifetime);
    }

    private bool IsWallInDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, chargeSpeed * Time.deltaTime, wallLayer);
        return hit.collider != null;
    }
}
