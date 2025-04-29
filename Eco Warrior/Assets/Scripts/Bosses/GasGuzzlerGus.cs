using UnityEngine;

public class GasGuzzlerGus : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Reference to the player transform.")]
    public Transform player;

    [Header("Component References")]
    private BossMovement movement;
    private BossAttack attack;
    private BossHealth health;
    private BossSpawner spawner;
    private BossBoundaryHandler boundaryHandler;
    private BossEncounter encounter;

    private Animator animator;

    void Awake()
    {
        // Get references to the modular components
        movement = GetComponent<BossMovement>();
        attack = GetComponent<BossAttack>();
        health = GetComponent<BossHealth>();
        spawner = GetComponent<BossSpawner>();
        boundaryHandler = GetComponent<BossBoundaryHandler>();
        encounter = GetComponent<BossEncounter>();
        animator = GetComponent<Animator>();

        // Initialize components
        movement.Initialize(player, animator);
        attack.Initialize(animator, player); // Pass the animator and player transform to BossAttack
        health.InitializeHealth();
    }

    void Start()
    {
        Debug.Log("GasGuzzlerGus initialized.");
    }

    void Update()
    {
        if (health.currentHealth <= 0)
            return; // Stop all logic if the boss is dead

        // Check if the encounter should start
        if (!encounter.isEncounterActive && Vector2.Distance(transform.position, player.position) < encounter.encounterStartDistance)
        {
            encounter.StartEncounter();
            return;
        }

        // Delegate logic to modular components
        if (encounter.isEncounterActive)
        {
            movement.HandleMovement();
            attack.HandleAttack();
            boundaryHandler.CheckBoundary();
        }
    }
}
