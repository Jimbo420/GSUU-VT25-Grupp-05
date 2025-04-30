using System.Collections;
using UnityEngine;

public class BossEncounter : MonoBehaviour
{
    [Header("Encounter Settings")]
    [Tooltip("Distance from the player to start the encounter.")]
    public float encounterStartDistance = 11f;

    [Header("Player Settings")]
    [Tooltip("Reference to the player transform.")]
    public Transform player;

    [Header("Phase Settings")]
    [Tooltip("Health thresholds for each phase (in percentage).")]
    public float[] phaseThresholds = { 0.75f, 0.5f, 0.25f };
    [Tooltip("Messages for each phase.")]
    public string[] phaseMessages = {
        "Get out of my factory!",
        "Now I'm getting mad! More gasoline!",
        "ARGHHHHH! BACKUP! I NEED BACKUP!"
    };

    [Header("Special Attack Settings")]
    public int specialAttackIterations = 3;
    public float chargeDistance = 10f;
    public float chargeSpeed = 36f;

    [Header("First Encounter Settings")]
    [Tooltip("Initial dialogue message for the first encounter.")]
    public string firstEncounterMessage = "So you're the one who's been meddling in my factory! You will pay dearly for this!";
    [Tooltip("Duration of the first encounter dialogue (in seconds).")]
    public float firstEncounterDialogueDuration = 3f;

    [Header("Chat Bubble Settings")]
    [Tooltip("Prefab for the chat bubble.")]
    public GameObject chatBubblePrefab;
    [Tooltip("Parent transform for the chat bubble.")]
    public Transform chatBubbleParent;

    [Header("Fire Trail Settings")]
    [Tooltip("Prefab for the fire trail.")]
    public GameObject fireTrailPrefab;

    private BossHealth health;
    private BossMovement movement;
    private BossAttack attack;
    private BossSpawner spawner;
    private Animator animator;

    private bool[] phaseTriggered;
    private bool isSpecialAttackActive = false;
    public bool isEncounterActive = false;
    private bool isFirstEncounterComplete = false;
    private float lastSpecialAttackTime;
    private GameObject activeChatBubble;

    void Awake()
    {
        // Get references to other components
        health = GetComponent<BossHealth>();
        movement = GetComponent<BossMovement>();
        attack = GetComponent<BossAttack>();
        spawner = GetComponent<BossSpawner>();
        animator = GetComponent<Animator>();

        // Initialize phase tracking
        phaseTriggered = new bool[phaseThresholds.Length];
    }

    void Update()
    {
        if (!isEncounterActive || health.currentHealth <= 0)
            return;

        if (isFirstEncounterComplete)
        {
            HandlePhases();
        }
    }

    public void StartEncounter()
    {
        if (isEncounterActive)
            return;

        isEncounterActive = true;
        StartCoroutine(FirstEncounterRoutine());
    }

    private IEnumerator FirstEncounterRoutine()
    {
        // Lock movement and attacks during the first encounter dialogue
        Debug.Log("First encounter started.");
        movement.enabled = false; // Disable movement logic
        attack.enabled = false; // Disable attacks

        // Display the first encounter dialogue using a chat bubble
        ShowChatBubble(firstEncounterMessage);

        // Wait for the dialogue duration
        yield return new WaitForSeconds(firstEncounterDialogueDuration);

        // Remove the chat bubble
        HideChatBubble();

        // Unlock movement and attacks
        movement.enabled = true; // Enable movement logic
        attack.enabled = true; // Enable attacks

        // Start spawning gasoline tanks
        spawner.StartSpawning();

        isFirstEncounterComplete = true;
        Debug.Log("First encounter completed. Boss is now active.");
    }

    private void ShowChatBubble(string message)
    {
        if (chatBubblePrefab == null || chatBubbleParent == null)
        {
            Debug.LogWarning("Chat bubble prefab or parent is not assigned.");
            return;
        }

        // Instantiate the chat bubble
        activeChatBubble = Instantiate(chatBubblePrefab, chatBubbleParent);
        var textComponent = activeChatBubble.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = message;
        }
    }

    private void HideChatBubble()
    {
        if (activeChatBubble != null)
        {
            Destroy(activeChatBubble);
            activeChatBubble = null;
        }
    }

    private void HandlePhases()
    {
        float currentHealthPercentage = health.currentHealth / health.maxHealth;

        for (int i = 0; i < phaseThresholds.Length; i++)
        {
            if (!phaseTriggered[i] && currentHealthPercentage <= phaseThresholds[i])
            {
                TriggerPhase(i);
            }
        }

        // Prioritize gasoline tanks if available
        if (isFirstEncounterComplete && !isSpecialAttackActive)
        {
            MoveToGasolineTank();
        }
    }

    private void TriggerPhase(int phaseIndex)
    {
        Debug.Log($"Phase {phaseIndex + 1} triggered: {phaseMessages[phaseIndex]}");
        phaseTriggered[phaseIndex] = true;

        // Display the phase message in a chat bubble
        ShowChatBubble(phaseMessages[phaseIndex]);

        // Adjust behavior based on the phase
        if (phaseIndex == 0)
        {
            // Phase 1: Slightly faster and slightly larger
            movement.moveSpeed *= 1.25f;
            attack.attackDamage *= 1.5f;
            transform.localScale *= 1.1f; // Increase size by 10%
        }
        else if (phaseIndex == 1)
        {
            // Phase 2: Faster and more powerful
            movement.moveSpeed *= 1.5f;
            attack.attackDamage *= 2f;
            transform.localScale *= 1.2f; // Increase size by 20%
            spawner.SpawnGasolineTank();
        }
        else if (phaseIndex == 2)
        {
            // Phase 3: Even faster and even more powerful
            movement.moveSpeed *= 1.75f;
            attack.attackDamage *= 2.5f;
            transform.localScale *= 1.3f; // Increase size by 30%
            spawner.SpawnEnemiesAtAllPoints();
        }
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

    private void MoveToGasolineTank()
    {
        GameObject closestTank = FindClosestGasolineTank();

        if (closestTank != null)
        {
            // Set the gasoline tank as the target
            movement.target = closestTank.transform;

            // Temporarily increase the movement speed for gasoline tank movement
            movement.SetTemporaryMoveSpeed(movement.gasTankMoveSpeed);

            // Check if Gus is close enough to consume the tank
            if (Vector2.Distance(transform.position, closestTank.transform.position) < 1f)
            {
                ConsumeGasolineTank(closestTank);
                movement.ResetMoveSpeed(); // Reset the movement speed after consuming the tank
            }
        }
        else
        {
            // No gasoline tank available, switch back to chasing the player
            if (player != null)
            {
                Debug.Log("No gasoline tank found. Switching to chasing the player.");
                movement.target = player; // Default to chasing the player
                movement.ResetMoveSpeed(); // Reset the movement speed
            }
            else
            {
                Debug.LogWarning("Player reference is not assigned. Gus has no valid target.");
            }
        }
    }

    private void ConsumeGasolineTank(GameObject tank)
    {
        if (tank != null)
        {
            Destroy(tank);
            Debug.Log("Gasoline tank consumed. Triggering special attack.");
            StartSpecialAttack(); // Trigger the special attack after consuming the tank
        }

        // Reset target to the player after consuming the tank
        movement.target = health.transform;
    }

    private void StartSpecialAttack()
    {
        if (isSpecialAttackActive)
            return;

        Debug.Log("Starting special attack!");

        // Disable normal attacks and collider during the special attack
        attack.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(SpecialAttackRoutine());
    }

    private IEnumerator SpecialAttackRoutine()
    {
        isSpecialAttackActive = true;

        for (int i = 0; i < specialAttackIterations; i++)
        {
            Debug.Log($"Special attack iteration {i + 1}/{specialAttackIterations}");

            // Calculate the charge direction toward the player
            Vector2 chargeDirection = (player.position - transform.position).normalized;

            // Ensure the animator reflects the charge direction
            animator.SetFloat("IdleX", Mathf.Round(chargeDirection.x));
            animator.SetFloat("IdleY", Mathf.Round(chargeDirection.y));
            animator.SetFloat("Speed", chargeSpeed);

            float elapsedTime = 0f;
            float chargeDuration = chargeDistance / chargeSpeed; // Time to cover the charge distance

            while (elapsedTime < chargeDuration)
            {
                // Perform a raycast to detect obstacles
                RaycastHit2D hit = Physics2D.Raycast(transform.position, chargeDirection, chargeSpeed * Time.deltaTime, LayerMask.GetMask("Walls"));
                if (hit.collider != null)
                {
                    Debug.Log($"Charge stopped due to collision with {hit.collider.name}");
                    break; // Stop the charge if a wall is detected
                }

                // Move Gus toward the player
                transform.position += (Vector3)(chargeDirection * chargeSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;

                // Spawn fire trails at intervals
                if (elapsedTime % 0.02f < Time.deltaTime) // Adjust interval as needed
                {
                    SpawnFireTrail(transform.position);
                }

                yield return null;
            }

            // Pause briefly between iterations
            yield return new WaitForSeconds(1f);
        }

        isSpecialAttackActive = false;

        // Re-enable normal attacks and collider after the special attack
        attack.enabled = true;
        GetComponent<Collider2D>().enabled = true;

        Debug.Log("Special attack completed.");
    }

    private void SpawnFireTrail(Vector3 position)
    {
        if (fireTrailPrefab == null)
        {
            Debug.LogWarning("Fire trail prefab is not assigned.");
            return;
        }

        GameObject fireInstance = Instantiate(fireTrailPrefab, position, Quaternion.identity);

        // Randomize scale
        float randomScale = Random.Range(3f, 5f);
        fireInstance.transform.localScale = new Vector3(randomScale, randomScale, 1f);

        // Randomize rotation
        float randomRotation = Random.Range(-50f, 50f);
        fireInstance.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        Destroy(fireInstance, 8f); // Destroy fire trail after 8 seconds
    }
}
