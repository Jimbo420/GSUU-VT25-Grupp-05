using System.Collections;
using UnityEngine;

public class GasGuzzlerGus : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Reference to the player transform.")]
    public Transform player;

    [Header("Movement Settings")]
    [Tooltip("Movement speed when chasing the player.")]
    [SerializeField] private float moveSpeed = 1f;
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
    [SerializeField] private float stuckThreshold = 3f;
    [Tooltip("Minimum distance Gus must move to not be considered stuck.")]
    [SerializeField] private float stuckDistanceThreshold = 0.1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        healthBar.Health(healthBar.slider.maxValue, healthBar.slider.maxValue); // Initialize health bar

        // Initialize stuck detection and starting location
        lastPosition = transform.position;
        stuckTimer = 0f;
        startingLocation = transform.position;
    }

    void Update()
    {
        if (isSpeechActive) // Prevent actions during speech
        {
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
                ChasePlayer();
            else if (Time.time > lastAttackTime + attackCooldown)
                StartAttack();

            Vector2 direction = (player.position - transform.position).normalized;
            animator.SetFloat("IdleX", Mathf.Round(direction.x));
            animator.SetFloat("IdleY", Mathf.Round(direction.y));
        }

        CheckHealthPhases();
        CheckIfStuck();
        CheckBoundary();
    }

    private void StartEncounter()
    {
        isEncounterActive = true;
        StartConversation("So you're the one who's been meddling in my factory! You will pay dearly for this!");

        // Start spawning gasoline tanks after the encounter begins
        InvokeRepeating(nameof(SpawnGasolineTank), 10f, 20f);
    }

    void ChasePlayer()
    {
        MoveTowards(player.position, moveSpeed);
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
            TriggerPhase(0, "You dare challenge me further? I'll make this your grave!");
        }
        else if (currentHealth <= maxHealth * 0.5f && !phaseTriggered[1])
        {
            TriggerPhase(1, "This battle is far from over! Witness my power!");
        }
        else if (currentHealth <= maxHealth * 0.25f && !phaseTriggered[2])
        {
            TriggerPhase(2, "You've pushed me to my limits, but I'll not fall easily!");
        }
    }

    private void TriggerPhase(int phaseIndex, string message)
    {
        StartConversation(message);
        phaseTriggered[phaseIndex] = true;
    }

    private void SpawnGasolineTank()
    {
        if (!isEncounterActive) return;
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
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
        StartCoroutine(SpecialAttackPattern());
    }

    private IEnumerator SpecialAttackPattern()
    {
        isSpecialAttackActive = true;

        for (int i = 0; i < specialAttackIterations; i++)
        {
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
            }

            Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, 1f);
            if (hitPlayer != null && hitPlayer.CompareTag("Player"))
            {
                Rigidbody2D playerRb = hitPlayer.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockbackDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                    playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }
                hitPlayer.GetComponent<HealthbarBehavior>().HitDamage(attackDamage, gameObject);
            }

            yield return new WaitForSeconds(1f);
        }

        animator.SetFloat("Speed", 0);
        animator.SetTrigger("Idle");
        isSpecialAttackActive = false;
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
        float randomScale = Random.Range(2f, 4f);
        fireInstance.transform.localScale = new Vector3(randomScale, randomScale, 1f);

        // Randomize rotation
        float randomRotation = Random.Range(-30f, 30f);
        fireInstance.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        Destroy(fireInstance, 8f);
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        animator.SetFloat("IdleX", Mathf.Round(direction.x));
        animator.SetFloat("IdleY", Mathf.Round(direction.y));
        animator.SetFloat("Speed", speed);
    }

    private void CheckIfStuck()
    {
        float distanceMoved = Vector2.Distance(transform.position, lastPosition);

        if (distanceMoved < stuckDistanceThreshold)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckThreshold)
            {
                Debug.LogWarning("Gus is stuck! Resetting state...");
                ResetStuckState();
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }

    private void ResetStuckState()
    {
        isSpecialAttackActive = false;
        isPlayerLocked = false;
        ChasePlayer();
    }

    private void CheckBoundary()
    {
        if (Vector3.Distance(transform.position, startingLocation) > boundaryRadius)
        {
            Debug.LogWarning("Gus is outside the boundary! Teleporting back to starting location...");
            transform.position = startingLocation;
        }
    }
}
