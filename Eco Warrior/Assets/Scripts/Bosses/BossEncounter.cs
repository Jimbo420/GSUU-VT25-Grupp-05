using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEncounter : MonoBehaviour
{
    [Header("Encounter Settings")]
    [Tooltip("Distance from the player to start the encounter.")]
    public float encounterStartDistance = 11f;

    [Header("Phase Settings")]
    [Tooltip("Phases for the boss encounter.")]
    public BossPhase[] phases; // Array of BossPhase ScriptableObjects

    [Header("Special Attack Settings")]
    [Tooltip("Special attack for the boss.")]
    public BossSpecialAttack specialAttack;

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

    private BossHealth health;
    private BossMovement movement;
    private BossSpawner spawner;

    private bool[] phaseTriggered;
    private bool isEncounterActive = false;
    private bool isFirstEncounterComplete = false;
    private GameObject activeChatBubble;

    private Transform player; // Removed public reference and made it private

    // Public read-only property to expose isEncounterActive
    public bool IsEncounterActive => isEncounterActive;

    void Awake()
    {
        // Get references to other components
        health = GetComponent<BossHealth>();
        movement = GetComponent<BossMovement>();
        spawner = GetComponent<BossSpawner>();

        // Initialize phase tracking
        phaseTriggered = new bool[phases.Length];
    }

    void Update()
    {
        // Keep checking for the player if it hasn't been found yet
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                Debug.Log("Player found by BossEncounter.");
            }
        }

        // If the player is found, check the distance to start the encounter
        if (player != null && !isEncounterActive)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= encounterStartDistance)
            {
                StartEncounter();
            }
        }

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

        if (player == null)
        {
            Debug.LogError("Player not found. Cannot start the encounter.");
            return;
        }

        isEncounterActive = true;

        MusicManager.Instance.PlayTensionMusic();

        // Set the player's Transform as the target for the boss movement
        if (movement != null)
        {
            movement.target = player;
            Debug.Log("Boss target set to player.");
        }
        else
        {
            Debug.LogWarning("BossMovement component is missing on the boss.");
        }

        StartCoroutine(FirstEncounterRoutine());
    }

    private IEnumerator FirstEncounterRoutine()
    {
        // Lock movement during the first encounter dialogue
        movement.enabled = false;

        // Display the first encounter dialogue using a chat bubble
        ShowChatBubble(firstEncounterMessage);

        // Wait for the dialogue duration
        yield return new WaitForSeconds(firstEncounterDialogueDuration);

        // Remove the chat bubble
        HideChatBubble();

        // Unlock movement
        movement.enabled = true;

        // Start spawning gasoline tanks (if applicable)
        if (spawner != null)
        {
            spawner.StartSpawningAtDefaultPoints(spawner.defaultPrefab, spawner.defaultSpawnInterval);
        }

        isFirstEncounterComplete = true;
        Debug.Log("First encounter completed. Boss is now active.");
    }

    private void HandlePhases()
    {
        float currentHealthPercentage = health.currentHealth / health.maxHealth;

        for (int i = 0; i < phases.Length; i++)
        {
            if (!phaseTriggered[i] && currentHealthPercentage <= phases[i].healthThreshold)
            {
                TriggerPhase(i);
            }
        }
    }

    private void TriggerPhase(int phaseIndex)
    {
        BossPhase phase = phases[phaseIndex];
        phaseTriggered[phaseIndex] = true;

        // Display the phase message in a chat bubble
        ShowChatBubble(phase.phaseMessage);

        // Hide the chat bubble after 6 seconds
        StartCoroutine(HideChatBubbleAfterDelay(6f));

        // Apply modifiers
        foreach (var modifier in phase.modifiers)
        {
            ApplyModifier(modifier);
        }

        // Trigger phase start events
        phase.onPhaseStart?.Invoke();

        // Handle spawning
        if (phase.spawnPrefab != null)
        {
            switch (phase.spawnPointType)
            {
                case BossPhase.SpawnPointType.Default:
                    spawner.SpawnAtDefaultPoints(phase.spawnPrefab, phase.useAllSpawnPoints, phase.spawnQuantity);
                    break;

                case BossPhase.SpawnPointType.Special:
                    spawner.SpawnAtSpecialPoints(phase.spawnPrefab, phase.useAllSpawnPoints, phase.spawnQuantity);
                    break;

                case BossPhase.SpawnPointType.Custom:
                    if (phase.customSpawnPoints != null && phase.customSpawnPoints.Length > 0)
                    {
                        spawner.StartSpawning(phase.spawnPrefab, spawner.defaultSpawnInterval, new List<Transform>(phase.customSpawnPoints));
                    }
                    break;
            }
        }
    }

    private IEnumerator HideChatBubbleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideChatBubble();
    }

    private void ApplyModifier(Modifier modifier)
    {
        switch (modifier.key)
        {
            case "MoveSpeed":
                movement.moveSpeed *= modifier.value;
                break;
            case "Size":
                transform.localScale *= modifier.value;
                break;
            default:
                break;
        }
    }

    private void ShowChatBubble(string message)
    {
        // Destroy the old chat bubble if it exists
        if (activeChatBubble != null)
        {
            Destroy(activeChatBubble);
            activeChatBubble = null;
        }

        if (chatBubblePrefab == null || chatBubbleParent == null)
        {
            Debug.LogWarning("Chat bubble prefab or parent is not assigned.");
            return;
        }

        // Instantiate the new chat bubble
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

    public void TriggerSpecialAttack()
    {
        if (specialAttack != null)
        {
            specialAttack.Execute();
        }
    }
}
