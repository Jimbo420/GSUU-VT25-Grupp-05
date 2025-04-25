using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GasGuzzlerGus : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Reference to the player transform.")]
    public Transform player;

    [Header("Movement Settings")]
    [Tooltip("Movement speed when chasing the player.")]
    [SerializeField] private float moveSpeed = 1f;
    private float currentMoveSpeed;
    [Tooltip("Movement speed when moving towards a gasoline tank.")]
    [SerializeField] private float gasTankMoveSpeed = 6f;

    [Header("Attack Settings")]
    [Tooltip("Range within which the boss can attack the player.")]
    [SerializeField] private float attackRange = 2f;
    [Tooltip("Cooldown time between attacks.")]
    [SerializeField] private float attackCooldown = 2f;
    [Tooltip("Damage dealt by the boss's attack.")]
    [SerializeField] private float attackDamage = 3f;
    [Tooltip("Knockback force applied to the player when hit.")]
    [SerializeField] private float knockbackForce = 10f;

    [Header("Encounter Settings")]
    [Tooltip("Distance from the player to start the encounter.")]
    [SerializeField] private float encounterStartDistance = 11f;

    [Header("Special Attack Settings")]
    [Tooltip("Speed of the boss during special attacks.")]
    [SerializeField] private float specialAttackSpeed = 12f;
    [Tooltip("Distance the boss charges during a special attack.")]
    [SerializeField] private float chargeDistance = 10f;
    [Tooltip("Speed of the boss during the charge phase of a special attack.")]
    [SerializeField] private float chargeSpeed = 36f;
    [Tooltip("Interval between fire trail spawns during a special attack.")]
    [SerializeField] private float fireSpawnInterval = 0.025f;
    [Tooltip("Number of charge iterations during a special attack.")]
    [SerializeField] private int specialAttackIterations = 3;

    [Header("Boundary Settings")]
    [Tooltip("Radius within which Gus must stay.")]
    [SerializeField] private float boundaryRadius = 20f;
    [Tooltip("Starting location of Gus.")]
    [SerializeField] private Vector3 startingLocation;

    [Header("Prefabs and References")]
    [Tooltip("Prefab for the chat bubble.")]
    [SerializeField] private GameObject chatBubblePrefab;
    [Tooltip("Parent transform for the chat bubble.")]
    [SerializeField] private Transform chatBubbleParent;
    [Tooltip("Prefab for the gasoline tank.")]
    [SerializeField] private GameObject gasolineTankPrefab;
    [Tooltip("Spawn points for gasoline tanks.")]
    [SerializeField] private Transform[] spawnPoints;
    [Tooltip("Prefab for the fire trail.")]
    [SerializeField] private GameObject fireTrailPrefab;

    [Header("UI References")]
    [Tooltip("Reference to the health bar UI.")]
    [SerializeField] private HealthbarBehavior healthBar;

    [Space]
    [Header("Debug Settings")]
    [Tooltip("Enable or disable debug logs.")]
    [SerializeField] private bool enableDebugLogs = false;

    private bool hasStartedConversation = false;
    private bool[] phaseTriggered = new bool[3];
    private GameObject activeChatBubble;
    private bool isPlayerLocked = false;
    private bool isSpeechActive = false; // New flag for speech state
    private Animator animator;
    private float lastAttackTime;
    private bool isSpecialAttackActive = false;
    private bool isEncounterActive = false;

    // Stuck detection
    private Vector2 lastPosition;
    private float stuckTimer;
    [Tooltip("Time in seconds before Gus is considered stuck.")]
    [SerializeField] private float stuckThreshold = 3f; // Increased to reduce false positives
    [Tooltip("Minimum distance Gus must move to not be considered stuck.")]
    [SerializeField] private float stuckDistanceThreshold = 0.2f; // Increased to reduce false positives

    // Teleport cooldown
    private float teleportCooldown = 5f; // Cooldown in seconds
    private float lastTeleportTime = 0f;

    // Gasoline spawn interval
    private float currentSpawnInterval = 20f; // Default spawn interval

    void Start()
    {
        animator = GetComponent<Animator>();
        healthBar.Health(healthBar.slider.maxValue, healthBar.slider.maxValue); // Initialize health bar

        // Initialize stuck detection and starting location
        lastPosition = transform.position;
        stuckTimer = 0f;
        startingLocation = transform.position;

        // Initialize current move speed
        currentMoveSpeed = moveSpeed;
    }

    void Update()
    {
        if (enableDebugLogs)
            Debug.Log("Update is running...");

        if (isSpeechActive) // Prevent actions during speech
        {
            if (enableDebugLogs)
                Debug.Log("Update skipped: Speech is active.");
            animator.SetFloat("Speed", 0);
            return;
        }

        if (!isEncounterActive && Vector2.Distance(transform.position, player.position) < encounterStartDistance)
        {
            StartEncounter();
            return;
        }

        if (!isEncounterActive || isSpecialAttackActive || isPlayerLocked)
        {
            if (enableDebugLogs)
                Debug.Log($"Update skipped: EncounterActive={isEncounterActive}, SpecialAttackActive={isSpecialAttackActive}, PlayerLocked={isPlayerLocked}");
            animator.SetFloat("Speed", 0);
            return;
        }

        GameObject closestTank = FindClosestGasolineTank();
        if (closestTank != null)
        {
            MoveTowards(closestTank.transform.position, gasTankMoveSpeed);
            if (Vector2.Distance(transform.position, closestTank.transform.position) < 1f)
                ConsumeGasolineTank(closestTank);
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
            else if (Time.time > lastAttackTime + attackCooldown)
            {
                StartAttack();
            }
            else
            {
                // Fallback: If Gus is idle for too long, force him to move
                Debug.LogWarning("Gus is idle. Forcing movement toward the player.");
                ChasePlayer();
            }

            Vector2 direction = (player.position - transform.position).normalized;
            animator.SetFloat("IdleX", Mathf.Round(direction.x));
            animator.SetFloat("IdleY", Mathf.Round(direction.y));
        }

        CheckHealthPhases();
        CheckIfStuck();
        CheckBoundary();
    }


    private void CheckBoundary()
    {
        float distanceFromStart = Vector3.Distance(transform.position, startingLocation);

        if (distanceFromStart > boundaryRadius && Time.time > lastTeleportTime + teleportCooldown)
        {
            Debug.LogWarning($"Gus is outside the boundary! Distance: {distanceFromStart}, Radius: {boundaryRadius}. Teleporting back to starting location...");
            transform.position = startingLocation;

            // Reset movement to avoid getting stuck after teleporting
            stuckTimer = 0f;
            lastPosition = transform.position;

            // Update the last teleport time
            lastTeleportTime = Time.time;
        }
    }

    private void CheckIfStuck()
    {
        float distanceMoved = Vector2.Distance(transform.position, lastPosition);

        if (distanceMoved < stuckDistanceThreshold)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckThreshold)
            {
                Debug.LogWarning($"Gus is stuck! Distance moved: {distanceMoved}, Threshold: {stuckDistanceThreshold}. Resetting state...");
                ResetStuckState();
            }
        }
        else
        {
            stuckTimer = 0f; // Reset the timer if movement is detected
        }

        lastPosition = transform.position;
    }

    private void ResetStuckState()
    {
        Debug.LogWarning("Gus is stuck! Resetting state...");
        stuckTimer = 0f; // Reset the stuck timer
        lastPosition = transform.position; // Update the last position

        // Reset critical states
        isSpecialAttackActive = false;
        isPlayerLocked = false;

        // Force Gus to chase the player
        ChasePlayer();
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        Vector2 newPosition = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Debug log to verify movement
        if (Vector2.Distance(transform.position, newPosition) < 0.01f)
        {
            Debug.LogWarning("Gus is not moving. Check for obstacles or state conflicts.");
        }

        transform.position = newPosition;
        animator.SetFloat("IdleX", Mathf.Round(direction.x));
        animator.SetFloat("IdleY", Mathf.Round(direction.y));
        animator.SetFloat("Speed", speed);
    }

    private void StartEncounter()
    {
        isEncounterActive = true;
        StartConversation("So you're the one who's been meddling in my factory! You will pay dearly for this!");

        // Start spawning gasoline tanks with the default interval
        InvokeRepeating(nameof(SpawnGasolineTank), 10f, currentSpawnInterval);
    }

    void ChasePlayer()
    {
        MoveTowards(player.position, currentMoveSpeed);
    }

    void StartAttack()
    {
        animator.SetFloat("Speed", 0);
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
    }

    public void PerformAttack()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                HealthbarBehavior playerHealth = hit.GetComponentInChildren<HealthbarBehavior>();
                if (playerHealth != null)
                {
                    playerHealth.HitDamage(attackDamage, hit.gameObject);
                }
                ApplyKnockback(hit);
            }
        }
    }

    void ApplyKnockback(Collider2D playerCollider)
    {
        Rigidbody2D playerRb = playerCollider.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 knockbackDirection = (playerCollider.transform.position - transform.position).normalized;
            if (knockbackDirection == Vector2.zero)
            {
                knockbackDirection = Vector2.up;
            }
            StartCoroutine(InterpolateKnockback(playerRb, knockbackDirection));
        }
    }

    IEnumerator InterpolateKnockback(Rigidbody2D playerRb, Vector2 direction)
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

    public void HitDamage(float damage)
    {
        healthBar.HitDamage(damage, gameObject);
        if (healthBar.slider.value <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void CheckHealthPhases()
    {
        float currentHealth = healthBar.slider.value;
        float maxHealth = healthBar.slider.maxValue;

        if (currentHealth <= maxHealth * 0.75f && !phaseTriggered[0])
        {
            TriggerPhase(0, "Get out of my factory!");
            currentMoveSpeed *= 1.25f; // Increase speed for phase 1
            currentSpawnInterval = 15f; // Spawn gasoline more frequently
            RestartGasolineSpawnTimer();
            transform.localScale *= 1.1f;
        }
        else if (currentHealth <= maxHealth * 0.5f && !phaseTriggered[1])
        {
            TriggerPhase(1, "Now I'm getting mad! More gasoline!");
            currentMoveSpeed *= 1.25f; // Increase speed for phase 2
            attackDamage *= 2f; // Increase damage for phase 2
            currentSpawnInterval = 10f; // Spawn gasoline even more frequently
            RestartGasolineSpawnTimer();
            SpawnGasolineTank(); // Spawn a gasoline tank immediately
            transform.localScale *= 1.1f;
        }
        else if (currentHealth <= maxHealth * 0.25f && !phaseTriggered[2])
        {
            TriggerPhase(2, "ARGHHHHH! I WILL SUE YOU FLAT BROKE!");
            currentMoveSpeed *= 1.5f; // Increase speed for phase 3
            currentSpawnInterval = 5f; // Spawn gasoline very frequently
            RestartGasolineSpawnTimer();
            transform.localScale *= 1.2f;
        }
    }

    private void TriggerPhase(int phaseIndex, string message)
    {
        StartConversation(message);
        phaseTriggered[phaseIndex] = true;
    }

    private void RestartGasolineSpawnTimer()
    {
        CancelInvoke(nameof(SpawnGasolineTank)); // Stop the current timer
        InvokeRepeating(nameof(SpawnGasolineTank), 0f, currentSpawnInterval); // Start a new timer with the updated interval
    }

    private void SpawnGasolineTank()
    {
        if (!isEncounterActive) return;
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Check if a tank already exists at the spawn point
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPoint.position, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("GasolineTank"))
            {
                Debug.Log("A gasoline tank already exists at this spawn point. Skipping spawn.");
                return;
            }
        }

        Instantiate(gasolineTankPrefab, spawnPoint.position, Quaternion.identity);
    }

    private GameObject FindClosestGasolineTank()
    {
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("GasolineTank");
        GameObject closestTank = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject tank in tanks)
        {
            float distance = Vector2.Distance(transform.position, tank.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTank = tank;
            }
        }

        return closestTank;
    }

    private void ConsumeGasolineTank(GameObject tank)
    {
        Destroy(tank);
        StartSpecialAttack();
    }

    private void StartSpecialAttack()
    {
        if (isSpecialAttackActive)
        {
            Debug.LogWarning("Special attack already active. Ignoring additional call.");
            return;
        }

        Debug.Log("Starting special attack.");
        StartCoroutine(SpecialAttackPattern());
    }

    private IEnumerator SpecialAttackPattern()
    {
        Debug.Log("SpecialAttackPattern started.");
        isSpecialAttackActive = true;

        float timeout = 10f; // Maximum time for the special attack
        float startTime = Time.time;

        try
        {
            for (int i = 0; i < specialAttackIterations; i++)
            {
                Debug.Log($"Special attack iteration {i + 1}/{specialAttackIterations}.");
                Vector2 chargeDirection = (player.position - transform.position).normalized;

                animator.SetFloat("IdleX", Mathf.Round(chargeDirection.x));
                animator.SetFloat("IdleY", Mathf.Round(chargeDirection.y));
                animator.SetFloat("Speed", specialAttackSpeed * 2f);

                float elapsedTime = 0f;
                float lastFireSpawnTime = 0f;

                while (elapsedTime < chargeDistance / chargeSpeed)
                {
                    transform.position += (Vector3)(chargeDirection * chargeSpeed * Time.deltaTime);

                    if (elapsedTime - lastFireSpawnTime >= fireSpawnInterval)
                    {
                        SpawnFireTrail(transform.position);
                        lastFireSpawnTime = elapsedTime;
                    }

                    elapsedTime += Time.deltaTime;
                    yield return null;

                    // Timeout check
                    if (Time.time - startTime > timeout)
                    {
                        Debug.LogError("SpecialAttackPattern timed out!");
                        yield break;
                    }
                }

                yield return new WaitForSeconds(1f);
            }
        }
        finally
        {
            Debug.Log("SpecialAttackPattern completed or interrupted.");
            isSpecialAttackActive = false; // Ensure the flag is reset
            animator.SetFloat("Speed", 0);
            animator.SetTrigger("Idle");
        }
    }

    private void StartConversation(string message)
    {
        if (activeChatBubble != null)
            return;

        isSpeechActive = true; // Set speech state
        isPlayerLocked = true;
        player.GetComponent<Movement>().isLocked = true;
        player.GetComponent<PlayerController>().isLocked = true;
        animator.SetFloat("Speed", 0);
        animator.SetTrigger("Idle");

        activeChatBubble = Instantiate(chatBubblePrefab, chatBubbleParent);
        activeChatBubble.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = message;
        StartCoroutine(EndConversation());
    }

    private IEnumerator EndConversation()
    {
        yield return new WaitForSeconds(3f);
        if (activeChatBubble != null)
        {
            Destroy(activeChatBubble);
            activeChatBubble = null;
        }
        isSpeechActive = false; // End speech state
        isPlayerLocked = false;
        player.GetComponent<Movement>().isLocked = false;
        player.GetComponent<PlayerController>().isLocked = false;
        animator.ResetTrigger("Idle");
    }

    private void SpawnFireTrail(Vector3 position)
    {
        GameObject fireInstance = Instantiate(fireTrailPrefab, position, Quaternion.identity);

        // Randomize scale
        float randomScale = Random.Range(3f, 5f);
        fireInstance.transform.localScale = new Vector3(randomScale, randomScale, 1f);

        // Randomize rotation
        float randomRotation = Random.Range(-50f, 50f);
        fireInstance.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        Destroy(fireInstance, 8f);
    }
}
