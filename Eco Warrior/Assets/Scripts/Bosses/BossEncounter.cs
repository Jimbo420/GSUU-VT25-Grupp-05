using System.Collections;
using System.Collections.Generic;
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
        // Lock movement during the first encounter dialogue
        //Debug.Log("First encounter started.");
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
        //Debug.Log($"Phase {phaseIndex + 1} triggered: {phase.phaseMessage}");
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
                    else
                    {
                        //Debug.LogWarning("No custom spawn points defined for this phase.");
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
                //Debug.LogWarning($"Unknown modifier key: {modifier.key}");
                break;
        }
    }

    private void ShowChatBubble(string message)
    {
        if (chatBubblePrefab == null || chatBubbleParent == null)
        {
            //Debug.LogWarning("Chat bubble prefab or parent is not assigned.");
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

    public void TriggerSpecialAttack()
    {
        if (specialAttack != null)
        {
            specialAttack.Execute();
        }
        else
        {
            //Debug.LogWarning("No special attack assigned to the boss.");
        }
    }
}
